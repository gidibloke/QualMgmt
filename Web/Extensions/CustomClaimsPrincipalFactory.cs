using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Persistence;
using System.Security.Claims;

namespace Web.Extensions
{
    public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<AppUser, AppRole>
    {
        private readonly ApplicationDbContext _context;

        public CustomClaimsPrincipalFactory(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IOptions<IdentityOptions> options, ApplicationDbContext context) : base(userManager, roleManager, options)
        {
            _context = context;
        }

        protected async override Task<ClaimsIdentity> GenerateClaimsAsync(AppUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            var careHome = await _context.CareHomes.Where(x => x.Id == user.CareHomeId).SingleOrDefaultAsync();
            var claims = new List<Claim>
            {
                new Claim("FullName", $"{user.FirstName} {user.LastName}"),
            };
            identity.AddClaims(claims);
            if(careHome != null)
            {
                var newClaims = new List<Claim>
                {
                    new Claim("CareHomeId", user.CareHomeId.ToString()),
                    new Claim("CareHomeName", careHome.HomeName)
                };
                identity.AddClaims(newClaims);
            }
            return identity;
        }
    }
}
