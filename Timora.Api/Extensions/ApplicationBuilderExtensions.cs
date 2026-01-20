using System.Diagnostics.CodeAnalysis;
using FirebaseAdmin;
using Timora.Data.Data;

namespace Timora.Api.Extensions;

/// <summary>
/// Extension methods for configuring the application pipeline.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Ensures the database is created and logs initialization status.
    /// </summary>
    public static WebApplication EnsureDatabase(this WebApplication app, IConfiguration configuration)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TimoraDbContext>();
        try
        {
            dbContext.Database.EnsureCreated();
            app.Logger.LogInformation("Database ensured/created at: {ConnectionString}",
                configuration.GetConnectionString("DefaultConnection"));
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "Failed to ensure database creation");
        }
        return app;
    }

    /// <summary>
    /// Logs Firebase Admin SDK initialization status.
    /// </summary>
    public static WebApplication LogFirebaseStatus(this WebApplication app)
    {
        if (FirebaseApp.DefaultInstance != null)
        {
            app.Logger.LogInformation("Firebase Admin SDK initialized successfully");
        }
        else
        {
            app.Logger.LogWarning("Firebase Admin SDK not initialized - authentication will fail");
        }
        return app;
    }

    /// <summary>
    /// Adds global exception handling middleware that returns detailed error information.
    /// </summary>
    public static WebApplication UseGlobalExceptionHandler(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            try
            {
                await next();
            }
            catch (Exception ex)
            {
                app.Logger.LogError(ex, "Unhandled exception for {Method} {Path}",
                    context.Request.Method, context.Request.Path);

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Internal Server Error",
                    message = ex.Message,
                    stackTrace = ex.StackTrace,
                    innerException = ex.InnerException?.Message
                });
            }
        });
        return app;
    }
}
