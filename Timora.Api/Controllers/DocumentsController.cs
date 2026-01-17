using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Timora.Api.DTOs;
using Timora.Api.Services;
using Timora.Data.Entities;

namespace Timora.Api.Controllers
{
    /// <summary>
    /// Controller for managing company documents.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;
        private readonly ILogger<DocumentsController> _logger;

        /// <summary>
        /// Initializes a new instance of the DocumentsController.
        /// </summary>
        /// <param name="documentService">The document service.</param>
        /// <param name="logger">The logger instance.</param>
        public DocumentsController(IDocumentService documentService, ILogger<DocumentsController> logger)
        {
            _documentService = documentService;
            _logger = logger;
        }

        /// <summary>
        /// Gets all documents for the current user's company.
        /// </summary>
        /// <returns>A list of documents belonging to the user's company.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllDocuments()
        {
            var companyId = GetCurrentUserCompanyId();
            if (companyId == null)
            {
                return BadRequest(new { message = "Company ID not found in user claims" });
            }

            _logger.LogInformation("Fetching all documents for company {CompanyId}", companyId);
            var documents = await _documentService.GetDocumentsByCompanyIdAsync(companyId.Value);
            return Ok(documents);
        }

        /// <summary>
        /// Gets a specific document by ID.
        /// </summary>
        /// <param name="id">The document ID.</param>
        /// <returns>The document if found and user has access.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDocumentById(int id)
        {
            var companyId = GetCurrentUserCompanyId();
            if (companyId == null)
            {
                return BadRequest(new { message = "Company ID not found in user claims" });
            }

            var document = await _documentService.GetDocumentByIdAsync(id);
            if (document == null)
            {
                return NotFound(new { message = $"Document with ID {id} not found" });
            }

            // Verify the document belongs to the user's company
            if (document.CompanyId != companyId.Value)
            {
                return Forbid();
            }

            return Ok(document);
        }

        /// <summary>
        /// Creates a new document.
        /// </summary>
        /// <param name="dto">The document data.</param>
        /// <returns>The created document.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateDocument([FromBody] CreateDocumentDto dto)
        {
            var companyId = GetCurrentUserCompanyId();
            if (companyId == null)
            {
                return BadRequest(new { message = "Company ID not found in user claims" });
            }

            // Verify the user can only create documents for their own company
            if (dto.CompanyId != companyId.Value)
            {
                return Forbid();
            }

            var document = new Document
            {
                CompanyId = dto.CompanyId,
                Title = dto.Title,
                Description = dto.Description,
                FileUrl = dto.FileUrl
            };

            _logger.LogInformation("Creating document '{Title}' for company {CompanyId}", dto.Title, dto.CompanyId);
            var created = await _documentService.CreateDocumentAsync(document);
            return CreatedAtAction(nameof(GetDocumentById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Updates an existing document.
        /// </summary>
        /// <param name="id">The document ID.</param>
        /// <param name="dto">The updated document data.</param>
        /// <returns>The updated document.</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateDocument(int id, [FromBody] UpdateDocumentDto dto)
        {
            var companyId = GetCurrentUserCompanyId();
            if (companyId == null)
            {
                return BadRequest(new { message = "Company ID not found in user claims" });
            }

            // First check if document exists and belongs to user's company
            var existingDocument = await _documentService.GetDocumentByIdAsync(id);
            if (existingDocument == null)
            {
                return NotFound(new { message = $"Document with ID {id} not found" });
            }

            if (existingDocument.CompanyId != companyId.Value)
            {
                return Forbid();
            }

            var document = new Document
            {
                Title = dto.Title ?? string.Empty,
                Description = dto.Description ?? string.Empty,
                FileUrl = dto.FileUrl ?? string.Empty
            };

            _logger.LogInformation("Updating document {DocumentId}", id);
            var updated = await _documentService.UpdateDocumentAsync(id, document);
            return Ok(updated);
        }

        /// <summary>
        /// Deletes a document.
        /// </summary>
        /// <param name="id">The document ID.</param>
        /// <returns>No content if successful.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDocument(int id)
        {
            var companyId = GetCurrentUserCompanyId();
            if (companyId == null)
            {
                return BadRequest(new { message = "Company ID not found in user claims" });
            }

            // First check if document exists and belongs to user's company
            var existingDocument = await _documentService.GetDocumentByIdAsync(id);
            if (existingDocument == null)
            {
                return NotFound(new { message = $"Document with ID {id} not found" });
            }

            if (existingDocument.CompanyId != companyId.Value)
            {
                return Forbid();
            }

            _logger.LogInformation("Deleting document {DocumentId}", id);
            await _documentService.DeleteDocumentAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Gets the current user's company ID from claims.
        /// </summary>
        /// <returns>The company ID if found, otherwise null.</returns>
        private int? GetCurrentUserCompanyId()
        {
            var companyIdClaim = User.FindFirst("CompanyId")?.Value;
            if (int.TryParse(companyIdClaim, out var companyId))
            {
                return companyId;
            }
            return null;
        }
    }
}
