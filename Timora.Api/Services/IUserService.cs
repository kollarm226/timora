using Timora.Data.Entities;

namespace Timora.Api.Services
{
    /// <summary>
    /// Service interface for user business logic operations.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Retrieves all users from the system.
        /// </summary>
        /// <returns>A collection of all users.</returns>
        Task<IEnumerable<User>> GetAllUsersAsync();

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        Task<User?> GetUserByIdAsync(int id);

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        Task<User?> GetUserByEmailAsync(string email);

        /// <summary>
        /// Creates a new user in the system.
        /// </summary>
        /// <param name="user">The user entity to create.</param>
        /// <returns>The created user with generated ID.</returns>
        Task<User> CreateUserAsync(User user);

        /// <summary>
        /// Deletes a user from the system by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user to delete.</param>
        /// <returns>True if the user was deleted; false if not found.</returns>
        Task<bool> DeleteUserAsync(int id);
    }
}
