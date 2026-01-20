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
        /// Retrieves a user by their Firebase UID.
        /// </summary>
        /// <param name="firebaseId">The Firebase UID of the user.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        Task<User?> GetUserByFirebaseIdAsync(string firebaseId);

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

        /// <summary>
        /// Updates an existing user in the system with the provided values.
        /// </summary>
        /// <param name="id">The unique identifier of the user to update.</param>
        /// <param name="user">The user entity containing updated values.</param>
        /// <returns>The updated user if found; otherwise, null.</returns>
        Task<User?> UpdateUserAsync(int id, User user);

        /// <summary>
        /// Approves a user's registration request.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to approve.</param>
        /// <param name="approverId">The unique identifier of the employer approving the user.</param>
        /// <returns>The approved user if found; otherwise, null.</returns>
        Task<User?> ApproveUserAsync(int userId, int approverId);

        /// <summary>
        /// Retrieves all users pending approval for a specific company.
        /// </summary>
        /// <param name="companyId">The unique identifier of the company.</param>
        /// <returns>A collection of users pending approval.</returns>
        Task<IEnumerable<User>> GetPendingUsersAsync(int companyId);
    }
}
