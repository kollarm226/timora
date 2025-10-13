using Microsoft.OpenApi.Models;

namespace Timora.Api.Extensions;

/// <summary>
/// Extension methods for configuring Swagger/OpenAPI services.
/// </summary>
public static class SwaggerServiceExtensions
{
    /// <summary>
    /// Adds Swagger services with JWT Bearer authentication support.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = "Timora API",
                    Version = "v1",
                    Description =
                        "API for managing employee holiday requests, notices, and company administration",
                }
            );

            // Add JWT Bearer authentication to Swagger
            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description =
                        "Enter your Firebase ID token. Example: 'eyJhbGciOiJSUzI1NiIsImtpZCI6...'",
                }
            );

            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                        },
                        Array.Empty<string>()
                    },
                }
            );
        });

        return services;
    }

    /// <summary>
    /// Configures Swagger UI middleware with root redirect.
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <returns>The application builder for chaining</returns>
    public static IApplicationBuilder UseSwaggerServices(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Timora API v1");
            options.RoutePrefix = "swagger";
        });

        // Redirect root to Swagger UI
        app.Use(
            async (context, next) =>
            {
                if (context.Request.Path == "/")
                {
                    context.Response.Redirect("/swagger");
                    return;
                }
                await next();
            }
        );

        return app;
    }
}
