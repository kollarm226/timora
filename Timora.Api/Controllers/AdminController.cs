using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Timora.Api.Services;
using Timora.Data.Entities;

namespace Timora.Api.Controllers;

/// <summary>
/// Controller for administrative operations.
/// Provides endpoints for user approval management (Employer role only).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class AdminController : ControllerBase
{
    private readonly ILogger<AdminController> _logger;
    private readonly IUserService _userService;
    private readonly IEmailService _emailService;

    /// <summary>
    /// Initializes a new instance of the AdminController.
    /// </summary>
    /// <param name="logger">Logger for diagnostic information.</param>
    /// <param name="userService">The user service.</param>
    /// <param name="emailService">The email service.</param>
    public AdminController(
        ILogger<AdminController> logger,
        IUserService userService,
        IEmailService emailService
    )
    {
        _logger = logger;
        _userService = userService;
        _emailService = emailService;
    }

    /// <summary>
    /// Gets all users pending approval for the current employer's company.
    /// </summary>
    /// <returns>A list of users pending approval.</returns>
    /// <response code="200">Returns the list of pending users.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="403">If the user is not an employer.</response>
    [HttpGet("pending-users")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetPendingUsers()
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
        {
            return Unauthorized(new { message = "User not found." });
        }

        if (currentUser.Role != UserRole.Employer && currentUser.Role != UserRole.Admin)
        {
            _logger.LogWarning("User {UserId} attempted to access pending users without employer role.", currentUser.Id);
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only employers can view pending users." });
        }

        var pendingUsers = await _userService.GetPendingUsersAsync(currentUser.CompanyId);

        _logger.LogInformation(
            "Employer {UserId} retrieved {Count} pending users for company {CompanyId}",
            currentUser.Id,
            pendingUsers.Count(),
            currentUser.CompanyId
        );

        return Ok(pendingUsers.Select(u => new
        {
            u.Id,
            u.Email,
            u.UserName,
            u.FirstName,
            u.LastName,
            Role = u.Role.ToString(),
            u.CompanyId,
            u.CreatedAt,
            u.IsApproved
        }));
    }

    /// <summary>
    /// Approves a user's registration request.
    /// </summary>
    /// <param name="userId">The ID of the user to approve.</param>
    /// <returns>The approved user.</returns>
    /// <response code="200">Returns the approved user.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="403">If the user is not an employer or user is from different company.</response>
    /// <response code="404">If the user to approve is not found.</response>
    [HttpPost("approve-user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveUser(int userId)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
        {
            return Unauthorized(new { message = "User not found." });
        }

        if (currentUser.Role != UserRole.Employer && currentUser.Role != UserRole.Admin)
        {
            _logger.LogWarning("User {UserId} attempted to approve a user without employer role.", currentUser.Id);
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only employers can approve users." });
        }

        var userToApprove = await _userService.GetUserByIdAsync(userId);
        if (userToApprove == null)
        {
            _logger.LogWarning("Employer {EmployerId} tried to approve non-existent user {UserId}", currentUser.Id, userId);
            return NotFound(new { message = $"User with ID {userId} not found." });
        }

        // Ensure user belongs to the same company
        if (userToApprove.CompanyId != currentUser.CompanyId)
        {
            _logger.LogWarning(
                "Employer {EmployerId} from company {EmployerCompanyId} tried to approve user {UserId} from different company {UserCompanyId}",
                currentUser.Id,
                currentUser.CompanyId,
                userId,
                userToApprove.CompanyId
            );
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "Cannot approve users from a different company." });
        }

        if (userToApprove.IsApproved)
        {
            return Ok(new { message = "User is already approved.", userId = userToApprove.Id });
        }

        var approvedUser = await _userService.ApproveUserAsync(userId, currentUser.Id);
        if (approvedUser == null)
        {
            return NotFound(new { message = $"Failed to approve user with ID {userId}." });
        }

        _logger.LogInformation(
            "Employer {EmployerId} approved user {UserId} ({Email})",
            currentUser.Id,
            approvedUser.Id,
            approvedUser.Email
        );

        // Send approval notification email
        var companyName = approvedUser.Company?.Name ?? "your company";
        await _emailService.SendUserApprovalEmailAsync(
            approvedUser.Email,
            approvedUser.FullName,
            companyName
        );

        return Ok(new
        {
            approvedUser.Id,
            approvedUser.Email,
            approvedUser.UserName,
            approvedUser.FirstName,
            approvedUser.LastName,
            Role = approvedUser.Role.ToString(),
            approvedUser.CompanyId,
            approvedUser.IsApproved,
            approvedUser.ApprovedBy,
            approvedUser.ApprovedAt
        });
    }

    /// <summary>
    /// Rejects and deletes a user's registration request.
    /// </summary>
    /// <param name="userId">The ID of the user to reject.</param>
    /// <returns>No content if successful.</returns>
    /// <response code="204">If the user was successfully rejected and deleted.</response>
    /// <response code="401">If the user is not authenticated.</response>
    /// <response code="403">If the user is not an employer or user is from different company.</response>
    /// <response code="404">If the user to reject is not found.</response>
    [HttpPost("reject-user/{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RejectUser(int userId)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
        {
            return Unauthorized(new { message = "User not found." });
        }

        if (currentUser.Role != UserRole.Employer && currentUser.Role != UserRole.Admin)
        {
            _logger.LogWarning("User {UserId} attempted to reject a user without employer role.", currentUser.Id);
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only employers can reject users." });
        }

        var userToReject = await _userService.GetUserByIdAsync(userId);
        if (userToReject == null)
        {
            _logger.LogWarning("Employer {EmployerId} tried to reject non-existent user {UserId}", currentUser.Id, userId);
            return NotFound(new { message = $"User with ID {userId} not found." });
        }

        // Ensure user belongs to the same company
        if (userToReject.CompanyId != currentUser.CompanyId)
        {
            _logger.LogWarning(
                "Employer {EmployerId} from company {EmployerCompanyId} tried to reject user {UserId} from different company {UserCompanyId}",
                currentUser.Id,
                currentUser.CompanyId,
                userId,
                userToReject.CompanyId
            );
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "Cannot reject users from a different company." });
        }

        var deleted = await _userService.DeleteUserAsync(userId);
        if (!deleted)
        {
            return NotFound(new { message = $"Failed to reject user with ID {userId}." });
        }

        _logger.LogInformation(
            "Employer {EmployerId} rejected and deleted user {UserId} ({Email})",
            currentUser.Id,
            userToReject.Id,
            userToReject.Email
        );

        return NoContent();
    }

    /// <summary>
    /// Gets the current authenticated user from claims.
    /// </summary>
    private async Task<User?> GetCurrentUserAsync()
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var firebaseUid = User.FindFirst("FirebaseUid")?.Value;

        if (!string.IsNullOrEmpty(email))
        {
            return await _userService.GetUserByEmailAsync(email);
        }

        if (!string.IsNullOrEmpty(firebaseUid))
        {
            return await _userService.GetUserByFirebaseIdAsync(firebaseUid);
        }

        return null;
    }
}
