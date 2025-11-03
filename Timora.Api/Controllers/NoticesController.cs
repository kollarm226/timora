using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Timora.Api.Services;

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
    }
}
