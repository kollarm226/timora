using Timora.Api.Repositories;
using Timora.Data.Entities;

namespace Timora.Api.Services
{
    /// <summary>
    /// Service implementation for company business logic operations.
    /// </summary>
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;

        /// <summary>
        /// Initializes a new instance of the CompanyService.
        /// </summary>
        /// <param name="companyRepository">The company repository.</param>
        public CompanyService(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        /// <summary>
        /// Retrieves all companies from the system.
        /// </summary>
        /// <returns>A collection of all companies.</returns>
        public async Task<IEnumerable<Company>> GetAllCompaniesAsync()
        {
            return await _companyRepository.GetAllCompaniesAsync();
        }

        /// <summary>
        /// Retrieves a company by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the company.</param>
        /// <returns>The company if found; otherwise, null.</returns>
        public async Task<Company?> GetCompanyByIdAsync(int id)
        {
            return await _companyRepository.GetCompanyByIdAsync(id);
        }

        /// <summary>
        /// Creates a new company in the system.
        /// </summary>
        /// <param name="company">The company entity to create.</param>
        /// <returns>The created company with generated ID.</returns>
        public async Task<Company> CreateCompanyAsync(Company company)
        {
            return await _companyRepository.CreateCompanyAsync(company);
        }

        /// <summary>
        /// Deletes a company from the system by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the company to delete.</param>
        /// <returns>True if the company was deleted; false if not found.</returns>
        public async Task<bool> DeleteCompanyAsync(int id)
        {
            return await _companyRepository.DeleteCompanyAsync(id);
        }

        /// <summary>
        /// Updates an existing company in the system with the provided values.
        /// </summary>
        /// <param name="id">The unique identifier of the company to update.</param>
        /// <param name="company">The company entity containing updated values.</param>
        /// <returns>The updated company if found; otherwise, null.</returns>
        public async Task<Company?> UpdateCompanyAsync(int id, Company company)
        {
            return await _companyRepository.UpdateCompanyAsync(id, company);
        }
    }
}
