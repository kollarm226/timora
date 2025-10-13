using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Timora.Api.Extensions;

/// <summary>
/// Extension methods for configuring Firebase Authentication services.
/// </summary>
public static class FirebaseAuthenticationExtension
{
    /// <summary>
    /// Adds Firebase Authentication with JWT Bearer token validation.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The application configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddFirebaseAuthentication(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var projectId =
            configuration["Firebase:ProjectId"]
            ?? throw new InvalidOperationException("Firebase ProjectId not configured");

        // Initialize Firebase Admin SDK
        FirebaseApp.Create(
            new AppOptions
            {
                Credential = GoogleCredential.GetApplicationDefault(),
                ProjectId = projectId,
            }
        );

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

        return services;
    }
}
