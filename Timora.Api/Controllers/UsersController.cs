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
    }
}
