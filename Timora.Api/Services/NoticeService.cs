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
    }
}
