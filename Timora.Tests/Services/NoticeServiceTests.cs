using Moq;
using Timora.Api.Repositories;
using Timora.Api.Services;
using Timora.Data.Entities;

namespace Timora.Tests.Services;

/// <summary>
/// Unit tests for the NoticeService.
/// </summary>
public class NoticeServiceTests
{
    private readonly Mock<INoticeRepository> _mockNoticeRepository;
    private readonly NoticeService _noticeService;

    public NoticeServiceTests()
    {
        _mockNoticeRepository = new Mock<INoticeRepository>();
        _noticeService = new NoticeService(_mockNoticeRepository.Object);
    }

    #region GetAllNoticesAsync Tests

    /// <summary>
    /// Tests that GetAllNoticesAsync returns all notices from the repository.
    /// </summary>
    [Fact]
    public async Task GetAllNoticesAsync_ReturnsAllNotices_WhenNoticesExist()
    {
        // Arrange
        var notices = new List<Notice>
        {
            new Notice { Id = 1, UserId = 1, Title = "Office Closure", Content = "Office will be closed on Friday.", CreatedAt = DateTime.UtcNow },
            new Notice { Id = 2, UserId = 1, Title = "Team Meeting", Content = "Team meeting scheduled for Monday.", CreatedAt = DateTime.UtcNow }
        };
        _mockNoticeRepository.Setup(r => r.GetAllNoticesAsync()).ReturnsAsync(notices);

        // Act
        var result = await _noticeService.GetAllNoticesAsync();

        // Assert
        Assert.Equal(2, result.Count());
        _mockNoticeRepository.Verify(r => r.GetAllNoticesAsync(), Times.Once);
    }

    /// <summary>
    /// Tests that GetAllNoticesAsync returns empty collection when no notices exist.
    /// </summary>
    [Fact]
    public async Task GetAllNoticesAsync_ReturnsEmptyCollection_WhenNoNoticesExist()
    {
        // Arrange
        _mockNoticeRepository.Setup(r => r.GetAllNoticesAsync()).ReturnsAsync(new List<Notice>());

        // Act
        var result = await _noticeService.GetAllNoticesAsync();

        // Assert
        Assert.Empty(result);
        _mockNoticeRepository.Verify(r => r.GetAllNoticesAsync(), Times.Once);
    }

    #endregion

    #region GetNoticeByIdAsync Tests

    /// <summary>
    /// Tests that GetNoticeByIdAsync returns notice when found.
    /// </summary>
    [Fact]
    public async Task GetNoticeByIdAsync_ReturnsNotice_WhenNoticeExists()
    {
        // Arrange
        var notice = new Notice { Id = 1, UserId = 1, Title = "Office Closure", Content = "Office will be closed on Friday.", CreatedAt = DateTime.UtcNow };
        _mockNoticeRepository.Setup(r => r.GetNoticeByIdAsync(1)).ReturnsAsync(notice);

        // Act
        var result = await _noticeService.GetNoticeByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Office Closure", result.Title);
        _mockNoticeRepository.Verify(r => r.GetNoticeByIdAsync(1), Times.Once);
    }

    /// <summary>
    /// Tests that GetNoticeByIdAsync returns null when notice not found.
    /// </summary>
    [Fact]
    public async Task GetNoticeByIdAsync_ReturnsNull_WhenNoticeDoesNotExist()
    {
        // Arrange
        _mockNoticeRepository.Setup(r => r.GetNoticeByIdAsync(999)).ReturnsAsync((Notice?)null);

        // Act
        var result = await _noticeService.GetNoticeByIdAsync(999);

        // Assert
        Assert.Null(result);
        _mockNoticeRepository.Verify(r => r.GetNoticeByIdAsync(999), Times.Once);
    }

    #endregion

    #region CreateNoticeAsync Tests

    /// <summary>
    /// Tests that CreateNoticeAsync creates and returns new notice.
    /// </summary>
    [Fact]
    public async Task CreateNoticeAsync_ReturnsCreatedNotice_WhenSuccessful()
    {
        // Arrange
        var newNotice = new Notice { UserId = 1, Title = "New Policy", Content = "New company policy effective immediately.", CreatedAt = DateTime.UtcNow };
        var createdNotice = new Notice { Id = 3, UserId = 1, Title = "New Policy", Content = "New company policy effective immediately.", CreatedAt = DateTime.UtcNow };
        _mockNoticeRepository.Setup(r => r.CreateNoticeAsync(It.IsAny<Notice>())).ReturnsAsync(createdNotice);

        // Act
        var result = await _noticeService.CreateNoticeAsync(newNotice);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Id);
        Assert.Equal("New Policy", result.Title);
        _mockNoticeRepository.Verify(r => r.CreateNoticeAsync(It.IsAny<Notice>()), Times.Once);
    }

    #endregion

    #region DeleteNoticeAsync Tests

    /// <summary>
    /// Tests that DeleteNoticeAsync returns true when notice deleted successfully.
    /// </summary>
    [Fact]
    public async Task DeleteNoticeAsync_ReturnsTrue_WhenNoticeDeleted()
    {
        // Arrange
        _mockNoticeRepository.Setup(r => r.DeleteNoticeAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _noticeService.DeleteNoticeAsync(1);

        // Assert
        Assert.True(result);
        _mockNoticeRepository.Verify(r => r.DeleteNoticeAsync(1), Times.Once);
    }

    /// <summary>
    /// Tests that DeleteNoticeAsync returns false when notice not found.
    /// </summary>
    [Fact]
    public async Task DeleteNoticeAsync_ReturnsFalse_WhenNoticeNotFound()
    {
        // Arrange
        _mockNoticeRepository.Setup(r => r.DeleteNoticeAsync(999)).ReturnsAsync(false);

        // Act
        var result = await _noticeService.DeleteNoticeAsync(999);

        // Assert
        Assert.False(result);
        _mockNoticeRepository.Verify(r => r.DeleteNoticeAsync(999), Times.Once);
    }

    #endregion

    #region UpdateNoticeAsync Tests

    /// <summary>
    /// Tests that UpdateNoticeAsync returns updated notice when successful.
    /// </summary>
    [Fact]
    public async Task UpdateNoticeAsync_ReturnsUpdatedNotice_WhenNoticeExists()
    {
        // Arrange
        var updateNotice = new Notice { Title = "Updated Office Closure", Content = "Office closure has been rescheduled." };
        var updatedNotice = new Notice { Id = 1, UserId = 1, Title = "Updated Office Closure", Content = "Office closure has been rescheduled.", CreatedAt = DateTime.UtcNow };
        _mockNoticeRepository.Setup(r => r.UpdateNoticeAsync(1, It.IsAny<Notice>())).ReturnsAsync(updatedNotice);

        // Act
        var result = await _noticeService.UpdateNoticeAsync(1, updateNotice);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Updated Office Closure", result.Title);
        Assert.Equal("Office closure has been rescheduled.", result.Content);
        _mockNoticeRepository.Verify(r => r.UpdateNoticeAsync(1, It.IsAny<Notice>()), Times.Once);
    }

    /// <summary>
    /// Tests that UpdateNoticeAsync returns null when notice not found.
    /// </summary>
    [Fact]
    public async Task UpdateNoticeAsync_ReturnsNull_WhenNoticeNotFound()
    {
        // Arrange
        var updateNotice = new Notice { Title = "Updated Notice", Content = "Updated content." };
        _mockNoticeRepository.Setup(r => r.UpdateNoticeAsync(999, It.IsAny<Notice>())).ReturnsAsync((Notice?)null);

        // Act
        var result = await _noticeService.UpdateNoticeAsync(999, updateNotice);

        // Assert
        Assert.Null(result);
        _mockNoticeRepository.Verify(r => r.UpdateNoticeAsync(999, It.IsAny<Notice>()), Times.Once);
    }

    #endregion
}
