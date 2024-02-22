using Applogiq.BlogModule;
using Applogiq.Common;
using Applogiq.Common.Middleware;
using Applogiq.EmailService;
using Applogiq.EmailService.Config;
using Applogiq.IdentityServer.DataSeed;
using Applogiq.IdentityServer.DTOs.User;
using Applogiq.IdentityServer.EFConfiguration;
using Applogiq.IdentityServer.Extensions;
using Applogiq.LoggerService;
using Applogiq.LoggerService.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Serilog;

namespace Applogiq
{
    public class Startup
    {
        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                   .SetBasePath(Environment.ContentRootPath)
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                   .AddJsonFile($"appsettings.{Environment.EnvironmentName}.json", optional: true)
                   .SeriLogConfiguration(Environment)
                   .AddEnvironmentVariables()
                   .Build();

            Console.WriteLine(Environment.EnvironmentName);

            services.ConfigureCors();

            services.ConfigureIISIntegration();

            ConfigureEmailService(services);

            services.ConfigureSwagger("Applogiq API", "v1");

            services.RegisterBlogModule(Configuration);

            services.ConfigureIdentity(Configuration);

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.MaxDepth = 1;
                });

            services.AddMemoryCache();

            services.AddHttpContextAccessor();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            ConfigureLogging(services);

            
        }

        private void ConfigureLogging(IServiceCollection services)
        {
            Log.Logger = new LoggerConfiguration()
                                            .Enrich
                                            .WithProperty("ApplicationName", "Applogiq")
                                            .ReadFrom
                                            .Configuration(Configuration)
                                            .CreateLogger();

            AppDomain.CurrentDomain.ProcessExit += (s, e) => Log.CloseAndFlush();
            services.AddSingleton(Log.Logger);
            services.AddScoped<ILogService, LogService>();
        }

        private void ConfigureEmailService(IServiceCollection services)
        {
            EmailConfiguration? emailConfig = Configuration
                                      .GetSection(nameof(EmailConfiguration))
                                      .Get<EmailConfiguration>();

            services.AddSingleton(emailConfig);

            services.AddScoped<IEmailSender, EmailSender>();
        }

        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            _ = env.IsDevelopment() ? app.UseDeveloperExceptionPage() : app.UseHsts();


            string pathToSave = Path.Combine(Configuration["AppConfig:FilesStoragePath"]);

            if (env.IsDevelopment())
            {
                pathToSave = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + @"Development\Files");
            }

            if (!Directory.Exists(pathToSave))
            {
                Directory.CreateDirectory(pathToSave);
            }
            app.UseFileServer(new FileServerOptions
            {

                FileProvider = new PhysicalFileProvider(pathToSave),
                RequestPath = "",
                EnableDefaultFiles = true
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCors("CorsPolicy");

            app.UseExceptionHandlerMiddleware();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });

            app.UseRouting();


            if (env.IsDevelopment())
            {
                using (IServiceScope serviceScope = app.ApplicationServices.CreateScope())
                {
                    UserDbContext userDbContext = serviceScope.ServiceProvider.GetRequiredService<UserDbContext>();
                    userDbContext.Database.Migrate();

                    BlogDbContext blogDbContext = serviceScope.ServiceProvider.GetRequiredService<BlogDbContext>();
                    blogDbContext.Database.Migrate();
                }
                Task task = IdentityServerDataInitializer.SeedData(userManager, roleManager);
                task.Wait();
            }

            app.UseAuthentication();
            app.UseAuthorization();


            app.EnableSwagger();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}