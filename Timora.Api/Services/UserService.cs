using Timora.Api.Repositories;
using Timora.Data.Entities;

namespace Timora.Api.Services
{
    /// <summary>
    /// Service implementation for user business logic operations.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the UserService.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Retrieves all users from the system.
        /// </summary>
        /// <returns>A collection of all users.</returns>
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetUserByEmailAsync(email);
        }

        /// <summary>
        /// Retrieves a user by their Firebase UID.
        /// </summary>
        /// <param name="firebaseId">The Firebase UID of the user.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        public async Task<User?> GetUserByFirebaseIdAsync(string firebaseId)
        {
            return await _userRepository.GetUserByFirebaseIdAsync(firebaseId);
        }

        /// <summary>
        /// Creates a new user in the system.
        /// </summary>
        /// <param name="user">The user entity to create.</param>
        /// <returns>The created user with generated ID.</returns>
        public async Task<User> CreateUserAsync(User user)
        {
            return await _userRepository.CreateUserAsync(user);
        }

        /// <summary>
        /// Deletes a user from the system by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user to delete.</param>
        /// <returns>True if the user was deleted; false if not found.</returns>
        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _userRepository.DeleteUserAsync(id);
        }

        /// <summary>
        /// Updates an existing user in the system with the provided values.
        /// </summary>
        /// <param name="id">The unique identifier of the user to update.</param>
        /// <param name="user">The user entity containing updated values.</param>
        /// <returns>The updated user if found; otherwise, null.</returns>
        public async Task<User?> UpdateUserAsync(int id, User user)
        {
            return await _userRepository.UpdateUserAsync(id, user);
        }
    }
}
