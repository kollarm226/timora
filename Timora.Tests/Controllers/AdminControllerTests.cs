using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Timora.Api.Controllers;
using Timora.Api.Services;
using Timora.Data.Entities;

namespace Timora.Tests.Controllers;

/// <summary>
/// Unit tests for the AdminController.
/// </summary>
public class AdminControllerTests
{
    private readonly Mock<ILogger<AdminController>> _mockLogger;
    private readonly Mock<IUserService> _mockUserService;
    private readonly AdminController _controller;

    public AdminControllerTests()
    {
        _mockLogger = new Mock<ILogger<AdminController>>();
        _mockUserService = new Mock<IUserService>();
        _controller = new AdminController(
            _mockLogger.Object,
            _mockUserService.Object
        );
    }

    #region GetPendingUsers Tests

    /// <summary>
    /// Tests that GetPendingUsers returns OK with list of pending users for employer.
    /// </summary>
    [Fact]
    public async Task GetPendingUsers_ReturnsOk_WhenEmployerRequestsPendingUsers()
    {
        // Arrange
        var employer = new User { Id = 1, Email = "employer@test.com", Role = UserRole.Employer, CompanyId = 10 };
        var pendingUsers = new List<User>
        {
            new User { Id = 2, Email = "pending1@test.com", FirstName = "John", LastName = "Doe", UserName = "johnd", Role = UserRole.Employee, CompanyId = 10, IsApproved = false },
            new User { Id = 3, Email = "pending2@test.com", FirstName = "Jane", LastName = "Smith", UserName = "janes", Role = UserRole.Employee, CompanyId = 10, IsApproved = false }
        };

        SetupControllerWithUser(employer);
        _mockUserService.Setup(s => s.GetUserByEmailAsync(employer.Email)).ReturnsAsync(employer);
        _mockUserService.Setup(s => s.GetPendingUsersAsync(10)).ReturnsAsync(pendingUsers);

        // Act
        var result = await _controller.GetPendingUsers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    /// <summary>
    /// Tests that GetPendingUsers returns Forbidden when non-employer requests.
    /// </summary>
    [Fact]
    public async Task GetPendingUsers_ReturnsForbidden_WhenNotEmployer()
    {
        // Arrange
        var employee = new User { Id = 1, Email = "employee@test.com", Role = UserRole.Employee, CompanyId = 10 };

        SetupControllerWithUser(employee);
        _mockUserService.Setup(s => s.GetUserByEmailAsync(employee.Email)).ReturnsAsync(employee);

        // Act
        var result = await _controller.GetPendingUsers();

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status403Forbidden, statusResult.StatusCode);
    }

    #endregion

    #region ApproveUser Tests

    /// <summary>
    /// Tests that ApproveUser returns OK when employer approves a user.
    /// </summary>
    [Fact]
    public async Task ApproveUser_ReturnsOk_WhenEmployerApprovesUser()
    {
        // Arrange
        var employer = new User { Id = 1, Email = "employer@test.com", Role = UserRole.Employer, CompanyId = 10 };
        var userToApprove = new User { Id = 2, Email = "pending@test.com", FirstName = "John", LastName = "Doe", UserName = "johnd", Role = UserRole.Employee, CompanyId = 10, IsApproved = false };
        var approvedUser = new User { Id = 2, Email = "pending@test.com", FirstName = "John", LastName = "Doe", UserName = "johnd", Role = UserRole.Employee, CompanyId = 10, IsApproved = true, ApprovedBy = 1, ApprovedAt = DateTime.UtcNow };

        SetupControllerWithUser(employer);
        _mockUserService.Setup(s => s.GetUserByEmailAsync(employer.Email)).ReturnsAsync(employer);
        _mockUserService.Setup(s => s.GetUserByIdAsync(2)).ReturnsAsync(userToApprove);
        _mockUserService.Setup(s => s.ApproveUserAsync(2, 1)).ReturnsAsync(approvedUser);

        // Act
        var result = await _controller.ApproveUser(2);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    /// <summary>
    /// Tests that ApproveUser returns NotFound when user doesn't exist.
    /// </summary>
    [Fact]
    public async Task ApproveUser_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var employer = new User { Id = 1, Email = "employer@test.com", Role = UserRole.Employer, CompanyId = 10 };

        SetupControllerWithUser(employer);
        _mockUserService.Setup(s => s.GetUserByEmailAsync(employer.Email)).ReturnsAsync(employer);
        _mockUserService.Setup(s => s.GetUserByIdAsync(999)).ReturnsAsync((User?)null);

        // Act
        var result = await _controller.ApproveUser(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    /// <summary>
    /// Tests that ApproveUser returns Forbidden when user is from different company.
    /// </summary>
    [Fact]
    public async Task ApproveUser_ReturnsForbidden_WhenUserFromDifferentCompany()
    {
        // Arrange
        var employer = new User { Id = 1, Email = "employer@test.com", Role = UserRole.Employer, CompanyId = 10 };
        var userFromOtherCompany = new User { Id = 2, Email = "other@test.com", CompanyId = 20, IsApproved = false };

        SetupControllerWithUser(employer);
        _mockUserService.Setup(s => s.GetUserByEmailAsync(employer.Email)).ReturnsAsync(employer);
        _mockUserService.Setup(s => s.GetUserByIdAsync(2)).ReturnsAsync(userFromOtherCompany);

        // Act
        var result = await _controller.ApproveUser(2);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status403Forbidden, statusResult.StatusCode);
    }

    /// <summary>
    /// Tests that ApproveUser returns Forbidden when caller is not employer.
    /// </summary>
    [Fact]
    public async Task ApproveUser_ReturnsForbidden_WhenNotEmployer()
    {
        // Arrange
        var employee = new User { Id = 1, Email = "employee@test.com", Role = UserRole.Employee, CompanyId = 10 };

        SetupControllerWithUser(employee);
        _mockUserService.Setup(s => s.GetUserByEmailAsync(employee.Email)).ReturnsAsync(employee);

        // Act
        var result = await _controller.ApproveUser(2);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status403Forbidden, statusResult.StatusCode);
    }

    #endregion

    #region RejectUser Tests

    /// <summary>
    /// Tests that RejectUser returns NoContent when employer rejects a user.
    /// </summary>
    [Fact]
    public async Task RejectUser_ReturnsNoContent_WhenEmployerRejectsUser()
    {
        // Arrange
        var employer = new User { Id = 1, Email = "employer@test.com", Role = UserRole.Employer, CompanyId = 10 };
        var userToReject = new User { Id = 2, Email = "pending@test.com", CompanyId = 10, IsApproved = false };

        SetupControllerWithUser(employer);
        _mockUserService.Setup(s => s.GetUserByEmailAsync(employer.Email)).ReturnsAsync(employer);
        _mockUserService.Setup(s => s.GetUserByIdAsync(2)).ReturnsAsync(userToReject);
        _mockUserService.Setup(s => s.DeleteUserAsync(2)).ReturnsAsync(true);

        // Act
        var result = await _controller.RejectUser(2);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    /// <summary>
    /// Tests that RejectUser returns NotFound when user doesn't exist.
    /// </summary>
    [Fact]
    public async Task RejectUser_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var employer = new User { Id = 1, Email = "employer@test.com", Role = UserRole.Employer, CompanyId = 10 };

        SetupControllerWithUser(employer);
        _mockUserService.Setup(s => s.GetUserByEmailAsync(employer.Email)).ReturnsAsync(employer);
        _mockUserService.Setup(s => s.GetUserByIdAsync(999)).ReturnsAsync((User?)null);

        // Act
        var result = await _controller.RejectUser(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    /// <summary>
    /// Tests that RejectUser returns Forbidden when user is from different company.
    /// </summary>
    [Fact]
    public async Task RejectUser_ReturnsForbidden_WhenUserFromDifferentCompany()
    {
        // Arrange
        var employer = new User { Id = 1, Email = "employer@test.com", Role = UserRole.Employer, CompanyId = 10 };
        var userFromOtherCompany = new User { Id = 2, Email = "other@test.com", CompanyId = 20, IsApproved = false };

        SetupControllerWithUser(employer);
        _mockUserService.Setup(s => s.GetUserByEmailAsync(employer.Email)).ReturnsAsync(employer);
        _mockUserService.Setup(s => s.GetUserByIdAsync(2)).ReturnsAsync(userFromOtherCompany);

        // Act
        var result = await _controller.RejectUser(2);

        // Assert
        var statusResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status403Forbidden, statusResult.StatusCode);
    }

    #endregion

    #region Helper Methods

    private void SetupControllerWithUser(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email)
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

    #endregion
}
