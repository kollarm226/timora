using Timora.Data.Entities;

namespace Timora.Api.Repositories
{
    /// <summary>
    /// Repository interface for Notice entity data access operations.
    /// </summary>
    public interface INoticeRepository
    {
        /// <summary>
        /// Retrieves all notices from the database.
        /// </summary>
        /// <returns>A collection of all notices.</returns>
        Task<IEnumerable<Notice>> GetAllNoticesAsync();

        /// <summary>
        /// Retrieves a notice by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the notice.</param>
        /// <returns>The notice if found; otherwise, null.</returns>
        Task<Notice?> GetNoticeByIdAsync(int id);

        /// <summary>
        /// Creates a new notice in the database.
        /// </summary>
        /// <param name="notice">The notice entity to create.</param>
        /// <returns>The created notice with generated ID.</returns>
        Task<Notice> CreateNoticeAsync(Notice notice);
    }
}
