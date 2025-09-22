using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Timora.Api.Extensions;

public static class SwaggerServiceExtension
{
    // Register Swagger generator
    public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = "Timora API",
                    Version = "v1",
                    Description = "Timora API documentation",
                }
            );
        });

        return services;
    }

    // Add middleware to the pipeline (call only in Development)
    public static WebApplication UseSwaggerServices(this WebApplication app)
    {
        app.UseSwagger(); // serves /swagger/v1/swagger.json
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Timora API v1");
            c.RoutePrefix = "swagger"; // UI at /swagger
        });

        // Redirect root to Swagger UI
        app.MapGet("/", () => Results.Redirect("/swagger"));

        return app;
    }
}
