using Domain.LookupModels;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
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
            if (!context.Qualifications.Any())
            {
                var data = new List<Qualification>
                {
                    new Qualification
                    {
                        Description = "A-Levels",
                        DateCreated = DateTime.Now
                    },
                    new Qualification
                    {
                        Description = "GCSEs",
                        DateCreated = DateTime.Now
                    },
                    new Qualification
                    {
                        Description = "Bachelors",
                        DateCreated = DateTime.Now
                    },
                    new Qualification
                    {
                        Description = "Masters",
                        DateCreated = DateTime.Now
                    },
                    new Qualification
                    {
                        Description = "PhD",
                        DateCreated = DateTime.Now
                    },
                    new Qualification
                    {
                        Description = "Other",
                        DateCreated = DateTime.Now
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
                        DateRegistered = DateTime.Now,
                        CareHomeId = 1
                    },
                    new AppUser
                    {
                        FirstName = "Idris",
                        LastName = "Admin",
                        Email = "idris@admin.com",
                        UserName = "idris@admin.com",
                        EmailConfirmed = true,
                        IsEnabled = true,
                        DateCreated = DateTime.Now,
                        DateRegistered = DateTime.Now

                    }

                };
                foreach(var user in users)
                {
                    await userManager.CreateAsync(user, "Password1");
                    if (user.Email == "idris@manager.com")                    {
                        await userManager.AddToRoleAsync(user, "Manager");
                    }
                    if (user.Email == "idris@admin.com")
                    {
                        await userManager.AddToRoleAsync(user, "Admin");

                    }
                }

            }
        }
    }
}
