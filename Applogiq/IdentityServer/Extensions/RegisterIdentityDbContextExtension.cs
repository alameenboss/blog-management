using Applogiq.IdentityServer.EFConfiguration;
using Microsoft.EntityFrameworkCore;

namespace Applogiq.IdentityServer.Extensions
{
    public static class RegisterIdentityDbContextExtension
    {
        public static void RegisterIdentityDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<UserDbContext>(opts =>
                opts.UseSqlServer(configuration.GetConnectionString("IdentityServerDBConnection"),
                    b => b.MigrationsAssembly("Applogiq")));
        }
    }
}