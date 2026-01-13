using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Timora.Api.Controllers;
using Timora.Api.DTOs;
using Timora.Api.Services;
using Timora.Data.Entities;

namespace Timora.Tests.Controllers;

/// <summary>
/// Unit tests for the HolidayRequestsController.
/// </summary>
public class HolidayRequestsControllerTests
{
    private readonly Mock<IHolidayRequestService> _mockHolidayRequestService;
    private readonly Mock<ILogger<HolidayRequestsController>> _mockLogger;
    private readonly HolidayRequestsController _controller;

    public HolidayRequestsControllerTests()
    {
        _mockHolidayRequestService = new Mock<IHolidayRequestService>();
        _mockLogger = new Mock<ILogger<HolidayRequestsController>>();
        _controller = new HolidayRequestsController(_mockHolidayRequestService.Object, _mockLogger.Object);
    }

    #region GetAllHolidayRequests Tests

    /// <summary>
    /// Tests that GetAllHolidayRequests returns OK with a list of requests.
    /// </summary>
    [Fact]
    public async Task GetAllHolidayRequests_ReturnsOk_WithListOfRequests()
    {
        // Arrange
        var requests = new List<HolidayRequest>
        {
            new HolidayRequest { Id = 1, UserId = 1, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(5), Reason = "Vacation", Status = HolidayRequestStatus.Pending },
            new HolidayRequest { Id = 2, UserId = 2, StartDate = DateTime.Today.AddDays(10), EndDate = DateTime.Today.AddDays(15), Reason = "Family event", Status = HolidayRequestStatus.Approved }
        };
        _mockHolidayRequestService.Setup(s => s.GetAllHolidayRequestsAsync()).ReturnsAsync(requests);

        // Act
        var result = await _controller.GetAllHolidayRequests();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedRequests = Assert.IsAssignableFrom<IEnumerable<HolidayRequest>>(okResult.Value);
        Assert.Equal(2, returnedRequests.Count());
    }

    #endregion

    #region GetHolidayRequestById Tests

    /// <summary>
    /// Tests that GetHolidayRequestById returns OK when request exists.
    /// </summary>
    [Fact]
    public async Task GetHolidayRequestById_ReturnsOk_WhenRequestExists()
    {
        // Arrange
        var request = new HolidayRequest { Id = 1, UserId = 1, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(5), Reason = "Vacation", Status = HolidayRequestStatus.Pending };
        _mockHolidayRequestService.Setup(s => s.GetHolidayRequestByIdAsync(1)).ReturnsAsync(request);

        // Act
        var result = await _controller.GetHolidayRequestById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedRequest = Assert.IsType<HolidayRequest>(okResult.Value);
        Assert.Equal(1, returnedRequest.Id);
    }

    /// <summary>
    /// Tests that GetHolidayRequestById returns NotFound when request does not exist.
    /// </summary>
    [Fact]
    public async Task GetHolidayRequestById_ReturnsNotFound_WhenRequestDoesNotExist()
    {
        // Arrange
        _mockHolidayRequestService.Setup(s => s.GetHolidayRequestByIdAsync(999)).ReturnsAsync((HolidayRequest?)null);

        // Act
        var result = await _controller.GetHolidayRequestById(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    #endregion

    #region CreateHolidayRequest Tests

    /// <summary>
    /// Tests that CreateHolidayRequest returns Created when successful.
    /// </summary>
    [Fact]
    public async Task CreateHolidayRequest_ReturnsCreated_WhenSuccessful()
    {
        // Arrange
        var createDto = new CreateHolidayRequestDto 
        { 
            UserId = 1, 
            StartDate = DateTime.Today, 
            EndDate = DateTime.Today.AddDays(5), 
            Reason = "Vacation" 
        };
        var createdRequest = new HolidayRequest { Id = 1, UserId = 1, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(5), Reason = "Vacation", Status = HolidayRequestStatus.Pending };
        _mockHolidayRequestService.Setup(s => s.CreateHolidayRequestAsync(It.IsAny<HolidayRequest>())).ReturnsAsync(createdRequest);

        // Act
        var result = await _controller.CreateHolidayRequest(createDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.GetHolidayRequestById), createdResult.ActionName);
    }

    /// <summary>
    /// Tests that CreateHolidayRequest returns BadRequest when end date is before start date.
    /// </summary>
    [Fact]
    public async Task CreateHolidayRequest_ReturnsBadRequest_WhenEndDateBeforeStartDate()
    {
        // Arrange
        var createDto = new CreateHolidayRequestDto 
        { 
            UserId = 1, 
            StartDate = DateTime.Today.AddDays(5), 
            EndDate = DateTime.Today, // End before start
            Reason = "Vacation" 
        };

        // Act
        var result = await _controller.CreateHolidayRequest(createDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    /// <summary>
    /// Tests that CreateHolidayRequest returns BadRequest when exception is thrown.
    /// </summary>
    [Fact]
    public async Task CreateHolidayRequest_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        var createDto = new CreateHolidayRequestDto 
        { 
            UserId = 1, 
            StartDate = DateTime.Today, 
            EndDate = DateTime.Today.AddDays(5), 
            Reason = "Vacation" 
        };
        _mockHolidayRequestService.Setup(s => s.CreateHolidayRequestAsync(It.IsAny<HolidayRequest>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.CreateHolidayRequest(createDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    #endregion

    #region DeleteHolidayRequest Tests

    /// <summary>
    /// Tests that DeleteHolidayRequest returns NoContent when successful.
    /// </summary>
    [Fact]
    public async Task DeleteHolidayRequest_ReturnsNoContent_WhenSuccessful()
    {
        // Arrange
        _mockHolidayRequestService.Setup(s => s.DeleteHolidayRequestAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteHolidayRequest(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    /// <summary>
    /// Tests that DeleteHolidayRequest returns NotFound when request does not exist.
    /// </summary>
    [Fact]
    public async Task DeleteHolidayRequest_ReturnsNotFound_WhenRequestDoesNotExist()
    {
        // Arrange
        _mockHolidayRequestService.Setup(s => s.DeleteHolidayRequestAsync(999)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteHolidayRequest(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    #endregion

    #region UpdateHolidayRequest Tests

    /// <summary>
    /// Tests that UpdateHolidayRequest returns OK when successful.
    /// </summary>
    [Fact]
    public async Task UpdateHolidayRequest_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var updateDto = new UpdateHolidayRequestDto { Status = HolidayRequestStatus.Approved };
        var updatedRequest = new HolidayRequest { Id = 1, UserId = 1, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(5), Reason = "Vacation", Status = HolidayRequestStatus.Approved };
        _mockHolidayRequestService.Setup(s => s.UpdateHolidayRequestAsync(1, It.IsAny<HolidayRequest>())).ReturnsAsync(updatedRequest);

        // Act
        var result = await _controller.UpdateHolidayRequest(1, updateDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedRequest = Assert.IsType<HolidayRequest>(okResult.Value);
        Assert.Equal(HolidayRequestStatus.Approved, returnedRequest.Status);
    }

    /// <summary>
    /// Tests that UpdateHolidayRequest returns BadRequest when end date is before start date.
    /// </summary>
    [Fact]
    public async Task UpdateHolidayRequest_ReturnsBadRequest_WhenEndDateBeforeStartDate()
    {
        // Arrange
        var updateDto = new UpdateHolidayRequestDto 
        { 
            StartDate = DateTime.Today.AddDays(5), 
            EndDate = DateTime.Today // End before start
        };

        // Act
        var result = await _controller.UpdateHolidayRequest(1, updateDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    /// <summary>
    /// Tests that UpdateHolidayRequest returns NotFound when request does not exist.
    /// </summary>
    [Fact]
    public async Task UpdateHolidayRequest_ReturnsNotFound_WhenRequestDoesNotExist()
    {
        // Arrange
        var updateDto = new UpdateHolidayRequestDto { Status = HolidayRequestStatus.Approved };
        _mockHolidayRequestService.Setup(s => s.UpdateHolidayRequestAsync(999, It.IsAny<HolidayRequest>())).ReturnsAsync((HolidayRequest?)null);

        // Act
        var result = await _controller.UpdateHolidayRequest(999, updateDto);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    /// <summary>
    /// Tests that UpdateHolidayRequest returns BadRequest when exception is thrown.
    /// </summary>
    [Fact]
    public async Task UpdateHolidayRequest_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        var updateDto = new UpdateHolidayRequestDto { Status = HolidayRequestStatus.Approved };
        _mockHolidayRequestService.Setup(s => s.UpdateHolidayRequestAsync(1, It.IsAny<HolidayRequest>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.UpdateHolidayRequest(1, updateDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    #endregion
}
