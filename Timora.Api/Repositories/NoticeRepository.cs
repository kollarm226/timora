using Microsoft.EntityFrameworkCore;
using Timora.Data.Data;
using Timora.Data.Entities;

namespace Timora.Api.Repositories
{
    /// <summary>
    /// Repository implementation for Notice entity data access operations.
    /// </summary>
    public class NoticeRepository : INoticeRepository
    {
        private readonly TimoraDbContext _context;

        /// <summary>
        /// Initializes a new instance of the NoticeRepository.
        /// </summary>
        /// <param name="context">The database context.</param>
        public NoticeRepository(TimoraDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all notices from the database with associated user information.
        /// </summary>
        /// <returns>A collection of all notices.</returns>
        public async Task<IEnumerable<Notice>> GetAllNoticesAsync()
        {
            return await _context
                .Notices.Include(n => n.User)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Retrieves a notice by its unique identifier with associated user information.
        /// </summary>
        /// <param name="id">The unique identifier of the notice.</param>
        /// <returns>The notice if found; otherwise, null.</returns>
        public async Task<Notice?> GetNoticeByIdAsync(int id)
        {
            return await _context.Notices.Include(n => n.User).FirstOrDefaultAsync(n => n.Id == id);
        }

        /// <summary>
        /// Creates a new notice in the database.
        /// </summary>
        /// <param name="notice">The notice entity to create.</param>
        /// <returns>The created notice with generated ID.</returns>
        public async Task<Notice> CreateNoticeAsync(Notice notice)
        {
            _context.Notices.Add(notice);
            await _context.SaveChangesAsync();

            // Reload with navigation properties
            return await _context.Notices.Include(n => n.User).FirstAsync(n => n.Id == notice.Id);
        }
    }
}
