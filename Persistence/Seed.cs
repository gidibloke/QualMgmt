using Domain.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public class Seed
    {
        public async static Task SeedData(ApplicationDbContext context, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                var roles = new List<AppRole>
                {
                    new AppRole
                    {
                        Name = "Admin",
                        ReadableName = "Administrator"
                    },
                    new AppRole
                    {
                        Name = "Manager",
                        ReadableName = "Manager"
                    }
                };
                foreach(var role in roles)
                {
                    await roleManager.CreateAsync(role);
                }
            }
            if (!context.CareHomes.Any())
            {
                var data = new List<CareHome>
                {
                    new CareHome
                    {
                        HomeName = "Cambridge Care Home",
                        PostCode = "CB6 2JR"
                    },
                    new CareHome
                    {
                        HomeName = "Peterborough Care Home",
                        PostCode = "PE1 1XS"
                    }
                };
                context.AddRange(data);
                await context.SaveChangesAsync();
            }

            if (!userManager.Users.Any())
            {
                var users = new List<AppUser>
                {
                    new AppUser
                    {
                        FirstName = "Idris",
                        LastName = "Manager",
                        Email = "idris@manager.com",
                        UserName = "idris@manager.com",
                        EmailConfirmed = true,
                        IsEnabled = true,
                        DateCreated = DateTime.Now,
                        DateRegistered = DateTime.Now
                    }

                };
                foreach(var user in users)
                {
                    var result = await userManager.CreateAsync(user, "Password1");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Manager");
                    }
                }

            }
        }
    }
}
