namespace Applogiq.Common
{
    public static class IISIntegrationExtension
    {
        public static void ConfigureIISIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(options =>
            {
            });
        }
    }
}