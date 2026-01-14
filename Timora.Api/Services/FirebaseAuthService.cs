using System.Diagnostics.CodeAnalysis;
using FirebaseAdmin.Auth;
using Timora.Data.Models;

namespace Timora.Api.Services;

/// <summary>
/// Service for handling Firebase Authentication operations including token validation and user retrieval.
/// </summary>
[ExcludeFromCodeCoverage]
public class FirebaseAuthService
{
    private readonly ILogger<FirebaseAuthService> _logger;

    /// <summary>
    /// Initializes a new instance of the FirebaseAuthService.
    /// </summary>
    /// <param name="logger">The logger instance for diagnostic information.</param>
    public FirebaseAuthService(ILogger<FirebaseAuthService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Validates a Firebase ID token and extracts user claims.
    /// </summary>
    /// <param name="idToken">The Firebase ID token from the Authorization header.</param>
    /// <returns>Firebase user claims if valid, null otherwise.</returns>
    public async Task<FirebaseUserClaims?> ValidateTokenAsync(string idToken)
    {
        try
        {
            // Check if Firebase is initialized
            if (FirebaseAuth.DefaultInstance == null)
            {
                _logger.LogError(
                    "Firebase Admin SDK not initialized. Check GOOGLE_APPLICATION_CREDENTIALS environment variable."
                );
                return null;
            }

            var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);

            return new FirebaseUserClaims
            {
                Uid = decodedToken.Uid,
                Email = decodedToken.Claims.TryGetValue("email", out var email)
                    ? email.ToString()
                    : null,
                EmailVerified =
                    decodedToken.Claims.TryGetValue("email_verified", out var verified)
                    && Convert.ToBoolean(verified),
            };
        }
        catch (FirebaseAuthException ex)
        {
            _logger.LogWarning(ex, "Firebase token validation failed: {Message}", ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during Firebase token validation");
            return null;
        }
    }

    /// <summary>
    /// Gets a Firebase user by their UID.
    /// </summary>
    /// <param name="uid">The Firebase user UID.</param>
    /// <returns>User record from Firebase, or null if not found.</returns>
    public async Task<UserRecord?> GetUserByUidAsync(string uid)
    {
        try
        {
            if (FirebaseAuth.DefaultInstance == null)
            {
                _logger.LogError("Firebase Admin SDK not initialized. Cannot retrieve user.");
                return null;
            }

            return await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
        }
        catch (FirebaseAuthException ex)
        {
            _logger.LogWarning(
                ex,
                "Failed to retrieve Firebase user {Uid}: {Message}",
                uid,
                ex.Message
            );
            return null;
        }
    }
}
