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

                    // Find matching user in the database by Firebase UID
                    try
                    {
                        var user = await dbContext
                           .Users
                           .AsNoTracking()
                           .Include(u => u.Company)
                            .FirstOrDefaultAsync(u => u.FirebaseId == firebaseUser.Uid);

                       if (user == null && !string.IsNullOrEmpty(firebaseUser.Email))
                       {
                           _logger.LogWarning(
                               "User not found by FirebaseId {FirebaseUid}, trying fallback by email {Email}",
                               firebaseUser.Uid,
                               firebaseUser.Email
                           );

                           user = await dbContext
                               .Users
                               .Include(u => u.Company)
                               .FirstOrDefaultAsync(u => u.Email == firebaseUser.Email);

                           // Handle the case where user exists but may have NULL or different FirebaseId
                           if (user != null)
                           {
                               // Case 1: User has NULL FirebaseId
                               if (string.IsNullOrEmpty(user.FirebaseId))
                               {
                                   _logger.LogInformation(
                                       "Found user by email with NULL FirebaseId. Updating for UserId={UserId}, Email={Email}",
                                       user.Id,
                                       user.Email
                                   );

                                   user.FirebaseId = firebaseUser.Uid;
                                   await dbContext.SaveChangesAsync();

                                   _logger.LogInformation(
                                       "Successfully updated FirebaseId for UserId={UserId}, Email={Email}, NewFirebaseUid={FirebaseUid}",
                                       user.Id,
                                       user.Email,
                                       firebaseUser.Uid
                                   );
                               }
                               // Case 2: User has different FirebaseId
                               else if (user.FirebaseId != firebaseUser.Uid)
                               {
                                   _logger.LogWarning(
                                       "SECURITY: User {UserId} ({Email}) has mismatched FirebaseId. " +
                                       "DB FirebaseId={DBFirebaseId}, Current Token UID={FirebaseUid}. " +
                                       "NOT updating (user may have multiple Firebase accounts). Rejecting login.",
                                       user.Id,
                                       user.Email,
                                       user.FirebaseId,
                                       firebaseUser.Uid
                                   );

                                   user = null;
                               }
                               else
                               {
                                   // Case 3: FirebaseIds match
                                   _logger.LogInformation(
                                       "User found by email with matching FirebaseId. UserId={UserId}, Email={Email}",
                                       user.Id,
                                       user.Email
                                   );
                               }
                           }

                           // If user was found and potentially updated, re-query for claims
                           if (user != null)
                           {
                               user = await dbContext
                                   .Users
                                   .AsNoTracking()
                                   .Include(u => u.Company)
                                   .FirstOrDefaultAsync(u => u.Id == user.Id);
                           }
                       }

                       // Debug: If user still not found, log all users with this email
                       if (user == null && !string.IsNullOrEmpty(firebaseUser.Email))
                       {
                           _logger.LogWarning(
                               "User not found by either FirebaseId or Email. Firebase UID: {FirebaseUid}, Email: {Email}. " +
                               "Checking if user exists in database at all...",
                               firebaseUser.Uid,
                               firebaseUser.Email
                           );

                           // Debug: Check all users with this email
                           var allUsersWithEmail = await dbContext.Users
                               .Where(u => u.Email == firebaseUser.Email)
                               .Select(u => new { u.Id, u.Email, u.FirebaseId })
                               .ToListAsync();

                           _logger.LogWarning(
                               "Database check result - Users with email {Email}: {Count} users found. Details: {Details}",
                               firebaseUser.Email,
                               allUsersWithEmail.Count,
                               string.Join("; ", allUsersWithEmail.Select(u => $"Id={u.Id}, FirebaseId={u.FirebaseId ?? "NULL"}"))
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
