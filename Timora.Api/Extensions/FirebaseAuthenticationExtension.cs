using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Timora.Api.Middleware;
using Timora.Api.Services;

namespace Timora.Api.Extensions;

/// <summary>
/// Extension methods for configuring Firebase Authentication services.
/// </summary>
public static class FirebaseAuthenticationExtensions
{
    /// <summary>
    /// Adds Firebase Authentication with JWT Bearer token validation.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The application configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddFirebaseAuthentication(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var projectId = configuration["Firebase:ProjectId"];

        if (string.IsNullOrEmpty(projectId))
        {
            throw new InvalidOperationException(
                "Firebase:ProjectId not configured in appsettings.json"
            );
        }

        // Initialize Firebase Admin SDK only if not already initialized
        if (FirebaseApp.DefaultInstance == null)
        {
            var credentialsPath = Environment.GetEnvironmentVariable(
                "GOOGLE_APPLICATION_CREDENTIALS"
            );

            // Check if credentials exist (skip during design-time EF operations)
            if (!string.IsNullOrEmpty(credentialsPath) && File.Exists(credentialsPath))
            {
                try
                {
                    FirebaseApp.Create(
                        new AppOptions
                        {
                            Credential = GoogleCredential.FromFile(credentialsPath),
                            ProjectId = projectId,
                        }
                    );
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Failed to initialize Firebase Admin SDK. Check credentials file at: {credentialsPath}",
                        ex
                    );
                }
            }
            else if (!string.IsNullOrEmpty(credentialsPath))
            {
                // Credentials path set but file doesn't exist
                throw new FileNotFoundException(
                    $"Firebase credentials file not found at: {credentialsPath}"
                );
            }
            // else: No credentials path set - likely design-time operation, skip initialization
        }

        // Configure JWT Bearer Authentication
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = $"https://securetoken.google.com/{projectId}";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = $"https://securetoken.google.com/{projectId}",
                    ValidateAudience = true,
                    ValidAudience = projectId,
                    ValidateLifetime = true,
                };
            });

        services.AddAuthorization();
        services.AddScoped<FirebaseAuthService>();

        return services;
    }

    /// <summary>
    /// Adds Firebase authentication middleware to the application pipeline.
    /// Must be called after UseAuthentication() and before UseAuthorization().
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <returns>The application builder for chaining</returns>
    public static IApplicationBuilder UseFirebaseAuthentication(this IApplicationBuilder app)
    {
        return app.UseMiddleware<FirebaseAuthenticationMiddleware>();
    }
}
