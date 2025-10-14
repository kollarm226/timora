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
                    var user = await dbContext
                        .Users.Include(u => u.Company)
                        .FirstOrDefaultAsync(u => u.FirebaseId == firebaseUser.Uid);

                    if (user != null)
                    {
                        // Populate ClaimsPrincipal with user data from database (source of truth)
                        var claims = new List<Claim>
                        {
                            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                            new(ClaimTypes.Name, user.UserName),
                            new(ClaimTypes.Email, user.Email),
                            new(ClaimTypes.GivenName, user.FirstName),
                            new(ClaimTypes.Surname, user.LastName),
                            new(ClaimTypes.Role, user.Role.ToString()),
                            new("CompanyId", user.CompanyId.ToString()),
                            new("FirebaseUid", firebaseUser.Uid),
                        };

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
                            "Firebase user {FirebaseUid} authenticated but no matching User entity found in database",
                            firebaseUser.Uid
                        );
                    }
                }
            }

            await _next(context);
        }
    }
}
