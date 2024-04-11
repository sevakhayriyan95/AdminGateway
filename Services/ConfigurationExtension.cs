


namespace AdminGateway.Services
{
    public static class ConfigurationExtension
    {
        public static void AddRequiredAppSettings(this ConfigurationManager configuration)
        {
            string? environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            configuration.AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true)
                                 .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                                 .AddEnvironmentVariables()
                                 .Build();
        }

        public static void AddServicesOptions(this IServiceCollection services, IConfiguration configuration)
        {
        }
    }
}
