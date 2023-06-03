using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Web.Helpers;
using Microsoft.AspNetCore.Identity;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.Services;
using Web.Interfaces;
using Web.Services;
using Application.Core;
using Application.Repositories;
using FluentValidation;
using Application.ViewModels;

namespace Web.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IWebHostEnvironment env, IConfiguration config)
        {
            services.AddHttpContextAccessor();
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(config.GetConnectionString(env.EnvironmentName));
                if (env.IsDevelopment())
                    options.EnableSensitiveDataLogging();
            });
            services.AddTransient<CustomEmailConfirmationTokenProvider<AppUser>>();
            services.AddTransient<CustomPasswordResetTokenProvider<AppUser>>();
            if (env.IsDevelopment())
            {
                services.AddDefaultIdentity<AppUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.User.RequireUniqueEmail = true;
                }).AddRoles<AppRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
                //services.AddRazorPages().AddRazorRuntimeCompilation();

            }
            else
            {
                services.AddDefaultIdentity<AppUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = true;
                    options.Password.RequireDigit = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequiredLength = 8;
                    options.Tokens.ProviderMap.Add("CustomEmailConfirmation",
                        new TokenProviderDescriptor(typeof(CustomEmailConfirmationTokenProvider<AppUser>)));
                    options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";
                    options.Tokens.ProviderMap.Add("CustomPasswordReset",
                        new TokenProviderDescriptor(typeof(CustomPasswordResetTokenProvider<AppUser>)));
                    options.Tokens.PasswordResetTokenProvider = "CustomPasswordReset";
                }).AddRoles<AppRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            }
            services.AddSession(opt =>
            {
                opt.Cookie.IsEssential = true;
            });
            services.AddHttpClient();
            services.AddValidatorsFromAssemblyContaining<CareViewModelValidator>();
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();
            services.AddScoped<IUserClaimsPrincipalFactory<AppUser>, CustomClaimsPrincipalFactory>();
            //services.AddValidatorsFromAssemblyContaining<>
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/LogOff";
                options.AccessDeniedPath = "/Account/AccessDenied";

            });
            services.AddScoped<IEmailer, EmailerService>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IAccount, AccountRepository>();
            services.AddScoped<IAdminDashboard, AdminDashboardRepository>();
            services.AddScoped<IUserManagement, UserManagementRepository>();
            services.AddScoped<ISetReturnMessages, SetReturnMessageService>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddAutoMapper(typeof(MappingProfiles).Assembly);

            return services;
        }
    }
}
