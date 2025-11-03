using Microsoft.EntityFrameworkCore;
using Timora.Data.Data;
using Timora.Data.Entities;

namespace Timora.Api.Repositories
{
    /// <summary>
    /// Repository implementation for Company entity data access operations.
    /// </summary>
    public class CompanyRepository : ICompanyRepository
    {
        private readonly TimoraDbContext _context;

        /// <summary>
        /// Initializes a new instance of the CompanyRepository.
        /// </summary>
        /// <param name="context">The database context.</param>
        public CompanyRepository(TimoraDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all companies from the database with their associated users.
        /// </summary>
        /// <returns>A collection of all companies.</returns>
        public async Task<IEnumerable<Company>> GetAllCompaniesAsync()
        {
            return await _context.Companies.Include(c => c.Users).ToListAsync();
        }

        /// <summary>
        /// Retrieves a company by its unique identifier with its associated users.
        /// </summary>
        /// <param name="id">The unique identifier of the company.</param>
        /// <returns>The company if found; otherwise, null.</returns>
        public async Task<Company?> GetCompanyByIdAsync(int id)
        {
            return await _context
                .Companies.Include(c => c.Users)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
