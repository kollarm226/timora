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

        /// <summary>
        /// Deletes a notice from the database by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the notice to delete.</param>
        /// <returns>True if the notice was deleted; false if not found.</returns>
        public async Task<bool> DeleteNoticeAsync(int id)
        {
            var notice = await _context.Notices.FindAsync(id);
            if (notice == null)
            {
                return false;
            }

            _context.Notices.Remove(notice);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Updates an existing notice in the database with the provided values.
        /// Only non-null properties from the provided notice entity will be applied.
        /// </summary>
        /// <param name="id">The unique identifier of the notice to update.</param>
        /// <param name="notice">The notice entity containing updated values.</param>
        /// <returns>The updated notice if found; otherwise, null.</returns>
        public async Task<Notice?> UpdateNoticeAsync(int id, Notice notice)
        {
            var existingNotice = await _context.Notices.FindAsync(id);
            if (existingNotice == null)
            {
                return null;
            }

            // Apply partial updates
            if (!string.IsNullOrEmpty(notice.Title))
                existingNotice.Title = notice.Title;
            if (!string.IsNullOrEmpty(notice.Content))
                existingNotice.Content = notice.Content;

            await _context.SaveChangesAsync();

            // Reload with navigation properties
            return await _context.Notices.Include(n => n.User).FirstAsync(n => n.Id == id);
        }
    }
}
