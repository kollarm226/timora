using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Timora.Api.DTOs;
using Timora.Api.Services;
using Timora.Data.Entities;

namespace Timora.Api.Controllers
{
    /// <summary>
    /// Controller for managing user operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        /// <summary>
        /// Initializes a new instance of the UsersController.
        /// </summary>
        /// <param name="userService">The user service.</param>
        /// <param name="logger">The logger instance.</param>
        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all users in the system.
        /// </summary>
        /// <returns>A list of all users.</returns>
        /// <response code="200">Returns the list of users.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllUsers()
        {
            _logger.LogInformation("Fetching all users");
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// Retrieves a specific user by their ID.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <returns>The requested user.</returns>
        /// <response code="200">Returns the requested user.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="404">If the user is not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById(int id)
        {
            _logger.LogInformation("Fetching user with ID: {UserId}", id);
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", id);
                return NotFound(new { message = $"User with ID {id} not found" });
            }

            return Ok(user);
        }

        /// <summary>
        /// Retrieves a specific user by their email address.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <returns>The requested user.</returns>
        /// <response code="200">Returns the requested user.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="404">If the user is not found.</response>
        [HttpGet("by-email/{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            _logger.LogInformation("Fetching user with email: {Email}", email);
            var user = await _userService.GetUserByEmailAsync(email);

            if (user == null)
            {
                _logger.LogWarning("User with email {Email} not found", email);
                return NotFound(new { message = $"User with email {email} not found" });
            }

            return Ok(user);
        }

        /// <summary>
        /// Creates a new user in the system.
        /// </summary>
        /// <param name="createUserDto">The user data to create.</param>
        /// <returns>The created user.</returns>
        /// <response code="201">Returns the newly created user.</response>
        /// <response code="400">If the user data is invalid.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            _logger.LogInformation("Creating new user with email: {Email}", createUserDto.Email);

            // Map DTO to entity
            var user = new User
            {
                FirebaseId = createUserDto.FirebaseId,
                CompanyId = createUserDto.CompanyId,
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                Email = createUserDto.Email,
                UserName = createUserDto.UserName,
                Role = createUserDto.Role,
                CreatedAt = DateTime.UtcNow,
            };

            try
            {
                var createdUser = await _userService.CreateUserAsync(user);
                _logger.LogInformation(
                    "User created successfully with ID: {UserId}",
                    createdUser.Id
                );

                return CreatedAtAction(
                    nameof(GetUserById),
                    new { id = createdUser.Id },
                    createdUser
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error creating user with email: {Email}",
                    createUserDto.Email
                );
                return BadRequest(new { message = "Failed to create user", error = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a user from the system by their ID.
        /// </summary>
        /// <param name="id">The unique identifier of the user to delete.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">If the user was successfully deleted.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="404">If the user is not found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            _logger.LogInformation("Attempting to delete user with ID: {UserId}", id);

            var result = await _userService.DeleteUserAsync(id);

            if (!result)
            {
                _logger.LogWarning("User with ID {UserId} not found for deletion", id);
                return NotFound(new { message = $"User with ID {id} not found" });
            }

            _logger.LogInformation("User with ID {UserId} deleted successfully", id);
            return NoContent();
        }

        /// <summary>
        /// Partially updates an existing user by their ID.
        /// </summary>
        /// <param name="id">The unique identifier of the user to update.</param>
        /// <param name="updateUserDto">The user data to update.</param>
        /// <returns>The updated user.</returns>
        /// <response code="200">Returns the updated user.</response>
        /// <response code="400">If the update data is invalid.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="404">If the user is not found.</response>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            _logger.LogInformation("Updating user with ID: {UserId}", id);

            // Map DTO to entity for partial update
            var user = new User
            {
                CompanyId = updateUserDto.CompanyId ?? 0,
                FirstName = updateUserDto.FirstName ?? string.Empty,
                LastName = updateUserDto.LastName ?? string.Empty,
                Email = updateUserDto.Email ?? string.Empty,
                UserName = updateUserDto.UserName ?? string.Empty,
                Role = updateUserDto.Role ?? UserRole.Employee,
            };

            try
            {
                var updatedUser = await _userService.UpdateUserAsync(id, user);

                if (updatedUser == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found for update", id);
                    return NotFound(new { message = $"User with ID {id} not found" });
                }

                _logger.LogInformation("User with ID {UserId} updated successfully", id);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID: {UserId}", id);
                return BadRequest(new { message = "Failed to update user", error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves all users pending approval for a specific company.
        /// </summary>
        /// <param name="companyId">The company ID to filter pending users.</param>
        /// <returns>A list of users pending approval.</returns>
        /// <response code="200">Returns the list of pending users.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpGet("pending/{companyId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetPendingUsers(int companyId)
        {
            _logger.LogInformation("Fetching pending users for company {CompanyId}", companyId);
            var pendingUsers = await _userService.GetPendingUsersAsync(companyId);
            var response = pendingUsers.Select(UserResponseDto.FromUser);
            return Ok(response);
        }

        /// <summary>
        /// Approves a user's registration request.
        /// </summary>
        /// <param name="id">The ID of the user to approve.</param>
        /// <param name="approveDto">Optional approval details.</param>
        /// <returns>The approved user.</returns>
        /// <response code="200">Returns the approved user.</response>
        /// <response code="400">If the approval fails.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="404">If the user is not found.</response>
        [HttpPost("{id}/approve")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ApproveUser(int id, [FromBody] ApproveUserDto? approveDto)
        {
            _logger.LogInformation("Approving user with ID: {UserId}", id);

            // Get current user ID from claims (the approver)
            var approverIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(approverIdClaim, out var approverId))
            {
                // Fallback: try to get from email
                var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                if (!string.IsNullOrEmpty(email))
                {
                    var approver = await _userService.GetUserByEmailAsync(email);
                    if (approver != null)
                    {
                        approverId = approver.Id;
                    }
                }
            }

            try
            {
                var approvedUser = await _userService.ApproveUserAsync(id, approverId);

                if (approvedUser == null)
                {
                    _logger.LogWarning("User with ID {UserId} not found for approval", id);
                    return NotFound(new { message = $"User with ID {id} not found" });
                }

                _logger.LogInformation("User with ID {UserId} approved successfully by {ApproverId}", id, approverId);
                return Ok(UserResponseDto.FromUser(approvedUser));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving user with ID: {UserId}", id);
                return BadRequest(new { message = "Failed to approve user", error = ex.Message });
            }
        }

        /// <summary>
        /// Rejects and deletes a user's registration request.
        /// </summary>
        /// <param name="id">The ID of the user to reject.</param>
        /// <param name="rejectDto">Optional rejection details.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">If the user was successfully rejected and deleted.</response>
        /// <response code="400">If the rejection fails.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="404">If the user is not found.</response>
        [HttpPost("{id}/reject")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RejectUser(int id, [FromBody] RejectUserDto? rejectDto)
        {
            _logger.LogInformation("Rejecting user with ID: {UserId}, Reason: {Reason}", id, rejectDto?.Reason ?? "No reason provided");

            try
            {
                var result = await _userService.DeleteUserAsync(id);

                if (!result)
                {
                    _logger.LogWarning("User with ID {UserId} not found for rejection", id);
                    return NotFound(new { message = $"User with ID {id} not found" });
                }

                _logger.LogInformation("User with ID {UserId} rejected and deleted successfully", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting user with ID: {UserId}", id);
                return BadRequest(new { message = "Failed to reject user", error = ex.Message });
            }
        }
    }
}
