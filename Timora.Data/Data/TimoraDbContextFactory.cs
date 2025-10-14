using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Timora.Data.Data;

/// <summary>
/// Design-time factory for creating TimoraDbContext during EF Core migrations.
/// Allows migrations to run without initializing Firebase Authentication.
/// </summary>
public class TimoraDbContextFactory : IDesignTimeDbContextFactory<TimoraDbContext>
{
    /// <summary>
    /// Creates a new instance of TimoraDbContext for design-time operations.
    /// </summary>
    /// <param name="args">Command-line arguments passed to the EF Core tools.</param>
    /// <returns>A configured TimoraDbContext instance.</returns>
    public TimoraDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Timora.Api"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<TimoraDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        optionsBuilder.UseSqlite(connectionString);

        return new TimoraDbContext(optionsBuilder.Options);
    }
}
