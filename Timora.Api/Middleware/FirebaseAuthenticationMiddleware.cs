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
                   // Log Firebase user info for debugging
                   _logger.LogInformation(
                       "Processing Firebase token for UID: {FirebaseUid}, Email: {Email}",
                       firebaseUser.Uid,
                       firebaseUser.Email ?? "N/A"
                   );

                    // Find matching user in the database - email is primary lookup (guaranteed unique in Firebase)
                    try
                    {
                        // Primary lookup: Email (source of truth from Firebase token)
                        var user = !string.IsNullOrEmpty(firebaseUser.Email)
                            ? await dbContext
                                .Users
                                .AsNoTracking()
                                .Include(u => u.Company)
                                .FirstOrDefaultAsync(u => u.Email == firebaseUser.Email)
                            : null;

                        // Secondary lookup: FirebaseId (fallback if email not found)
                        if (user == null)
                        {
                            _logger.LogWarning(
                                "User not found by Email {Email}, trying fallback by FirebaseId {FirebaseUid}",
                                firebaseUser.Email ?? "N/A",
                                firebaseUser.Uid
                            );

                            user = await dbContext
                                .Users
                                .Include(u => u.Company)
                                .FirstOrDefaultAsync(u => u.FirebaseId == firebaseUser.Uid);

                            // If found by FirebaseId, update email if it changed in Firebase
                            if (user != null && !string.IsNullOrEmpty(firebaseUser.Email) && user.Email != firebaseUser.Email)
                            {
                                _logger.LogInformation(
                                    "User found by FirebaseId but email changed. UserId={UserId}, OldEmail={OldEmail}, NewEmail={NewEmail}",
                                    user.Id,
                                    user.Email,
                                    firebaseUser.Email
                                );
                                // Note: We don't auto-update email here as it could cause conflicts
                            }
                        }
                        // Handle the case where user was found by email but may have NULL or different FirebaseId
                        else if (user != null)
                        {
                            // Case 1: User has NULL FirebaseId - update it
                            if (string.IsNullOrEmpty(user.FirebaseId))
                            {
                                _logger.LogInformation(
                                    "Found user by email with NULL FirebaseId. Updating for UserId={UserId}, Email={Email}",
                                    user.Id,
                                    user.Email
                                );

                                // Need to re-fetch as tracked entity to update
                                var trackedUser = await dbContext.Users.FindAsync(user.Id);
                                if (trackedUser != null)
                                {
                                    trackedUser.FirebaseId = firebaseUser.Uid;
                                    await dbContext.SaveChangesAsync();
                                    user = await dbContext.Users
                                        .AsNoTracking()
                                        .Include(u => u.Company)
                                        .FirstOrDefaultAsync(u => u.Id == user.Id);
                                }

                                _logger.LogInformation(
                                    "Successfully updated FirebaseId for UserId={UserId}, Email={Email}, NewFirebaseUid={FirebaseUid}",
                                    user?.Id,
                                    user?.Email,
                                    firebaseUser.Uid
                                );
                            }
                            // Case 2: User has different FirebaseId
                            else if (user.FirebaseId != firebaseUser.Uid)
                            {
                                _logger.LogWarning(
                                    "SECURITY: User {UserId} ({Email}) has mismatched FirebaseId. " +
                                    "DB FirebaseId={DBFirebaseId}, Current Token UID={FirebaseUid}. " +
                                    "Updating to new FirebaseId (user re-authenticated with same email).",
                                    user.Id,
                                    user.Email,
                                    user.FirebaseId,
                                    firebaseUser.Uid
                                );

                                // Update FirebaseId since email is the source of truth
                                var trackedUser = await dbContext.Users.FindAsync(user.Id);
                                if (trackedUser != null)
                                {
                                    trackedUser.FirebaseId = firebaseUser.Uid;
                                    await dbContext.SaveChangesAsync();
                                    user = await dbContext.Users
                                        .AsNoTracking()
                                        .Include(u => u.Company)
                                        .FirstOrDefaultAsync(u => u.Id == user.Id);
                                }
                            }
                            // Case 3: FirebaseIds match - all good
                            else
                            {
                                _logger.LogInformation(
                                    "User found by email with matching FirebaseId. UserId={UserId}, Email={Email}",
                                    user.Id,
                                    user.Email
                                );
                            }
                        }

                        // Debug: If user still not found, log diagnostic info
                        if (user == null && !string.IsNullOrEmpty(firebaseUser.Email))
                        {
                            _logger.LogWarning(
                                "User not found by either Email or FirebaseId. Firebase UID: {FirebaseUid}, Email: {Email}. " +
                                "User needs to complete registration.",
                                firebaseUser.Uid,
                                firebaseUser.Email
                            );
                        }

                        _logger.LogInformation(
                            "Database lookup result: Found={Found}, UserId={UserId}, Email={Email}",
                            user != null,
                            user?.Id ?? 0,
                            user?.Email ?? "N/A"
                        );

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
                    }
                }
            }

            await _next(context);
        }
    }
}
