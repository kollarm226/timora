using Timora.Data.Entities;

namespace Timora.Api.Services
{
    /// <summary>
    /// Service interface for company business logic operations.
    /// </summary>
    public interface ICompanyService
    {
        /// <summary>
        /// Retrieves all companies from the system.
        /// </summary>
        /// <returns>A collection of all companies.</returns>
        Task<IEnumerable<Company>> GetAllCompaniesAsync();

        /// <summary>
        /// Retrieves a company by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the company.</param>
        /// <returns>The company if found; otherwise, null.</returns>
        Task<Company?> GetCompanyByIdAsync(int id);
    }
}
