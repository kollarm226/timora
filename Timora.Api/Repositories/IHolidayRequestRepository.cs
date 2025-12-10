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

        /// <summary>
        /// Deletes a holiday request from the database by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the holiday request to delete.</param>
        /// <returns>True if the holiday request was deleted; false if not found.</returns>
        Task<bool> DeleteHolidayRequestAsync(int id);

        /// <summary>
        /// Updates an existing holiday request in the database with the provided values.
        /// </summary>
        /// <param name="id">The unique identifier of the holiday request to update.</param>
        /// <param name="holidayRequest">The holiday request entity containing updated values.</param>
        /// <returns>The updated holiday request if found; otherwise, null.</returns>
        Task<HolidayRequest?> UpdateHolidayRequestAsync(int id, HolidayRequest holidayRequest);
    }
}
