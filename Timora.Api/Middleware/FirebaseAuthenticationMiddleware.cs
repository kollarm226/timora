using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Timora.Api.Services;
using Timora.Data.Data;

namespace Timora.Api.Middleware
{
    /// <summary>
    /// Middleware that validates Firebase ID tokens and populates HttpContext with authenticated user claims.
    /// Matches Firebase UID to User.FirebaseId and enriches the ClaimsPrincipal with user data from the database.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class FirebaseAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<FirebaseAuthenticationMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of the FirebaseAuthenticationMiddleware.
        /// </summary>
        /// <param name="next">The next middleware in the request pipeline.</param>
        /// <param name="logger">Logger for diagnostic information.</param>
        public FirebaseAuthenticationMiddleware(
            RequestDelegate next,
            ILogger<FirebaseAuthenticationMiddleware> logger
        )
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(
            HttpContext context,
            FirebaseAuthService firebaseAuthService,
            TimoraDbContext dbContext
        )
        {
            var authHeader = context.Request.Headers.Authorization.FirstOrDefault();

            if (
                !string.IsNullOrEmpty(authHeader)
                && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            )
            {
                var token = authHeader["Bearer ".Length..].Trim();

                var firebaseUser = await firebaseAuthService.ValidateTokenAsync(token);

                if (firebaseUser != null)
                {
                    // Find matching user in the database by Firebase UID
                    try
                    {
                        var user = await dbContext
                            .Users.Include(u => u.Company)
                            .FirstOrDefaultAsync(u => u.FirebaseId == firebaseUser.Uid);

                        if (user != null)
                        {
                            // Populate ClaimsPrincipal with user data from database (source of truth)
                            var claims = new List<Claim>
                            {
                                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                                new("FirebaseUid", firebaseUser.Uid),
                                new("CompanyId", user.CompanyId.ToString()),
                                new(ClaimTypes.Role, user.Role.ToString()),
                            };

                            // Add optional claims only when values are present to avoid ArgumentNullException
                            if (!string.IsNullOrWhiteSpace(user.UserName))
                            {
                                claims.Add(new Claim(ClaimTypes.Name, user.UserName));
                            }
                            if (!string.IsNullOrWhiteSpace(user.Email))
                            {
                                claims.Add(new Claim(ClaimTypes.Email, user.Email));
                            }
                            if (!string.IsNullOrWhiteSpace(user.FirstName))
                            {
                                claims.Add(new Claim(ClaimTypes.GivenName, user.FirstName));
                            }
                            if (!string.IsNullOrWhiteSpace(user.LastName))
                            {
                                claims.Add(new Claim(ClaimTypes.Surname, user.LastName));
                            }

                            var identity = new ClaimsIdentity(claims, "Firebase");
                            context.User = new ClaimsPrincipal(identity);

                            _logger.LogInformation(
                                "Authenticated user {UserId} ({Email}) from Firebase UID {FirebaseUid}",
                                user.Id,
                                user.Email,
                                firebaseUser.Uid
                            );
                        }
                        else
                        {
                            _logger.LogWarning(
                                "Firebase user {FirebaseUid} authenticated but no matching User entity found in database. User needs to complete registration.",
                                firebaseUser.Uid
                            );

                            // Set minimal claims for unregistered Firebase users
                            // This allows them to access the registration endpoint
                            var minimalClaims = new List<Claim>
                            {
                                new("FirebaseUid", firebaseUser.Uid),
                                new(ClaimTypes.Email, firebaseUser.Email ?? string.Empty),
                                new("IsRegistered", "false"),
                            };

                            var minimalIdentity = new ClaimsIdentity(minimalClaims, "Firebase");
                            context.User = new ClaimsPrincipal(minimalIdentity);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to enrich user from DB for Firebase UID {FirebaseUid}. Continuing without DB enrichment.", firebaseUser.Uid);
                        // Leave context.User as set by JWT Bearer if present
                    }
                }
            }

            await _next(context);
        }
    }
}
