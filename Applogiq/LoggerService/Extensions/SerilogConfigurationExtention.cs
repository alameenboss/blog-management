namespace Applogiq.LoggerService.Extensions
{
    public static class SerilogConfigurationExtention
    {
        public static IConfigurationBuilder SeriLogConfiguration(this IConfigurationBuilder builder, IWebHostEnvironment env)
        {
            string filePath = Path.Combine(PathHelperExtention.GetAssemblyDirectory(), "LoggerService");
            builder.AddJsonFile(Path.Combine(filePath, $"serilogsettings.{env.EnvironmentName}.json"), optional: true);
            return builder;
        }
    }
}