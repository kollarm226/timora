using Timora.Data.Entities;

namespace Timora.Api.Services
{
    /// <summary>
    /// Service interface for notice business logic operations.
    /// </summary>
    public interface INoticeService
    {
        /// <summary>
        /// Retrieves all notices from the system.
        /// </summary>
        /// <returns>A collection of all notices.</returns>
        Task<IEnumerable<Notice>> GetAllNoticesAsync();

        /// <summary>
        /// Retrieves a notice by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the notice.</param>
        /// <returns>The notice if found; otherwise, null.</returns>
        Task<Notice?> GetNoticeByIdAsync(int id);
    }
}
