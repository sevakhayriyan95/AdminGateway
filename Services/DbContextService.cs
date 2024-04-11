using AdminGateway.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AdminGateway.Services
{
    public static class DbContextService
    {
        public static IServiceCollection AddDbContexts(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AdminDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("AdminDb"),
                b => b.EnableRetryOnFailure()
                ));
            return services;
        }

    }
}
