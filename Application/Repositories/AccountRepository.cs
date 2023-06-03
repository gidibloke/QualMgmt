using Application.Core;
using Application.Interfaces;
using Application.ViewModels;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Microsoft.AspNetCore.Mvc.Rendering;
using AutoMapper;
using Application.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Application.Repositories
{
    public class AccountRepository : IAccount
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IEmailer _emailer;
        private readonly IHttpContextAccessor _httpContext;
        private readonly LinkGenerator _linkGenerator;

        public AccountRepository(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<AppRole> roleManager,
            ApplicationDbContext context, IMapper mapper, IEmailer emailer, IHttpContextAccessor httpContext, LinkGenerator linkGenerator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
            _mapper = mapper;
            _emailer = emailer;
            _httpContext = httpContext;
            _linkGenerator = linkGenerator;
        }

        public async Task<AddUserViewModel> AddUser(AddUserViewModel model)
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var homes = await _context.CareHomes.ToListAsync();
            model.Roles = roles.Select(x => new SelectListItem
            {
                Value = x.Id,
                Text = x.ReadableName
            }).ToList();
            model.HomeList = homes.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.HomeName
            });
            return model;
        }

        public async Task<Result<IdentityResult>> ConfirmEmail(ConfirmEmailViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return Result<IdentityResult>.Failure("Invalid link");
            var confirm = await _userManager.ConfirmEmailAsync(user, model.Code);
            if (confirm.Succeeded)
            {
                user.DateRegistered = DateTime.Now;
                await _userManager.UpdateAsync(user);
                var password = await _userManager.AddPasswordAsync(user, model.Password);
                return Result<IdentityResult>.Success(password);
            }
            return Result<IdentityResult>.Failure(string.Join(" ", confirm.Errors.Select(x => x.Description)));

        }

        public async Task<Result<SignInResult>> Login(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return Result<SignInResult>.Failure("Invalid login attempt");
            if (!user.IsEnabled) return Result<SignInResult>.Failure("Account disabled. Please contact dev team");
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true);
            if (result.Succeeded) return Result<SignInResult>.Success(result);
            if (result.IsLockedOut) return Result<SignInResult>.Failure("User account has been locked out");
            return Result<SignInResult>.Failure("Invalid login attempt");

        }

        public async Task LogOff()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<Result<IdentityResult>> Register(AddUserViewModel model)
        {
            var user = _mapper.Map<AppUser>(model);
            user.DateCreated = DateTime.Now;
            user.IsEnabled = false;
            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                var role = await _roleManager.FindByIdAsync(model.RoleId);
                await _userManager.AddToRoleAsync(user, role.Name);
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackLink = _linkGenerator.GetPathByAction(_httpContext.HttpContext,
                    action: "ConfirmEmail",
                    controller: "Account",
                    values: new
                    {
                        userId = user.Id,
                        code = token
                    });
                var email = new EmailMessage
                {
                    To = model.Email,
                    EmailBody = callbackLink
                };
                //You can use hangfire to send this message
                await _emailer.PrepareAndSendEmail(EmailTypeEnum.ConfirmEmail, email);
                return Result<IdentityResult>.Success(result);
            }
            return Result<IdentityResult>.Failure("Error adding user");
        }



    }
}
