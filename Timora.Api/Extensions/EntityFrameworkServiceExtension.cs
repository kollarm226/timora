using Microsoft.EntityFrameworkCore;
using Timora.Data.Data;

namespace Timora.Api.Extensions
{
    /// <summary>
    /// Extension methods for configuring Entity Framework services.
    /// </summary>
    public static class EntityFrameworkServiceExtension
    {
        /// <summary>
        /// Adds Entity Framework services with SQLite configuration.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The application configuration</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection AddEntityFrameworkServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            var connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' not found."
                );

            services.AddDbContext<TimoraDbContext>(options => options.UseSqlite(connectionString));

            return services;
        }
    }
}
