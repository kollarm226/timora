using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Timora.Api.Controllers;

/// <summary>
/// Controller for authentication-related endpoints.
/// Provides user information retrieval and Firebase token validation testing.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Initializes a new instance of the AuthController.
    /// </summary>
    /// <param name="logger">Logger for diagnostic information.</param>
    public AuthController(ILogger<AuthController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Gets the current authenticated user's information from claims.
    /// </summary>
    /// <returns>User data extracted from the Firebase authentication token.</returns>
    /// <response code="200">Returns the authenticated user's claims and profile data.</response>
    /// <response code="401">If the user is not authenticated or token is invalid.</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var userName = User.FindFirst(ClaimTypes.Name)?.Value;
        var firstName = User.FindFirst(ClaimTypes.GivenName)?.Value;
        var lastName = User.FindFirst(ClaimTypes.Surname)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        var companyId = User.FindFirst("CompanyId")?.Value;
        var firebaseUid = User.FindFirst("FirebaseUid")?.Value;

        _logger.LogInformation("User {UserId} accessed /api/auth/me endpoint", userId);

        return Ok(
            new
            {
                UserId = userId,
                Email = email,
                UserName = userName,
                FirstName = firstName,
                LastName = lastName,
                Role = role,
                CompanyId = companyId,
                FirebaseUid = firebaseUid,
                AllClaims = User.Claims.Select(c => new { c.Type, c.Value }),
            }
        );
    }
}
