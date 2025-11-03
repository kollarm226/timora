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
    }
}
