using Serilog;

namespace Applogiq
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                IHost host = CreateHostBuilder(args).Build();

                IWebHostEnvironment env = host.Services.GetRequiredService<IWebHostEnvironment>();

                Log.Information("Starting host...");

                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                    .UseKestrel()
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseIISIntegration()
                    .UseStartup<Startup>();
                });
        }
    }
}