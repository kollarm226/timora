using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Timora.Api.Controllers;
using Timora.Api.DTOs;
using Timora.Api.Services;
using Timora.Data.Entities;

namespace Timora.Tests.Controllers;

/// <summary>
/// Unit tests for the DocumentsController.
/// </summary>
public class DocumentsControllerTests
{
    private readonly Mock<IDocumentService> _mockDocumentService;
    private readonly Mock<ILogger<DocumentsController>> _mockLogger;
    private readonly DocumentsController _controller;

    public DocumentsControllerTests()
    {
        _mockDocumentService = new Mock<IDocumentService>();
        _mockLogger = new Mock<ILogger<DocumentsController>>();
        _controller = new DocumentsController(_mockDocumentService.Object, _mockLogger.Object);
    }

    /// <summary>
    /// Sets up the controller with a user having the specified company ID claim.
    /// </summary>
    private void SetupUserWithCompanyId(int companyId)
    {
        var claims = new List<Claim>
        {
            new Claim("CompanyId", companyId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    /// <summary>
    /// Sets up the controller with a user without a company ID claim.
    /// </summary>
    private void SetupUserWithoutCompanyId()
    {
        var identity = new ClaimsIdentity(new List<Claim>(), "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    #region GetAllDocuments Tests

    /// <summary>
    /// Tests that GetAllDocuments returns OK with a list of documents for the user's company.
    /// </summary>
    [Fact]
    public async Task GetAllDocuments_ReturnsOk_WithListOfDocuments()
    {
        // Arrange
        SetupUserWithCompanyId(1);
        var documents = new List<Document>
        {
            new Document { Id = 1, CompanyId = 1, Title = "Document 1", Description = "Description 1", FileUrl = "http://example.com/doc1", CreatedAt = DateTime.UtcNow },
            new Document { Id = 2, CompanyId = 1, Title = "Document 2", Description = "Description 2", FileUrl = "http://example.com/doc2", CreatedAt = DateTime.UtcNow }
        };
        _mockDocumentService.Setup(s => s.GetDocumentsByCompanyIdAsync(1)).ReturnsAsync(documents);

        // Act
        var result = await _controller.GetAllDocuments();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedDocuments = Assert.IsAssignableFrom<IEnumerable<Document>>(okResult.Value);
        Assert.Equal(2, returnedDocuments.Count());
    }

    /// <summary>
    /// Tests that GetAllDocuments returns OK with empty list when no documents exist.
    /// </summary>
    [Fact]
    public async Task GetAllDocuments_ReturnsOk_WithEmptyList_WhenNoDocumentsExist()
    {
        // Arrange
        SetupUserWithCompanyId(1);
        _mockDocumentService.Setup(s => s.GetDocumentsByCompanyIdAsync(1)).ReturnsAsync(new List<Document>());

        // Act
        var result = await _controller.GetAllDocuments();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedDocuments = Assert.IsAssignableFrom<IEnumerable<Document>>(okResult.Value);
        Assert.Empty(returnedDocuments);
    }

    /// <summary>
    /// Tests that GetAllDocuments returns BadRequest when company ID is missing from claims.
    /// </summary>
    [Fact]
    public async Task GetAllDocuments_ReturnsBadRequest_WhenCompanyIdMissing()
    {
        // Arrange
        SetupUserWithoutCompanyId();

        // Act
        var result = await _controller.GetAllDocuments();

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    #endregion

    #region GetDocumentById Tests

    /// <summary>
    /// Tests that GetDocumentById returns OK when document exists and belongs to user's company.
    /// </summary>
    [Fact]
    public async Task GetDocumentById_ReturnsOk_WhenDocumentExistsAndBelongsToCompany()
    {
        // Arrange
        SetupUserWithCompanyId(1);
        var document = new Document { Id = 1, CompanyId = 1, Title = "Test Document", Description = "Test Description", FileUrl = "http://example.com/doc", CreatedAt = DateTime.UtcNow };
        _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(1)).ReturnsAsync(document);

        // Act
        var result = await _controller.GetDocumentById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedDocument = Assert.IsType<Document>(okResult.Value);
        Assert.Equal(1, returnedDocument.Id);
        Assert.Equal("Test Document", returnedDocument.Title);
    }

    /// <summary>
    /// Tests that GetDocumentById returns NotFound when document does not exist.
    /// </summary>
    [Fact]
    public async Task GetDocumentById_ReturnsNotFound_WhenDocumentDoesNotExist()
    {
        // Arrange
        SetupUserWithCompanyId(1);
        _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(999)).ReturnsAsync((Document?)null);

        // Act
        var result = await _controller.GetDocumentById(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    /// <summary>
    /// Tests that GetDocumentById returns Forbid when document belongs to different company.
    /// </summary>
    [Fact]
    public async Task GetDocumentById_ReturnsForbid_WhenDocumentBelongsToDifferentCompany()
    {
        // Arrange
        SetupUserWithCompanyId(1);
        var document = new Document { Id = 1, CompanyId = 2, Title = "Test Document", Description = "Test Description", FileUrl = "http://example.com/doc", CreatedAt = DateTime.UtcNow };
        _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(1)).ReturnsAsync(document);

        // Act
        var result = await _controller.GetDocumentById(1);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    /// <summary>
    /// Tests that GetDocumentById returns BadRequest when company ID is missing from claims.
    /// </summary>
    [Fact]
    public async Task GetDocumentById_ReturnsBadRequest_WhenCompanyIdMissing()
    {
        // Arrange
        SetupUserWithoutCompanyId();

        // Act
        var result = await _controller.GetDocumentById(1);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    #endregion

    #region CreateDocument Tests

    /// <summary>
    /// Tests that CreateDocument returns Created when successful.
    /// </summary>
    [Fact]
    public async Task CreateDocument_ReturnsCreated_WhenSuccessful()
    {
        // Arrange
        SetupUserWithCompanyId(1);
        var createDto = new CreateDocumentDto { CompanyId = 1, Title = "New Document", Description = "New Description", FileUrl = "http://example.com/new" };
        var createdDocument = new Document { Id = 1, CompanyId = 1, Title = "New Document", Description = "New Description", FileUrl = "http://example.com/new", CreatedAt = DateTime.UtcNow };
        _mockDocumentService.Setup(s => s.CreateDocumentAsync(It.IsAny<Document>())).ReturnsAsync(createdDocument);

        // Act
        var result = await _controller.CreateDocument(createDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.GetDocumentById), createdResult.ActionName);
        var returnedDocument = Assert.IsType<Document>(createdResult.Value);
        Assert.Equal(1, returnedDocument.Id);
    }

    /// <summary>
    /// Tests that CreateDocument returns Forbid when trying to create for different company.
    /// </summary>
    [Fact]
    public async Task CreateDocument_ReturnsForbid_WhenCreatingForDifferentCompany()
    {
        // Arrange
        SetupUserWithCompanyId(1);
        var createDto = new CreateDocumentDto { CompanyId = 2, Title = "New Document", Description = "New Description", FileUrl = "http://example.com/new" };

        // Act
        var result = await _controller.CreateDocument(createDto);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    /// <summary>
    /// Tests that CreateDocument returns BadRequest when company ID is missing from claims.
    /// </summary>
    [Fact]
    public async Task CreateDocument_ReturnsBadRequest_WhenCompanyIdMissing()
    {
        // Arrange
        SetupUserWithoutCompanyId();
        var createDto = new CreateDocumentDto { CompanyId = 1, Title = "New Document", Description = "New Description", FileUrl = "http://example.com/new" };

        // Act
        var result = await _controller.CreateDocument(createDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    #endregion

    #region UpdateDocument Tests

    /// <summary>
    /// Tests that UpdateDocument returns OK when successful.
    /// </summary>
    [Fact]
    public async Task UpdateDocument_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        SetupUserWithCompanyId(1);
        var existingDocument = new Document { Id = 1, CompanyId = 1, Title = "Existing", Description = "Existing", FileUrl = "http://example.com/existing", CreatedAt = DateTime.UtcNow };
        var updateDto = new UpdateDocumentDto { Title = "Updated Document" };
        var updatedDocument = new Document { Id = 1, CompanyId = 1, Title = "Updated Document", Description = "Existing", FileUrl = "http://example.com/existing", CreatedAt = DateTime.UtcNow };

        _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(1)).ReturnsAsync(existingDocument);
        _mockDocumentService.Setup(s => s.UpdateDocumentAsync(1, It.IsAny<Document>())).ReturnsAsync(updatedDocument);

        // Act
        var result = await _controller.UpdateDocument(1, updateDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedDocument = Assert.IsType<Document>(okResult.Value);
        Assert.Equal("Updated Document", returnedDocument.Title);
    }

    /// <summary>
    /// Tests that UpdateDocument returns NotFound when document does not exist.
    /// </summary>
    [Fact]
    public async Task UpdateDocument_ReturnsNotFound_WhenDocumentDoesNotExist()
    {
        // Arrange
        SetupUserWithCompanyId(1);
        var updateDto = new UpdateDocumentDto { Title = "Updated Document" };
        _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(999)).ReturnsAsync((Document?)null);

        // Act
        var result = await _controller.UpdateDocument(999, updateDto);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    /// <summary>
    /// Tests that UpdateDocument returns Forbid when document belongs to different company.
    /// </summary>
    [Fact]
    public async Task UpdateDocument_ReturnsForbid_WhenDocumentBelongsToDifferentCompany()
    {
        // Arrange
        SetupUserWithCompanyId(1);
        var existingDocument = new Document { Id = 1, CompanyId = 2, Title = "Existing", Description = "Existing", FileUrl = "http://example.com/existing", CreatedAt = DateTime.UtcNow };
        var updateDto = new UpdateDocumentDto { Title = "Updated Document" };
        _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(1)).ReturnsAsync(existingDocument);

        // Act
        var result = await _controller.UpdateDocument(1, updateDto);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    /// <summary>
    /// Tests that UpdateDocument returns BadRequest when company ID is missing from claims.
    /// </summary>
    [Fact]
    public async Task UpdateDocument_ReturnsBadRequest_WhenCompanyIdMissing()
    {
        // Arrange
        SetupUserWithoutCompanyId();
        var updateDto = new UpdateDocumentDto { Title = "Updated Document" };

        // Act
        var result = await _controller.UpdateDocument(1, updateDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    #endregion

    #region DeleteDocument Tests

    /// <summary>
    /// Tests that DeleteDocument returns NoContent when successful.
    /// </summary>
    [Fact]
    public async Task DeleteDocument_ReturnsNoContent_WhenSuccessful()
    {
        // Arrange
        SetupUserWithCompanyId(1);
        var existingDocument = new Document { Id = 1, CompanyId = 1, Title = "Existing", Description = "Existing", FileUrl = "http://example.com/existing", CreatedAt = DateTime.UtcNow };
        _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(1)).ReturnsAsync(existingDocument);
        _mockDocumentService.Setup(s => s.DeleteDocumentAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteDocument(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    /// <summary>
    /// Tests that DeleteDocument returns NotFound when document does not exist.
    /// </summary>
    [Fact]
    public async Task DeleteDocument_ReturnsNotFound_WhenDocumentDoesNotExist()
    {
        // Arrange
        SetupUserWithCompanyId(1);
        _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(999)).ReturnsAsync((Document?)null);

        // Act
        var result = await _controller.DeleteDocument(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    /// <summary>
    /// Tests that DeleteDocument returns Forbid when document belongs to different company.
    /// </summary>
    [Fact]
    public async Task DeleteDocument_ReturnsForbid_WhenDocumentBelongsToDifferentCompany()
    {
        // Arrange
        SetupUserWithCompanyId(1);
        var existingDocument = new Document { Id = 1, CompanyId = 2, Title = "Existing", Description = "Existing", FileUrl = "http://example.com/existing", CreatedAt = DateTime.UtcNow };
        _mockDocumentService.Setup(s => s.GetDocumentByIdAsync(1)).ReturnsAsync(existingDocument);

        // Act
        var result = await _controller.DeleteDocument(1);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    /// <summary>
    /// Tests that DeleteDocument returns BadRequest when company ID is missing from claims.
    /// </summary>
    [Fact]
    public async Task DeleteDocument_ReturnsBadRequest_WhenCompanyIdMissing()
    {
        // Arrange
        SetupUserWithoutCompanyId();

        // Act
        var result = await _controller.DeleteDocument(1);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    #endregion
}
