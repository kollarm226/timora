using Microsoft.EntityFrameworkCore;
using Timora.Data.Data;
using Timora.Data.Entities;

namespace Timora.Api.Repositories
{
    /// <summary>
    /// Repository implementation for HolidayRequest entity data access operations.
    /// </summary>
    public class HolidayRequestRepository : IHolidayRequestRepository
    {
        private readonly TimoraDbContext _context;

        /// <summary>
        /// Initializes a new instance of the HolidayRequestRepository.
        /// </summary>
        /// <param name="context">The database context.</param>
        public HolidayRequestRepository(TimoraDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all holiday requests from the database with associated user and resolver information.
        /// </summary>
        /// <returns>A collection of all holiday requests.</returns>
        public async Task<IEnumerable<HolidayRequest>> GetAllHolidayRequestsAsync()
        {
            return await _context
                .HolidayRequests.Include(hr => hr.User)
                .Include(hr => hr.ResolvedBy)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a holiday request by its unique identifier with associated user and resolver information.
        /// </summary>
        /// <param name="id">The unique identifier of the holiday request.</param>
        /// <returns>The holiday request if found; otherwise, null.</returns>
        public async Task<HolidayRequest?> GetHolidayRequestByIdAsync(int id)
        {
            return await _context
                .HolidayRequests.Include(hr => hr.User)
                .Include(hr => hr.ResolvedBy)
                .FirstOrDefaultAsync(hr => hr.Id == id);
        }
    }
}
