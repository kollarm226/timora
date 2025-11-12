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

        /// <summary>
        /// Creates a new holiday request in the database.
        /// </summary>
        /// <param name="holidayRequest">The holiday request entity to create.</param>
        /// <returns>The created holiday request with generated ID.</returns>
        public async Task<HolidayRequest> CreateHolidayRequestAsync(HolidayRequest holidayRequest)
        {
            _context.HolidayRequests.Add(holidayRequest);
            await _context.SaveChangesAsync();

            // Reload with navigation properties
            return await _context.HolidayRequests
                .Include(hr => hr.User)
                .Include(hr => hr.ResolvedBy)
                .FirstAsync(hr => hr.Id == holidayRequest.Id);
        }

        /// <summary>
        /// Deletes a holiday request from the database by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the holiday request to delete.</param>
        /// <returns>True if the holiday request was deleted; false if not found.</returns>
        public async Task<bool> DeleteHolidayRequestAsync(int id)
        {
            var holidayRequest = await _context.HolidayRequests.FindAsync(id);
            if (holidayRequest == null)
            {
                return false;
            }

            _context.HolidayRequests.Remove(holidayRequest);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
