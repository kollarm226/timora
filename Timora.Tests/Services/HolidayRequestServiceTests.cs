using Moq;
using Timora.Api.Repositories;
using Timora.Api.Services;
using Timora.Data.Entities;

namespace Timora.Tests.Services;

/// <summary>
/// Unit tests for the HolidayRequestService.
/// </summary>
public class HolidayRequestServiceTests
{
    private readonly Mock<IHolidayRequestRepository> _mockHolidayRequestRepository;
    private readonly HolidayRequestService _holidayRequestService;

    public HolidayRequestServiceTests()
    {
        _mockHolidayRequestRepository = new Mock<IHolidayRequestRepository>();
        _holidayRequestService = new HolidayRequestService(_mockHolidayRequestRepository.Object);
    }

    #region GetAllHolidayRequestsAsync Tests

    /// <summary>
    /// Tests that GetAllHolidayRequestsAsync returns all holiday requests from the repository.
    /// </summary>
    [Fact]
    public async Task GetAllHolidayRequestsAsync_ReturnsAllHolidayRequests_WhenHolidayRequestsExist()
    {
        // Arrange
        var holidayRequests = new List<HolidayRequest>
        {
            new HolidayRequest { Id = 1, UserId = 1, StartDate = DateTime.UtcNow.AddDays(7), EndDate = DateTime.UtcNow.AddDays(14), Status = HolidayRequestStatus.Pending, Reason = "Vacation" },
            new HolidayRequest { Id = 2, UserId = 2, StartDate = DateTime.UtcNow.AddDays(30), EndDate = DateTime.UtcNow.AddDays(37), Status = HolidayRequestStatus.Approved, Reason = "Family trip" }
        };
        _mockHolidayRequestRepository.Setup(r => r.GetAllHolidayRequestsAsync()).ReturnsAsync(holidayRequests);

        // Act
        var result = await _holidayRequestService.GetAllHolidayRequestsAsync();

        // Assert
        Assert.Equal(2, result.Count());
        _mockHolidayRequestRepository.Verify(r => r.GetAllHolidayRequestsAsync(), Times.Once);
    }

    /// <summary>
    /// Tests that GetAllHolidayRequestsAsync returns empty collection when no holiday requests exist.
    /// </summary>
    [Fact]
    public async Task GetAllHolidayRequestsAsync_ReturnsEmptyCollection_WhenNoHolidayRequestsExist()
    {
        // Arrange
        _mockHolidayRequestRepository.Setup(r => r.GetAllHolidayRequestsAsync()).ReturnsAsync(new List<HolidayRequest>());

        // Act
        var result = await _holidayRequestService.GetAllHolidayRequestsAsync();

        // Assert
        Assert.Empty(result);
        _mockHolidayRequestRepository.Verify(r => r.GetAllHolidayRequestsAsync(), Times.Once);
    }

    #endregion

    #region GetHolidayRequestByIdAsync Tests

    /// <summary>
    /// Tests that GetHolidayRequestByIdAsync returns holiday request when found.
    /// </summary>
    [Fact]
    public async Task GetHolidayRequestByIdAsync_ReturnsHolidayRequest_WhenHolidayRequestExists()
    {
        // Arrange
        var holidayRequest = new HolidayRequest { Id = 1, UserId = 1, StartDate = DateTime.UtcNow.AddDays(7), EndDate = DateTime.UtcNow.AddDays(14), Status = HolidayRequestStatus.Pending, Reason = "Vacation" };
        _mockHolidayRequestRepository.Setup(r => r.GetHolidayRequestByIdAsync(1)).ReturnsAsync(holidayRequest);

        // Act
        var result = await _holidayRequestService.GetHolidayRequestByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(HolidayRequestStatus.Pending, result.Status);
        _mockHolidayRequestRepository.Verify(r => r.GetHolidayRequestByIdAsync(1), Times.Once);
    }

    /// <summary>
    /// Tests that GetHolidayRequestByIdAsync returns null when holiday request not found.
    /// </summary>
    [Fact]
    public async Task GetHolidayRequestByIdAsync_ReturnsNull_WhenHolidayRequestDoesNotExist()
    {
        // Arrange
        _mockHolidayRequestRepository.Setup(r => r.GetHolidayRequestByIdAsync(999)).ReturnsAsync((HolidayRequest?)null);

        // Act
        var result = await _holidayRequestService.GetHolidayRequestByIdAsync(999);

        // Assert
        Assert.Null(result);
        _mockHolidayRequestRepository.Verify(r => r.GetHolidayRequestByIdAsync(999), Times.Once);
    }

    #endregion

    #region CreateHolidayRequestAsync Tests

    /// <summary>
    /// Tests that CreateHolidayRequestAsync creates and returns new holiday request.
    /// </summary>
    [Fact]
    public async Task CreateHolidayRequestAsync_ReturnsCreatedHolidayRequest_WhenSuccessful()
    {
        // Arrange
        var newHolidayRequest = new HolidayRequest { UserId = 1, StartDate = DateTime.UtcNow.AddDays(7), EndDate = DateTime.UtcNow.AddDays(14), Status = HolidayRequestStatus.Pending, Reason = "Personal leave" };
        var createdHolidayRequest = new HolidayRequest { Id = 3, UserId = 1, StartDate = DateTime.UtcNow.AddDays(7), EndDate = DateTime.UtcNow.AddDays(14), Status = HolidayRequestStatus.Pending, Reason = "Personal leave" };
        _mockHolidayRequestRepository.Setup(r => r.CreateHolidayRequestAsync(It.IsAny<HolidayRequest>())).ReturnsAsync(createdHolidayRequest);

        // Act
        var result = await _holidayRequestService.CreateHolidayRequestAsync(newHolidayRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Id);
        Assert.Equal("Personal leave", result.Reason);
        _mockHolidayRequestRepository.Verify(r => r.CreateHolidayRequestAsync(It.IsAny<HolidayRequest>()), Times.Once);
    }

    #endregion

    #region DeleteHolidayRequestAsync Tests

    /// <summary>
    /// Tests that DeleteHolidayRequestAsync returns true when holiday request deleted successfully.
    /// </summary>
    [Fact]
    public async Task DeleteHolidayRequestAsync_ReturnsTrue_WhenHolidayRequestDeleted()
    {
        // Arrange
        _mockHolidayRequestRepository.Setup(r => r.DeleteHolidayRequestAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _holidayRequestService.DeleteHolidayRequestAsync(1);

        // Assert
        Assert.True(result);
        _mockHolidayRequestRepository.Verify(r => r.DeleteHolidayRequestAsync(1), Times.Once);
    }

    /// <summary>
    /// Tests that DeleteHolidayRequestAsync returns false when holiday request not found.
    /// </summary>
    [Fact]
    public async Task DeleteHolidayRequestAsync_ReturnsFalse_WhenHolidayRequestNotFound()
    {
        // Arrange
        _mockHolidayRequestRepository.Setup(r => r.DeleteHolidayRequestAsync(999)).ReturnsAsync(false);

        // Act
        var result = await _holidayRequestService.DeleteHolidayRequestAsync(999);

        // Assert
        Assert.False(result);
        _mockHolidayRequestRepository.Verify(r => r.DeleteHolidayRequestAsync(999), Times.Once);
    }

    #endregion

    #region UpdateHolidayRequestAsync Tests

    /// <summary>
    /// Tests that UpdateHolidayRequestAsync returns updated holiday request when successful.
    /// </summary>
    [Fact]
    public async Task UpdateHolidayRequestAsync_ReturnsUpdatedHolidayRequest_WhenHolidayRequestExists()
    {
        // Arrange
        var updateHolidayRequest = new HolidayRequest { StartDate = DateTime.UtcNow.AddDays(10), EndDate = DateTime.UtcNow.AddDays(17), Status = HolidayRequestStatus.Approved, Reason = "Updated vacation" };
        var updatedHolidayRequest = new HolidayRequest { Id = 1, UserId = 1, StartDate = DateTime.UtcNow.AddDays(10), EndDate = DateTime.UtcNow.AddDays(17), Status = HolidayRequestStatus.Approved, Reason = "Updated vacation" };
        _mockHolidayRequestRepository.Setup(r => r.UpdateHolidayRequestAsync(1, It.IsAny<HolidayRequest>())).ReturnsAsync(updatedHolidayRequest);

        // Act
        var result = await _holidayRequestService.UpdateHolidayRequestAsync(1, updateHolidayRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(HolidayRequestStatus.Approved, result.Status);
        Assert.Equal("Updated vacation", result.Reason);
        _mockHolidayRequestRepository.Verify(r => r.UpdateHolidayRequestAsync(1, It.IsAny<HolidayRequest>()), Times.Once);
    }

    /// <summary>
    /// Tests that UpdateHolidayRequestAsync returns null when holiday request not found.
    /// </summary>
    [Fact]
    public async Task UpdateHolidayRequestAsync_ReturnsNull_WhenHolidayRequestNotFound()
    {
        // Arrange
        var updateHolidayRequest = new HolidayRequest { StartDate = DateTime.UtcNow.AddDays(10), EndDate = DateTime.UtcNow.AddDays(17), Status = HolidayRequestStatus.Approved, Reason = "Updated vacation" };
        _mockHolidayRequestRepository.Setup(r => r.UpdateHolidayRequestAsync(999, It.IsAny<HolidayRequest>())).ReturnsAsync((HolidayRequest?)null);

        // Act
        var result = await _holidayRequestService.UpdateHolidayRequestAsync(999, updateHolidayRequest);

        // Assert
        Assert.Null(result);
        _mockHolidayRequestRepository.Verify(r => r.UpdateHolidayRequestAsync(999, It.IsAny<HolidayRequest>()), Times.Once);
    }

    #endregion
}
