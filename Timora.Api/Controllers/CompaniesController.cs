using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Timora.Api.DTOs;
using Timora.Api.Services;
using Timora.Data.Entities;

namespace Timora.Api.Controllers
{
    /// <summary>
    /// Controller for managing company operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly ILogger<CompaniesController> _logger;

        /// <summary>
        /// Initializes a new instance of the CompaniesController.
        /// </summary>
        /// <param name="companyService">The company service.</param>
        /// <param name="logger">The logger instance.</param>
        public CompaniesController(
            ICompanyService companyService,
            ILogger<CompaniesController> logger
        )
        {
            _companyService = companyService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all companies in the system.
        /// </summary>
        /// <returns>A list of all companies.</returns>
        /// <response code="200">Returns the list of companies.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllCompanies()
        {
            _logger.LogInformation("Fetching all companies");
            var companies = await _companyService.GetAllCompaniesAsync();
            return Ok(companies);
        }

        /// <summary>
        /// Retrieves a specific company by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the company.</param>
        /// <returns>The requested company.</returns>
        /// <response code="200">Returns the requested company.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="404">If the company is not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCompanyById(int id)
        {
            _logger.LogInformation("Fetching company with ID: {CompanyId}", id);
            var company = await _companyService.GetCompanyByIdAsync(id);

            if (company == null)
            {
                _logger.LogWarning("Company with ID {CompanyId} not found", id);
                return NotFound(new { message = $"Company with ID {id} not found" });
            }

            return Ok(company);
        }

        /// <summary>
        /// Creates a new company in the system.
        /// </summary>
        /// <param name="createCompanyDto">The company data to create.</param>
        /// <returns>The created company.</returns>
        /// <response code="201">Returns the newly created company.</response>
        /// <response code="400">If the company data is invalid.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyDto createCompanyDto)
        {
            _logger.LogInformation(
                "Creating new company with name: {CompanyName}",
                createCompanyDto.Name
            );

            // Map DTO to entity
            var company = new Company { Name = createCompanyDto.Name };

            try
            {
                var createdCompany = await _companyService.CreateCompanyAsync(company);
                _logger.LogInformation(
                    "Company created successfully with ID: {CompanyId}",
                    createdCompany.Id
                );

                return CreatedAtAction(
                    nameof(GetCompanyById),
                    new { id = createdCompany.Id },
                    createdCompany
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error creating company with name: {CompanyName}",
                    createCompanyDto.Name
                );
                return BadRequest(new { message = "Failed to create company", error = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a company from the system by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the company to delete.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">If the company was successfully deleted.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="404">If the company is not found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            _logger.LogInformation("Attempting to delete company with ID: {CompanyId}", id);

            var result = await _companyService.DeleteCompanyAsync(id);

            if (!result)
            {
                _logger.LogWarning("Company with ID {CompanyId} not found for deletion", id);
                return NotFound(new { message = $"Company with ID {id} not found" });
            }

            _logger.LogInformation("Company with ID {CompanyId} deleted successfully", id);
            return NoContent();
        }
    }
}
