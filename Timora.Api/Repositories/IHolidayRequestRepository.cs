using Timora.Data.Entities;

namespace Timora.Api.Repositories
{
    /// <summary>
    /// Repository interface for HolidayRequest entity data access operations.
    /// </summary>
    public interface IHolidayRequestRepository
    {
        /// <summary>
        /// Retrieves all holiday requests from the database.
        /// </summary>
        /// <returns>A collection of all holiday requests.</returns>
        Task<IEnumerable<HolidayRequest>> GetAllHolidayRequestsAsync();

        /// <summary>
        /// Retrieves a holiday request by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the holiday request.</param>
        /// <returns>The holiday request if found; otherwise, null.</returns>
        Task<HolidayRequest?> GetHolidayRequestByIdAsync(int id);

        /// <summary>
        /// Creates a new holiday request in the database.
        /// </summary>
        /// <param name="holidayRequest">The holiday request entity to create.</param>
        /// <returns>The created holiday request with generated ID.</returns>
        Task<HolidayRequest> CreateHolidayRequestAsync(HolidayRequest holidayRequest);
    }
}
