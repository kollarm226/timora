using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Timora.Api.DTOs;
using Timora.Api.Services;
using Timora.Data.Entities;

namespace Timora.Api.Controllers;

/// <summary>
/// Controller for authentication-related endpoints.
/// Provides user information retrieval, registration, and Firebase token validation.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly IUserService _userService;
    private readonly ICompanyService _companyService;

    /// <summary>
    /// Initializes a new instance of the AuthController.
    /// </summary>
    /// <param name="logger">Logger for diagnostic information.</param>
    /// <param name="userService">The user service.</param>
    /// <param name="companyService">The company service.</param>
    public AuthController(
        ILogger<AuthController> logger,
        IUserService userService,
        ICompanyService companyService
    )
    {
        _logger = logger;
        _userService = userService;
        _companyService = companyService;
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

    /// <summary>
    /// Validates login credentials against a specific company.
    /// User must be authenticated via Firebase AND must belong to the specified company.
    /// This endpoint ensures that a user cannot login with a wrong company ID.
    /// </summary>
    /// <param name="loginDto">The login data with companyId validation.</param>
    /// <returns>The authenticated user's profile with correct company information.</returns>
    /// <response code="200">Returns the authenticated user with their correct company details.</response>
    /// <response code="400">If user is not in the specified company.</response>
    /// <response code="401">If the user is not authenticated with Firebase.</response>
    /// <response code="404">If user is not found in database.</response>
    [HttpPost("login")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var firebaseUid = User.FindFirst("FirebaseUid")?.Value;
        var userCompanyIdClaim = User.FindFirst("CompanyId")?.Value;

        if (string.IsNullOrEmpty(firebaseUid))
        {
            _logger.LogWarning("Login attempt without proper Firebase authentication");
            return Unauthorized(new { message = "Invalid authentication. Firebase UID not found in token." });
        }

        // Parse the company ID from claims
        if (!int.TryParse(userCompanyIdClaim, out int userActualCompanyId))
        {
            _logger.LogWarning("Invalid CompanyId claim for user {FirebaseUid}", firebaseUid);
            return BadRequest(new { message = "Invalid company information in user profile." });
        }

        // CRITICAL: Check if the company ID from the request matches the user's actual company
        if (loginDto.CompanyId != userActualCompanyId)
        {
            _logger.LogWarning(
                "User {FirebaseUid} attempted to login with wrong company. Claimed: {ClaimedCompanyId}, Actual: {ActualCompanyId}",
                firebaseUid,
                loginDto.CompanyId,
                userActualCompanyId
            );
            return BadRequest(new { message = $"Invalid company ID. You are registered in company {userActualCompanyId}, not company {loginDto.CompanyId}." });
        }

        // Fetch full user data from database
        var user = await _userService.GetUserByFirebaseIdAsync(firebaseUid);
        if (user == null)
        {
            _logger.LogWarning("User with Firebase UID {FirebaseUid} not found in database after successful login validation", firebaseUid);
            return NotFound(new { message = "User not found." });
        }

        _logger.LogInformation(
            "User {UserId} ({Email}) successfully logged in to company {CompanyId}",
            user.Id,
            user.Email,
            user.CompanyId
        );

        return Ok(new
        {
            UserId = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role.ToString(),
            CompanyId = user.CompanyId,
            CompanyName = user.Company?.Name
        });
    }

    /// <summary>
    /// Registers a new user after Firebase authentication.
    /// User can either join an existing company (as Employee) or create a new company (as Employer).
    /// </summary>
    /// <param name="registerDto">The registration data.</param>
    /// <returns>The created user profile.</returns>
    /// <response code="201">Returns the newly created user.</response>
    /// <response code="400">If the registration data is invalid or company not found.</response>
    /// <response code="401">If the user is not authenticated with Firebase.</response>
    /// <response code="409">If the user is already registered.</response>
    [HttpPost("register")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
    {
        // Extract Firebase UID and email from claims (set by middleware)
        var firebaseUid = User.FindFirst("FirebaseUid")?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(firebaseUid))
        {
            _logger.LogWarning("Registration attempt without Firebase UID in claims");
            return Unauthorized(new { message = "Invalid authentication. Firebase UID not found in token." });
        }

        // Check if user already exists
        var existingUser = await _userService.GetUserByFirebaseIdAsync(firebaseUid);
        if (existingUser != null)
        {
            _logger.LogWarning("User with Firebase UID {FirebaseUid} attempted to register but already exists", firebaseUid);
            return Conflict(new { message = "User is already registered.", userId = existingUser.Id });
        }

        int companyId;
        UserRole role;

        // Handle company selection/creation
        if (!string.IsNullOrWhiteSpace(registerDto.CompanyName))
        {
            // Create new company - user becomes Employer (admin)
            var newCompany = new Company
            {
                Name = registerDto.CompanyName.Trim(),
            };

            try
            {
                var createdCompany = await _companyService.CreateCompanyAsync(newCompany);
                companyId = createdCompany.Id;
                role = UserRole.Employer;

                _logger.LogInformation(
                    "Created new company '{CompanyName}' with ID {CompanyId} for user {FirebaseUid}",
                    createdCompany.Name,
                    companyId,
                    firebaseUid
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create company '{CompanyName}' for user {FirebaseUid}", registerDto.CompanyName, firebaseUid);
                return BadRequest(new { message = "Failed to create company.", error = ex.Message });
            }
        }
        else if (registerDto.CompanyId.HasValue)
        {
            // Join existing company - user becomes Employee
            var existingCompany = await _companyService.GetCompanyByIdAsync(registerDto.CompanyId.Value);
            if (existingCompany == null)
            {
                _logger.LogWarning("User {FirebaseUid} tried to join non-existent company {CompanyId}", firebaseUid, registerDto.CompanyId.Value);
                return BadRequest(new { message = $"Company with ID {registerDto.CompanyId.Value} not found." });
            }

            companyId = existingCompany.Id;
            role = UserRole.Employee;

            _logger.LogInformation(
                "User {FirebaseUid} joining existing company '{CompanyName}' (ID: {CompanyId})",
                firebaseUid,
                existingCompany.Name,
                companyId
            );
        }
        else
        {
            // This shouldn't happen due to DTO validation, but handle it anyway
            return BadRequest(new { message = "Either CompanyId or CompanyName must be provided." });
        }

        // Create the user
        var user = new User
        {
            FirebaseId = firebaseUid,
            Email = email ?? $"{firebaseUid}@unknown.com",
            FirstName = registerDto.FirstName.Trim(),
            LastName = registerDto.LastName.Trim(),
            UserName = registerDto.UserName.Trim(),
            CompanyId = companyId,
            Role = role,
            CreatedAt = DateTime.UtcNow,
        };

        try
        {
            var createdUser = await _userService.CreateUserAsync(user);

            _logger.LogInformation(
                "Successfully registered user {UserId} ({Email}) with role {Role} in company {CompanyId}",
                createdUser.Id,
                createdUser.Email,
                role,
                companyId
            );

            return CreatedAtAction(
                nameof(GetCurrentUser),
                new
                {
                    UserId = createdUser.Id,
                    Email = createdUser.Email,
                    UserName = createdUser.UserName,
                    FirstName = createdUser.FirstName,
                    LastName = createdUser.LastName,
                    Role = createdUser.Role.ToString(),
                    CompanyId = createdUser.CompanyId,
                    CompanyName = createdUser.Company?.Name,
                    FirebaseUid = createdUser.FirebaseId,
                }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create user for Firebase UID {FirebaseUid}", firebaseUid);
            return BadRequest(new { message = "Failed to create user.", error = ex.Message });
        }
    }
}
