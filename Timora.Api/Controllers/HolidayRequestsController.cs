using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Timora.Api.Services;

namespace Timora.Api.Controllers
{
    /// <summary>
    /// Controller for managing holiday request operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class HolidayRequestsController : ControllerBase
    {
        private readonly IHolidayRequestService _holidayRequestService;
        private readonly ILogger<HolidayRequestsController> _logger;

        /// <summary>
        /// Initializes a new instance of the HolidayRequestsController.
        /// </summary>
        /// <param name="holidayRequestService">The holiday request service.</param>
        /// <param name="logger">The logger instance.</param>
        public HolidayRequestsController(
            IHolidayRequestService holidayRequestService,
            ILogger<HolidayRequestsController> logger
        )
        {
            _holidayRequestService = holidayRequestService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all holiday requests in the system.
        /// </summary>
        /// <returns>A list of all holiday requests.</returns>
        /// <response code="200">Returns the list of holiday requests.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllHolidayRequests()
        {
            _logger.LogInformation("Fetching all holiday requests");
            var holidayRequests = await _holidayRequestService.GetAllHolidayRequestsAsync();
            return Ok(holidayRequests);
        }

        /// <summary>
        /// Retrieves a specific holiday request by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the holiday request.</param>
        /// <returns>The requested holiday request.</returns>
        /// <response code="200">Returns the requested holiday request.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="404">If the holiday request is not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHolidayRequestById(int id)
        {
            _logger.LogInformation("Fetching holiday request with ID: {HolidayRequestId}", id);
            var holidayRequest = await _holidayRequestService.GetHolidayRequestByIdAsync(id);

            if (holidayRequest == null)
            {
                _logger.LogWarning("Holiday request with ID {HolidayRequestId} not found", id);
                return NotFound(new { message = $"Holiday request with ID {id} not found" });
            }

            return Ok(holidayRequest);
        }
    }
}
