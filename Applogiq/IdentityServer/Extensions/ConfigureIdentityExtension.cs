using Applogiq.Common;
using Applogiq.IdentityServer.DTOs.User;
using Applogiq.IdentityServer.EFConfiguration;
using Applogiq.IdentityServer.JwtFeatures;
using Microsoft.AspNetCore.Identity;

namespace Applogiq.IdentityServer.Extensions
{
    public static class ConfigureIdentityExtension
    {
        public static void ConfigureIdentity(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.RegisterIdentityDbContext(configuration);

            services.AddIdentity<ApplicationUser, IdentityRole>(opt =>
            {
                opt.Password.RequiredLength = 8;
                opt.Password.RequireDigit = true;

                opt.User.RequireUniqueEmail = true;

                opt.Lockout.AllowedForNewUsers = true;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(60);
                opt.Lockout.MaxFailedAccessAttempts = 3;
            })
             .AddEntityFrameworkStores<UserDbContext>()
             .AddDefaultTokenProviders();

            services.Configure<DataProtectionTokenProviderOptions>(opt =>
                opt.TokenLifespan = TimeSpan.FromMinutes(60));

            services.ConfigureAuthentication(configuration);

            services.AddScoped<JwtHandler>();
        }

    }

}