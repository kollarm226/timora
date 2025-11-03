using Timora.Api.Repositories;
using Timora.Data.Entities;

namespace Timora.Api.Services
{
    /// <summary>
    /// Service implementation for holiday request business logic operations.
    /// </summary>
    public class HolidayRequestService : IHolidayRequestService
    {
        private readonly IHolidayRequestRepository _holidayRequestRepository;

        /// <summary>
        /// Initializes a new instance of the HolidayRequestService.
        /// </summary>
        /// <param name="holidayRequestRepository">The holiday request repository.</param>
        public HolidayRequestService(IHolidayRequestRepository holidayRequestRepository)
        {
            _holidayRequestRepository = holidayRequestRepository;
        }

        /// <summary>
        /// Retrieves all holiday requests from the system.
        /// </summary>
        /// <returns>A collection of all holiday requests.</returns>
        public async Task<IEnumerable<HolidayRequest>> GetAllHolidayRequestsAsync()
        {
            return await _holidayRequestRepository.GetAllHolidayRequestsAsync();
        }

        /// <summary>
        /// Retrieves a holiday request by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the holiday request.</param>
        /// <returns>The holiday request if found; otherwise, null.</returns>
        public async Task<HolidayRequest?> GetHolidayRequestByIdAsync(int id)
        {
            return await _holidayRequestRepository.GetHolidayRequestByIdAsync(id);
        }
    }
}
