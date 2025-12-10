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

        /// <summary>
        /// Deletes a company from the database by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the company to delete.</param>
        /// <returns>True if the company was deleted; false if not found.</returns>
        Task<bool> DeleteCompanyAsync(int id);

        /// <summary>
        /// Updates an existing company in the database with the provided values.
        /// </summary>
        /// <param name="id">The unique identifier of the company to update.</param>
        /// <param name="company">The company entity containing updated values.</param>
        /// <returns>The updated company if found; otherwise, null.</returns>
        Task<Company?> UpdateCompanyAsync(int id, Company company);
    }
}
