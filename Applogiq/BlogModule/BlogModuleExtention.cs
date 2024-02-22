using Applogiq.BlogModule.Repositories;
using Applogiq.BlogModule.Services;
using Applogiq.Common.EFCore;
using Microsoft.EntityFrameworkCore;

namespace Applogiq.BlogModule
{
    public static class BlogModuleExtention
    {
        
        public static IServiceCollection RegisterBlogModule(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<BlogDbContext>(options => {
                options.UseSqlServer(configuration.GetConnectionString("BlogDBConnection"),
                     o => o.MigrationsHistoryTable("__BlogDb_EFMigrationsHistory"));
            }, ServiceLifetime.Scoped);

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IBlogService, BlogService>();
            services.AddScoped<IBlogRepository, BlogRepository>();

            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            return services;
        }
    }
}