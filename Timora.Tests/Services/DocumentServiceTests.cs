using Moq;
using Timora.Api.Repositories;
using Timora.Api.Services;
using Timora.Data.Entities;

namespace Timora.Tests.Services;

/// <summary>
/// Unit tests for the DocumentService.
/// </summary>
public class DocumentServiceTests
{
    private readonly Mock<IDocumentRepository> _mockDocumentRepository;
    private readonly DocumentService _documentService;

    public DocumentServiceTests()
    {
        _mockDocumentRepository = new Mock<IDocumentRepository>();
        _documentService = new DocumentService(_mockDocumentRepository.Object);
    }

    #region GetDocumentsByCompanyIdAsync Tests

    /// <summary>
    /// Tests that GetDocumentsByCompanyIdAsync returns all documents for the company.
    /// </summary>
    [Fact]
    public async Task GetDocumentsByCompanyIdAsync_ReturnsAllDocuments_WhenDocumentsExist()
    {
        // Arrange
        var documents = new List<Document>
        {
            new Document { Id = 1, CompanyId = 1, Title = "Employee Handbook", Description = "Company policies", FileUrl = "http://example.com/handbook", CreatedAt = DateTime.UtcNow },
            new Document { Id = 2, CompanyId = 1, Title = "Safety Guidelines", Description = "Safety procedures", FileUrl = "http://example.com/safety", CreatedAt = DateTime.UtcNow }
        };
        _mockDocumentRepository.Setup(r => r.GetDocumentsByCompanyIdAsync(1)).ReturnsAsync(documents);

        // Act
        var result = await _documentService.GetDocumentsByCompanyIdAsync(1);

        // Assert
        Assert.Equal(2, result.Count());
        _mockDocumentRepository.Verify(r => r.GetDocumentsByCompanyIdAsync(1), Times.Once);
    }

    /// <summary>
    /// Tests that GetDocumentsByCompanyIdAsync returns empty collection when no documents exist.
    /// </summary>
    [Fact]
    public async Task GetDocumentsByCompanyIdAsync_ReturnsEmptyCollection_WhenNoDocumentsExist()
    {
        // Arrange
        _mockDocumentRepository.Setup(r => r.GetDocumentsByCompanyIdAsync(1)).ReturnsAsync(new List<Document>());

        // Act
        var result = await _documentService.GetDocumentsByCompanyIdAsync(1);

        // Assert
        Assert.Empty(result);
        _mockDocumentRepository.Verify(r => r.GetDocumentsByCompanyIdAsync(1), Times.Once);
    }

    #endregion

    #region GetDocumentByIdAsync Tests

    /// <summary>
    /// Tests that GetDocumentByIdAsync returns document when found.
    /// </summary>
    [Fact]
    public async Task GetDocumentByIdAsync_ReturnsDocument_WhenDocumentExists()
    {
        // Arrange
        var document = new Document { Id = 1, CompanyId = 1, Title = "Employee Handbook", Description = "Company policies", FileUrl = "http://example.com/handbook", CreatedAt = DateTime.UtcNow };
        _mockDocumentRepository.Setup(r => r.GetDocumentByIdAsync(1)).ReturnsAsync(document);

        // Act
        var result = await _documentService.GetDocumentByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Employee Handbook", result.Title);
        _mockDocumentRepository.Verify(r => r.GetDocumentByIdAsync(1), Times.Once);
    }

    /// <summary>
    /// Tests that GetDocumentByIdAsync returns null when document not found.
    /// </summary>
    [Fact]
    public async Task GetDocumentByIdAsync_ReturnsNull_WhenDocumentDoesNotExist()
    {
        // Arrange
        _mockDocumentRepository.Setup(r => r.GetDocumentByIdAsync(999)).ReturnsAsync((Document?)null);

        // Act
        var result = await _documentService.GetDocumentByIdAsync(999);

        // Assert
        Assert.Null(result);
        _mockDocumentRepository.Verify(r => r.GetDocumentByIdAsync(999), Times.Once);
    }

    #endregion

    #region CreateDocumentAsync Tests

    /// <summary>
    /// Tests that CreateDocumentAsync returns created document.
    /// </summary>
    [Fact]
    public async Task CreateDocumentAsync_ReturnsCreatedDocument()
    {
        // Arrange
        var newDocument = new Document { CompanyId = 1, Title = "New Document", Description = "New Description", FileUrl = "http://example.com/new" };
        var createdDocument = new Document { Id = 1, CompanyId = 1, Title = "New Document", Description = "New Description", FileUrl = "http://example.com/new", CreatedAt = DateTime.UtcNow };
        _mockDocumentRepository.Setup(r => r.CreateDocumentAsync(newDocument)).ReturnsAsync(createdDocument);

        // Act
        var result = await _documentService.CreateDocumentAsync(newDocument);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("New Document", result.Title);
        _mockDocumentRepository.Verify(r => r.CreateDocumentAsync(newDocument), Times.Once);
    }

    #endregion

    #region UpdateDocumentAsync Tests

    /// <summary>
    /// Tests that UpdateDocumentAsync returns updated document when successful.
    /// </summary>
    [Fact]
    public async Task UpdateDocumentAsync_ReturnsUpdatedDocument_WhenSuccessful()
    {
        // Arrange
        var documentUpdate = new Document { Title = "Updated Title", Description = "Updated Description", FileUrl = "http://example.com/updated" };
        var updatedDocument = new Document { Id = 1, CompanyId = 1, Title = "Updated Title", Description = "Updated Description", FileUrl = "http://example.com/updated", CreatedAt = DateTime.UtcNow };
        _mockDocumentRepository.Setup(r => r.UpdateDocumentAsync(1, documentUpdate)).ReturnsAsync(updatedDocument);

        // Act
        var result = await _documentService.UpdateDocumentAsync(1, documentUpdate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Title", result.Title);
        _mockDocumentRepository.Verify(r => r.UpdateDocumentAsync(1, documentUpdate), Times.Once);
    }

    /// <summary>
    /// Tests that UpdateDocumentAsync returns null when document not found.
    /// </summary>
    [Fact]
    public async Task UpdateDocumentAsync_ReturnsNull_WhenDocumentDoesNotExist()
    {
        // Arrange
        var documentUpdate = new Document { Title = "Updated Title", Description = "Updated Description", FileUrl = "http://example.com/updated" };
        _mockDocumentRepository.Setup(r => r.UpdateDocumentAsync(999, documentUpdate)).ReturnsAsync((Document?)null);

        // Act
        var result = await _documentService.UpdateDocumentAsync(999, documentUpdate);

        // Assert
        Assert.Null(result);
        _mockDocumentRepository.Verify(r => r.UpdateDocumentAsync(999, documentUpdate), Times.Once);
    }

    #endregion

    #region DeleteDocumentAsync Tests

    /// <summary>
    /// Tests that DeleteDocumentAsync returns true when deletion successful.
    /// </summary>
    [Fact]
    public async Task DeleteDocumentAsync_ReturnsTrue_WhenDeletionSuccessful()
    {
        // Arrange
        _mockDocumentRepository.Setup(r => r.DeleteDocumentAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _documentService.DeleteDocumentAsync(1);

        // Assert
        Assert.True(result);
        _mockDocumentRepository.Verify(r => r.DeleteDocumentAsync(1), Times.Once);
    }

    /// <summary>
    /// Tests that DeleteDocumentAsync returns false when document not found.
    /// </summary>
    [Fact]
    public async Task DeleteDocumentAsync_ReturnsFalse_WhenDocumentDoesNotExist()
    {
        // Arrange
        _mockDocumentRepository.Setup(r => r.DeleteDocumentAsync(999)).ReturnsAsync(false);

        // Act
        var result = await _documentService.DeleteDocumentAsync(999);

        // Assert
        Assert.False(result);
        _mockDocumentRepository.Verify(r => r.DeleteDocumentAsync(999), Times.Once);
    }

    #endregion
}
