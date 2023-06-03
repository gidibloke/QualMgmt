using Application.Core;
using Application.Helpers;
using Application.Interfaces;
using Application.ViewModels;
using AutoMapper;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System.Security.Claims;

namespace Application.Repositories
{
    public class UserManagementRepository : IUserManagement
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailer _emailer;
        private readonly UserManager<AppUser> _userManager;
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContext;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IMapper _mapper;

        public UserManagementRepository(ApplicationDbContext context, IEmailer emailer, UserManager<AppUser> userManager, LinkGenerator linkGenerator, IHttpContextAccessor httpContext, RoleManager<AppRole> roleManager, IMapper mapper)
        {
            _context = context;
            _emailer = emailer;
            _userManager = userManager;
            _linkGenerator = linkGenerator;
            _httpContext = httpContext;
            _roleManager = roleManager;
            _mapper = mapper;
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

        public async Task<Result<IdentityResult>> Register(AddUserViewModel model)
        {
            var user = _mapper.Map<AppUser>(model);
            user.DateCreated = DateTime.Now;
            user.IsEnabled = false;
            user.UserName = model.Email;
            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                var role = await _roleManager.FindByIdAsync(model.RoleId);
                await _userManager.AddToRoleAsync(user, role.Name);
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var url = _linkGenerator.GetUriByAction("ConfirmEmail", "Account", values: new
                {
                    userId = user.Id,
                    code = token
                }, _httpContext.HttpContext.Request.Scheme, _httpContext.HttpContext.Request.Host);

                var email = new EmailMessage
                {
                    To = model.Email,
                    EmailBody = url
                };
                //You can use hangfire to send this message
                await _emailer.PrepareAndSendEmail(EmailTypeEnum.ConfirmEmail, email);
                return Result<IdentityResult>.Success(result);
            }
            return Result<IdentityResult>.Failure("Error adding user");
        }

        public async Task<Result<object>> ChangeCareHomeAsync(EditUserViewModel model)
        {
            var user = await _context.Users.FindAsync(model.UserId);
            if (user == null)
            {
                return Result<object>.Failure("User not found");
            }
            user.CareHomeId = model.CareHomeId;
            _context.Update(user);
            var success = await _context.SaveChangesAsync() > 0;
            if (success)
                return Result<object>.Success(new { success = true, message = "User disabled" });
            else
                return Result<object>.Failure("User not found");

        }

        public async Task<Result<object>> DisableUserAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return Result<object>.Failure("User not found");
            }
            var currentUser = _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(currentUser == user.Id)
            {
                return Result<object>.Failure("You can disable your account while logged in");
            }
            user.IsEnabled = false;
            var success = await _context.SaveChangesAsync() > 0;
            if (success)
                return Result<object>.Success(new {success = true, message = "User disabled"});
            else
                return Result<object>.Failure("User not found");

        }

        public async Task<Result<object>> EnableUserAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return Result<object>.Failure("User not found");
            }
            user.IsEnabled = true;
            var success = await _context.SaveChangesAsync() > 0;
            if (success)
                return Result<object>.Success(new { success = true, message = "User disabled" });
            else
                return Result<object>.Failure("User not found");
        }

        public async Task<Result<EditUserViewModel>> GetEditViewAsync(string userId)
        {
            var user = await _context.Users.Where(x => x.Id == userId)
                .Include(x => x.CareHome).SingleOrDefaultAsync();
            if (user == null)
            {
                return Result<EditUserViewModel>.Failure("User not found");
            }
            var model = new EditUserViewModel
            {
                UserId = user.Id,
                CareHomeId = user.CareHomeId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
            await PopulateViewModel(model);
            return Result<EditUserViewModel>.Success(model);
   
        }

        public async Task<Result<List<UsersViewModel>>> GetUsersAsync()
        {
            var data = new List<UsersViewModel>();
            var r = await _context.Users.Include(x => x.CareHome)
                    .SelectMany(//--below emulates a left outer join, as it returns DefaultIfEmpty in the collectionSelector
                                user => _context.UserRoles.Where(userRoleMapEntry => user.Id == userRoleMapEntry.UserId).DefaultIfEmpty(),
                                (user, roleMapEntry) => new { User = user, RoleMapEntry = roleMapEntry })
                                .SelectMany(
                                    // perform the same operation to convert role IDs from the role map entry to roles
                                    x => _context.Roles.Where(role => role.Id == x.RoleMapEntry.RoleId).DefaultIfEmpty(),
                                    (x, role) => new { x.User, Role = role })
                                .ToListAsync(); // runs the queries and sends us back into EF Core LINQ world
            var result = r.Aggregate(
                        new Dictionary<AppUser, List<AppRole>>(), // seed
                        (dict, data) => {
                            // safely ensure the user entry is configured
                            dict.TryAdd(data.User, new List<AppRole>());
                            if (null != data.Role)
                            {
                                dict[data.User].Add(data.Role);
                            }
                            return dict;
                        }, x => x);
            foreach (KeyValuePair<AppUser, List<AppRole>> item in result)
            {
                var user = item.Key;
                var role = item.Value;
                var returnValue = new UsersViewModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Fullname = $"{user.FirstName} {user.LastName}",
                    CareHomeName = user.CareHome?.HomeName,
                    RoleName = role.FirstOrDefault().ReadableName,
                    EmailConfirmed = user.EmailConfirmed,
                    IsEnabled = user.IsEnabled
                };
                data.Add(returnValue);
            }
            return Result<List<UsersViewModel>>.Success(data);
        }

        public async Task<Result<object>> UpdateEmailAsync(EditUserViewModel model)
        {
            var user = await _context.Users.FindAsync(model.UserId);
            if (user == null)
            {
                return Result<object>.Failure("User not found");
            }
            user.Email = model.Email;
            user.NormalizedEmail = model.Email;
            user.UserName = model.Email;
            user.IsEnabled = false;
            user.EmailConfirmed = false;
            _context.Update(user);
            var result = await _context.SaveChangesAsync() > 0;
            if (result)
            {
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
            }
            throw new NotImplementedException();
        }

        private async Task PopulateViewModel(EditUserViewModel model)
        {
            var homes = await _context.CareHomes.ToListAsync();
            model.CareHomes = homes.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.HomeName
            });
        }
    }
}
