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
    }
}
