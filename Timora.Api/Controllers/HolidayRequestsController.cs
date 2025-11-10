using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Timora.Api.DTOs;
using Timora.Api.Services;
using Timora.Data.Entities;

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

        /// <summary>
        /// Creates a new holiday request in the system.
        /// </summary>
        /// <param name="createHolidayRequestDto">The holiday request data to create.</param>
        /// <returns>The created holiday request.</returns>
        /// <response code="201">Returns the newly created holiday request.</response>
        /// <response code="400">If the holiday request data is invalid.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateHolidayRequest(
            [FromBody] CreateHolidayRequestDto createHolidayRequestDto
        )
        {
            _logger.LogInformation(
                "Creating new holiday request for user ID: {UserId}",
                createHolidayRequestDto.UserId
            );

            // Validate dates
            if (createHolidayRequestDto.EndDate < createHolidayRequestDto.StartDate)
            {
                _logger.LogWarning("Invalid date range: End date is before start date");
                return BadRequest(
                    new { message = "End date must be after or equal to start date" }
                );
            }

            // Map DTO to entity
            var holidayRequest = new HolidayRequest
            {
                UserId = createHolidayRequestDto.UserId,
                StartDate = createHolidayRequestDto.StartDate,
                EndDate = createHolidayRequestDto.EndDate,
                Reason = createHolidayRequestDto.Reason,
                Status = HolidayRequestStatus.Pending,
                RequestedAt = DateTime.UtcNow,
            };

            try
            {
                var createdHolidayRequest = await _holidayRequestService.CreateHolidayRequestAsync(
                    holidayRequest
                );
                _logger.LogInformation(
                    "Holiday request created successfully with ID: {HolidayRequestId}",
                    createdHolidayRequest.Id
                );

                return CreatedAtAction(
                    nameof(GetHolidayRequestById),
                    new { id = createdHolidayRequest.Id },
                    createdHolidayRequest
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error creating holiday request for user ID: {UserId}",
                    createHolidayRequestDto.UserId
                );
                return BadRequest(
                    new { message = "Failed to create holiday request", error = ex.Message }
                );
            }
        }
    }
}
