using Microsoft.EntityFrameworkCore;
using Timora.Data.Data;
using Timora.Data.Entities;

namespace Timora.Api.Repositories
{
    /// <summary>
    /// Repository implementation for User entity data access operations.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly TimoraDbContext _context;

        /// <summary>
        /// Initializes a new instance of the UserRepository.
        /// </summary>
        /// <param name="context">The database context.</param>
        public UserRepository(TimoraDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all users from the database with their associated company.
        /// </summary>
        /// <returns>A collection of all users.</returns>
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.Include(u => u.Company).ToListAsync();
        }

        /// <summary>
        /// Retrieves a user by their unique identifier with their associated company.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context
                .Users.Include(u => u.Company)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        /// <summary>
        /// Retrieves a user by their email address with their associated company.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context
                .Users.Include(u => u.Company)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <summary>
        /// Retrieves a user by their Firebase UID with their associated company.
        /// </summary>
        /// <param name="firebaseId">The Firebase UID of the user.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        public async Task<User?> GetUserByFirebaseIdAsync(string firebaseId)
        {
            return await _context
                .Users.Include(u => u.Company)
                .FirstOrDefaultAsync(u => u.FirebaseId == firebaseId);
        }

        /// <summary>
        /// Creates a new user in the database.
        /// </summary>
        /// <param name="user">The user entity to create.</param>
        /// <returns>The created user with generated ID.</returns>
        public async Task<User> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Reload with navigation properties
            return await _context.Users.Include(u => u.Company).FirstAsync(u => u.Id == user.Id);
        }

        /// <summary>
        /// Deletes a user from the database by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user to delete.</param>
        /// <returns>True if the user was deleted; false if not found.</returns>
        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Updates an existing user in the database with the provided values.
        /// Only non-null properties from the provided user entity will be applied.
        /// </summary>
        /// <param name="id">The unique identifier of the user to update.</param>
        /// <param name="user">The user entity containing updated values.</param>
        /// <returns>The updated user if found; otherwise, null.</returns>
        public async Task<User?> UpdateUserAsync(int id, User user)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
            {
                return null;
            }

            // Apply partial updates - only update fields that are provided
            if (user.CompanyId != 0)
                existingUser.CompanyId = user.CompanyId;
            if (!string.IsNullOrEmpty(user.FirstName))
                existingUser.FirstName = user.FirstName;
            if (!string.IsNullOrEmpty(user.LastName))
                existingUser.LastName = user.LastName;
            if (!string.IsNullOrEmpty(user.Email))
                existingUser.Email = user.Email;
            if (!string.IsNullOrEmpty(user.UserName))
                existingUser.UserName = user.UserName;
            if (user.Role != existingUser.Role)
                existingUser.Role = user.Role;

            await _context.SaveChangesAsync();

            // Reload with navigation properties
            return await _context
                .Users.Include(u => u.Company)
                .FirstAsync(u => u.Id == id);
        }
    }
}
