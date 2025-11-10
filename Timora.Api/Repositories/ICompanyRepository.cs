using Timora.Data.Entities;

namespace Timora.Api.Repositories
{
    /// <summary>
    /// Repository interface for Company entity data access operations.
    /// </summary>
    public interface ICompanyRepository
    {
        /// <summary>
        /// Retrieves all companies from the database.
        /// </summary>
        /// <returns>A collection of all companies.</returns>
        Task<IEnumerable<Company>> GetAllCompaniesAsync();

        /// <summary>
        /// Retrieves a company by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the company.</param>
        /// <returns>The company if found; otherwise, null.</returns>
        Task<Company?> GetCompanyByIdAsync(int id);

        /// <summary>
        /// Creates a new company in the database.
        /// </summary>
        /// <param name="company">The company entity to create.</param>
        /// <returns>The created company with generated ID.</returns>
        Task<Company> CreateCompanyAsync(Company company);
    }
}
