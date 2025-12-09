using Timora.Api.Repositories;
using Timora.Data.Entities;

namespace Timora.Api.Services
{
    /// <summary>
    /// Service implementation for notice business logic operations.
    /// </summary>
    public class NoticeService : INoticeService
    {
        private readonly INoticeRepository _noticeRepository;

        /// <summary>
        /// Initializes a new instance of the NoticeService.
        /// </summary>
        /// <param name="noticeRepository">The notice repository.</param>
        public NoticeService(INoticeRepository noticeRepository)
        {
            _noticeRepository = noticeRepository;
        }

        /// <summary>
        /// Retrieves all notices from the system.
        /// </summary>
        /// <returns>A collection of all notices.</returns>
        public async Task<IEnumerable<Notice>> GetAllNoticesAsync()
        {
            return await _noticeRepository.GetAllNoticesAsync();
        }

        /// <summary>
        /// Retrieves a notice by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the notice.</param>
        /// <returns>The notice if found; otherwise, null.</returns>
        public async Task<Notice?> GetNoticeByIdAsync(int id)
        {
            return await _noticeRepository.GetNoticeByIdAsync(id);
        }

        /// <summary>
        /// Creates a new notice in the system.
        /// </summary>
        /// <param name="notice">The notice entity to create.</param>
        /// <returns>The created notice with generated ID.</returns>
        public async Task<Notice> CreateNoticeAsync(Notice notice)
        {
            return await _noticeRepository.CreateNoticeAsync(notice);
        }

        /// <summary>
        /// Deletes a notice from the system by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the notice to delete.</param>
        /// <returns>True if the notice was deleted; false if not found.</returns>
        public async Task<bool> DeleteNoticeAsync(int id)
        {
            return await _noticeRepository.DeleteNoticeAsync(id);
        }

        /// <summary>
        /// Updates an existing notice in the system with the provided values.
        /// </summary>
        /// <param name="id">The unique identifier of the notice to update.</param>
        /// <param name="notice">The notice entity containing updated values.</param>
        /// <returns>The updated notice if found; otherwise, null.</returns>
        public async Task<Notice?> UpdateNoticeAsync(int id, Notice notice)
        {
            return await _noticeRepository.UpdateNoticeAsync(id, notice);
        }
    }
}
