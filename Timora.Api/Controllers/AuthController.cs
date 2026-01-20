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
    /// Includes fallback logic to retrieve user data from database if claims are incomplete.
    /// </summary>
    /// <returns>User data extracted from the Firebase authentication token and/or database.</returns>
    /// <response code="200">Returns the authenticated user's claims and profile data.</response>
    /// <response code="401">If the user is not authenticated or token is invalid.</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var userName = User.FindFirst(ClaimTypes.Name)?.Value;
        var firstName = User.FindFirst(ClaimTypes.GivenName)?.Value;
        var lastName = User.FindFirst(ClaimTypes.Surname)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        var companyId = User.FindFirst("CompanyId")?.Value;
        var firebaseUid = User.FindFirst("FirebaseUid")?.Value;

        _logger.LogInformation(
            "User accessed /api/auth/me endpoint. UserId={UserId}, Email={Email}, FirebaseUid={FirebaseUid}",
            userId ?? "MISSING",
            email ?? "MISSING",
            firebaseUid ?? "MISSING"
        );

        // FALLBACK: If critical profile data is missing from claims, try to retrieve from database
        // This handles cases where middleware couldn't find user or user has incomplete profile data
        bool profileIncomplete = string.IsNullOrEmpty(userId) 
                              || string.IsNullOrEmpty(firstName) 
                              || string.IsNullOrEmpty(lastName);

        if (profileIncomplete)
        {
            User? userFromDb = null;

            // Primary lookup: Use Email
            if (!string.IsNullOrEmpty(email))
            {
                _logger.LogWarning(
                    "GetCurrentUser: Incomplete profile data in claims (UserId={UserId}, FirstName={FirstName}, LastName={LastName}), attempting fallback lookup by Email {Email}",
                    userId ?? "MISSING",
                    firstName ?? "MISSING",
                    lastName ?? "MISSING",
                    email
                );

                userFromDb = await _userService.GetUserByEmailAsync(email);
            }

            // Secondary lookup: Fall back to FirebaseUid if email lookup failed
            if (userFromDb == null && !string.IsNullOrEmpty(firebaseUid))
            {
                _logger.LogWarning(
                    "GetCurrentUser: Email lookup failed or unavailable, attempting fallback lookup by FirebaseUid {FirebaseUid}",
                    firebaseUid
                );

                userFromDb = await _userService.GetUserByFirebaseIdAsync(firebaseUid);
            }

            if (userFromDb != null)
            {
                userId = userFromDb.Id.ToString();
                userName = userFromDb.UserName;
                firstName = userFromDb.FirstName;
                lastName = userFromDb.LastName;
                role = userFromDb.Role.ToString();
                companyId = userFromDb.CompanyId.ToString();

                _logger.LogInformation(
                    "Fallback: Successfully retrieved user data from database. UserId={UserId}, Email={Email}",
                    userId,
                    email
                );
            }
            else
            {
                _logger.LogWarning(
                    "Fallback: User not found in database despite Firebase authentication. FirebaseUid={FirebaseUid}, Email={Email}",
                    firebaseUid ?? "MISSING",
                    email ?? "MISSING"
                );
            }
        }

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
    /// Validates login credentials for a Firebase-authenticated user.
    /// Ensures the user exists in the database and is assigned to a company.
    /// </summary>
    /// <param name="loginDto">The login data.</param>
    /// <returns>The authenticated user's profile with company information.</returns>
    /// <response code="200">Returns the authenticated user with their company details.</response>
    /// <response code="400">If user is not assigned to any company.</response>
    /// <response code="401">If the user is not authenticated with Firebase.</response>
    /// <response code="404">If user is not found in database.</response>
    [HttpPost("login")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var firebaseUid = User.FindFirst("FirebaseUid")?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(firebaseUid) && string.IsNullOrEmpty(email))
        {
            _logger.LogWarning("Login attempt without proper Firebase authentication");
            return Unauthorized(new { message = "Invalid authentication. Firebase UID or email not found in token." });
        }

        // Primary lookup: Email (source of truth from Firebase token)
        var user = !string.IsNullOrEmpty(email)
            ? await _userService.GetUserByEmailAsync(email)
            : null;

        // Secondary lookup: FirebaseUid (fallback if email lookup failed)
        if (user == null && !string.IsNullOrEmpty(firebaseUid))
        {
            user = await _userService.GetUserByFirebaseIdAsync(firebaseUid);
        }

        if (user == null)
        {
            _logger.LogWarning("User with Email {Email} / FirebaseUid {FirebaseUid} not found in database. User needs to register first.", email ?? "N/A", firebaseUid ?? "N/A");
            return NotFound(new { message = "User not found. Please register first." });
        }

        // Ensure user is assigned to a company
        if (user.CompanyId <= 0)
        {
            _logger.LogWarning(
                "User {FirebaseUid} attempted to login but is not assigned to any company.",
                firebaseUid
            );
            return BadRequest(new { message = "User is not assigned to any company." });
        }

        // Check if user is approved
        if (!user.IsApproved)
        {
            _logger.LogWarning(
                "User {UserId} ({Email}) attempted to login but is pending approval.",
                user.Id,
                user.Email
            );
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "Your account is pending approval." });
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
            CompanyName = user.Company?.Name,
            IsApproved = user.IsApproved
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
        // Extract Firebase UID from claims (set by middleware)
        var firebaseUid = User.FindFirst("FirebaseUid")?.Value;

        if (string.IsNullOrEmpty(firebaseUid))
        {
            _logger.LogWarning("Registration attempt without Firebase UID in claims");
            return Unauthorized(new { message = "Invalid authentication. Firebase UID not found in token." });
        }

        // Check if user already exists by Firebase UID
        var existingUser = await _userService.GetUserByFirebaseIdAsync(firebaseUid);
        if (existingUser != null)
        {
            _logger.LogWarning("User with Firebase UID {FirebaseUid} attempted to register but already exists", firebaseUid);
            return Conflict(new { message = "User is already registered.", userId = existingUser.Id });
        }

        // Check if email from form already exists in database
        var userByEmail = await _userService.GetUserByEmailAsync(registerDto.Email);
        if (userByEmail != null)
        {
            _logger.LogWarning("Registration attempt with already registered email {Email}", registerDto.Email);
            return Conflict(new { message = "Email is already registered." });
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
                "User {FirebaseUid} joining existing company '{CompanyName}' (ID: {CompanyId}) - pending approval",
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

        // Determine if user needs approval (employees joining existing company need approval)
        var isApproved = !string.IsNullOrWhiteSpace(registerDto.CompanyName); // Creating company = auto-approved

        // Create the user
        var user = new User
        {
            FirebaseId = firebaseUid,
            Email = registerDto.Email.Trim(),
            FirstName = registerDto.FirstName.Trim(),
            LastName = registerDto.LastName.Trim(),
            UserName = registerDto.UserName.Trim(),
            CompanyId = companyId,
            Role = role,
            CreatedAt = DateTime.UtcNow,
            IsApproved = isApproved,
            ApprovedAt = isApproved ? DateTime.UtcNow : null,
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
                    IsApproved = createdUser.IsApproved,
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
