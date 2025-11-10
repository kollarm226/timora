using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Timora.Api.DTOs;
using Timora.Api.Services;
using Timora.Data.Entities;

namespace Timora.Api.Controllers
{
    /// <summary>
    /// Controller for managing notice operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class NoticesController : ControllerBase
    {
        private readonly INoticeService _noticeService;
        private readonly ILogger<NoticesController> _logger;

        /// <summary>
        /// Initializes a new instance of the NoticesController.
        /// </summary>
        /// <param name="noticeService">The notice service.</param>
        /// <param name="logger">The logger instance.</param>
        public NoticesController(INoticeService noticeService, ILogger<NoticesController> logger)
        {
            _noticeService = noticeService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all notices in the system ordered by creation date (newest first).
        /// </summary>
        /// <returns>A list of all notices.</returns>
        /// <response code="200">Returns the list of notices.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllNotices()
        {
            _logger.LogInformation("Fetching all notices");
            var notices = await _noticeService.GetAllNoticesAsync();
            return Ok(notices);
        }

        /// <summary>
        /// Retrieves a specific notice by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the notice.</param>
        /// <returns>The requested notice.</returns>
        /// <response code="200">Returns the requested notice.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="404">If the notice is not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetNoticeById(int id)
        {
            _logger.LogInformation("Fetching notice with ID: {NoticeId}", id);
            var notice = await _noticeService.GetNoticeByIdAsync(id);

            if (notice == null)
            {
                _logger.LogWarning("Notice with ID {NoticeId} not found", id);
                return NotFound(new { message = $"Notice with ID {id} not found" });
            }

            return Ok(notice);
        }

        /// <summary>
        /// Creates a new notice in the system.
        /// </summary>
        /// <param name="createNoticeDto">The notice data to create.</param>
        /// <returns>The created notice.</returns>
        /// <response code="201">Returns the newly created notice.</response>
        /// <response code="400">If the notice data is invalid.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateNotice([FromBody] CreateNoticeDto createNoticeDto)
        {
            _logger.LogInformation(
                "Creating new notice with title: {Title} for user ID: {UserId}",
                createNoticeDto.Title,
                createNoticeDto.UserId
            );

            // Map DTO to entity
            var notice = new Notice
            {
                UserId = createNoticeDto.UserId,
                Title = createNoticeDto.Title,
                Content = createNoticeDto.Content,
                CreatedAt = DateTime.UtcNow,
            };

            try
            {
                var createdNotice = await _noticeService.CreateNoticeAsync(notice);
                _logger.LogInformation(
                    "Notice created successfully with ID: {NoticeId}",
                    createdNotice.Id
                );

                return CreatedAtAction(
                    nameof(GetNoticeById),
                    new { id = createdNotice.Id },
                    createdNotice
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error creating notice with title: {Title}",
                    createNoticeDto.Title
                );
                return BadRequest(new { message = "Failed to create notice", error = ex.Message });
            }
        }
    }
}
