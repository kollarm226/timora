using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Timora.Api.Services;

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
    }
}
