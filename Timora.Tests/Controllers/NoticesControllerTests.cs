using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Timora.Api.Controllers;
using Timora.Api.DTOs;
using Timora.Api.Services;
using Timora.Data.Entities;

namespace Timora.Tests.Controllers;

/// <summary>
/// Unit tests for the NoticesController.
/// </summary>
public class NoticesControllerTests
{
    private readonly Mock<INoticeService> _mockNoticeService;
    private readonly Mock<ILogger<NoticesController>> _mockLogger;
    private readonly NoticesController _controller;

    public NoticesControllerTests()
    {
        _mockNoticeService = new Mock<INoticeService>();
        _mockLogger = new Mock<ILogger<NoticesController>>();
        _controller = new NoticesController(_mockNoticeService.Object, _mockLogger.Object);
    }

    #region GetAllNotices Tests

    /// <summary>
    /// Tests that GetAllNotices returns OK with a list of notices.
    /// </summary>
    [Fact]
    public async Task GetAllNotices_ReturnsOk_WithListOfNotices()
    {
        // Arrange
        var notices = new List<Notice>
        {
            new Notice { Id = 1, UserId = 1, Title = "Notice 1", Content = "Content 1", CreatedAt = DateTime.UtcNow },
            new Notice { Id = 2, UserId = 1, Title = "Notice 2", Content = "Content 2", CreatedAt = DateTime.UtcNow }
        };
        _mockNoticeService.Setup(s => s.GetAllNoticesAsync()).ReturnsAsync(notices);

        // Act
        var result = await _controller.GetAllNotices();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedNotices = Assert.IsAssignableFrom<IEnumerable<Notice>>(okResult.Value);
        Assert.Equal(2, returnedNotices.Count());
    }

    /// <summary>
    /// Tests that GetAllNotices returns OK with empty list when no notices exist.
    /// </summary>
    [Fact]
    public async Task GetAllNotices_ReturnsOk_WithEmptyList_WhenNoNoticesExist()
    {
        // Arrange
        _mockNoticeService.Setup(s => s.GetAllNoticesAsync()).ReturnsAsync(new List<Notice>());

        // Act
        var result = await _controller.GetAllNotices();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedNotices = Assert.IsAssignableFrom<IEnumerable<Notice>>(okResult.Value);
        Assert.Empty(returnedNotices);
    }

    #endregion

    #region GetNoticeById Tests

    /// <summary>
    /// Tests that GetNoticeById returns OK when notice exists.
    /// </summary>
    [Fact]
    public async Task GetNoticeById_ReturnsOk_WhenNoticeExists()
    {
        // Arrange
        var notice = new Notice { Id = 1, UserId = 1, Title = "Test Notice", Content = "Test Content", CreatedAt = DateTime.UtcNow };
        _mockNoticeService.Setup(s => s.GetNoticeByIdAsync(1)).ReturnsAsync(notice);

        // Act
        var result = await _controller.GetNoticeById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedNotice = Assert.IsType<Notice>(okResult.Value);
        Assert.Equal(1, returnedNotice.Id);
        Assert.Equal("Test Notice", returnedNotice.Title);
    }

    /// <summary>
    /// Tests that GetNoticeById returns NotFound when notice does not exist.
    /// </summary>
    [Fact]
    public async Task GetNoticeById_ReturnsNotFound_WhenNoticeDoesNotExist()
    {
        // Arrange
        _mockNoticeService.Setup(s => s.GetNoticeByIdAsync(999)).ReturnsAsync((Notice?)null);

        // Act
        var result = await _controller.GetNoticeById(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    #endregion

    #region CreateNotice Tests

    /// <summary>
    /// Tests that CreateNotice returns Created when successful.
    /// </summary>
    [Fact]
    public async Task CreateNotice_ReturnsCreated_WhenSuccessful()
    {
        // Arrange
        var createDto = new CreateNoticeDto { UserId = 1, Title = "New Notice", Content = "New Content" };
        var createdNotice = new Notice { Id = 1, UserId = 1, Title = "New Notice", Content = "New Content", CreatedAt = DateTime.UtcNow };
        _mockNoticeService.Setup(s => s.CreateNoticeAsync(It.IsAny<Notice>())).ReturnsAsync(createdNotice);

        // Act
        var result = await _controller.CreateNotice(createDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.GetNoticeById), createdResult.ActionName);
        var returnedNotice = Assert.IsType<Notice>(createdResult.Value);
        Assert.Equal(1, returnedNotice.Id);
    }

    /// <summary>
    /// Tests that CreateNotice returns BadRequest when exception is thrown.
    /// </summary>
    [Fact]
    public async Task CreateNotice_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        var createDto = new CreateNoticeDto { UserId = 1, Title = "New Notice", Content = "New Content" };
        _mockNoticeService.Setup(s => s.CreateNoticeAsync(It.IsAny<Notice>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.CreateNotice(createDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    #endregion

    #region DeleteNotice Tests

    /// <summary>
    /// Tests that DeleteNotice returns NoContent when successful.
    /// </summary>
    [Fact]
    public async Task DeleteNotice_ReturnsNoContent_WhenSuccessful()
    {
        // Arrange
        _mockNoticeService.Setup(s => s.DeleteNoticeAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteNotice(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    /// <summary>
    /// Tests that DeleteNotice returns NotFound when notice does not exist.
    /// </summary>
    [Fact]
    public async Task DeleteNotice_ReturnsNotFound_WhenNoticeDoesNotExist()
    {
        // Arrange
        _mockNoticeService.Setup(s => s.DeleteNoticeAsync(999)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteNotice(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    #endregion

    #region UpdateNotice Tests

    /// <summary>
    /// Tests that UpdateNotice returns OK when successful.
    /// </summary>
    [Fact]
    public async Task UpdateNotice_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var updateDto = new UpdateNoticeDto { Title = "Updated Notice", Content = "Updated Content" };
        var updatedNotice = new Notice { Id = 1, UserId = 1, Title = "Updated Notice", Content = "Updated Content", CreatedAt = DateTime.UtcNow };
        _mockNoticeService.Setup(s => s.UpdateNoticeAsync(1, It.IsAny<Notice>())).ReturnsAsync(updatedNotice);

        // Act
        var result = await _controller.UpdateNotice(1, updateDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedNotice = Assert.IsType<Notice>(okResult.Value);
        Assert.Equal("Updated Notice", returnedNotice.Title);
    }

    /// <summary>
    /// Tests that UpdateNotice returns NotFound when notice does not exist.
    /// </summary>
    [Fact]
    public async Task UpdateNotice_ReturnsNotFound_WhenNoticeDoesNotExist()
    {
        // Arrange
        var updateDto = new UpdateNoticeDto { Title = "Updated Notice" };
        _mockNoticeService.Setup(s => s.UpdateNoticeAsync(999, It.IsAny<Notice>())).ReturnsAsync((Notice?)null);

        // Act
        var result = await _controller.UpdateNotice(999, updateDto);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    /// <summary>
    /// Tests that UpdateNotice returns BadRequest when exception is thrown.
    /// </summary>
    [Fact]
    public async Task UpdateNotice_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        var updateDto = new UpdateNoticeDto { Title = "Updated Notice" };
        _mockNoticeService.Setup(s => s.UpdateNoticeAsync(1, It.IsAny<Notice>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.UpdateNotice(1, updateDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    #endregion
}
