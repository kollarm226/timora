using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Timora.Api.Controllers;
using Timora.Api.DTOs;
using Timora.Api.Services;
using Timora.Data.Entities;

namespace Timora.Tests.Controllers;

/// <summary>
/// Unit tests for the UsersController.
/// </summary>
public class UsersControllerTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<ILogger<UsersController>> _mockLogger;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _mockUserService = new Mock<IUserService>();
        _mockLogger = new Mock<ILogger<UsersController>>();
        _controller = new UsersController(_mockUserService.Object, _mockLogger.Object);
    }

    #region GetAllUsers Tests

    /// <summary>
    /// Tests that GetAllUsers returns OK with a list of users.
    /// </summary>
    [Fact]
    public async Task GetAllUsers_ReturnsOk_WithListOfUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = 1, FirebaseId = "fb1", CompanyId = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", UserName = "johnd", Role = UserRole.Employee },
            new User { Id = 2, FirebaseId = "fb2", CompanyId = 1, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com", UserName = "janes", Role = UserRole.Employer }
        };
        _mockUserService.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(users);

        // Act
        var result = await _controller.GetAllUsers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedUsers = Assert.IsAssignableFrom<IEnumerable<User>>(okResult.Value);
        Assert.Equal(2, returnedUsers.Count());
    }

    /// <summary>
    /// Tests that GetAllUsers returns OK with empty list when no users exist.
    /// </summary>
    [Fact]
    public async Task GetAllUsers_ReturnsOk_WithEmptyList_WhenNoUsersExist()
    {
        // Arrange
        _mockUserService.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(new List<User>());

        // Act
        var result = await _controller.GetAllUsers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedUsers = Assert.IsAssignableFrom<IEnumerable<User>>(okResult.Value);
        Assert.Empty(returnedUsers);
    }

    #endregion

    #region GetUserById Tests

    /// <summary>
    /// Tests that GetUserById returns OK when user exists.
    /// </summary>
    [Fact]
    public async Task GetUserById_ReturnsOk_WhenUserExists()
    {
        // Arrange
        var user = new User { Id = 1, FirebaseId = "fb1", CompanyId = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", UserName = "johnd", Role = UserRole.Employee };
        _mockUserService.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync(user);

        // Act
        var result = await _controller.GetUserById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedUser = Assert.IsType<User>(okResult.Value);
        Assert.Equal(1, returnedUser.Id);
        Assert.Equal("John", returnedUser.FirstName);
    }

    /// <summary>
    /// Tests that GetUserById returns NotFound when user does not exist.
    /// </summary>
    [Fact]
    public async Task GetUserById_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        _mockUserService.Setup(s => s.GetUserByIdAsync(999)).ReturnsAsync((User?)null);

        // Act
        var result = await _controller.GetUserById(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    #endregion

    #region GetUserByEmail Tests

    /// <summary>
    /// Tests that GetUserByEmail returns OK when user exists.
    /// </summary>
    [Fact]
    public async Task GetUserByEmail_ReturnsOk_WhenUserExists()
    {
        // Arrange
        var user = new User { Id = 1, FirebaseId = "fb1", CompanyId = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", UserName = "johnd", Role = UserRole.Employee };
        _mockUserService.Setup(s => s.GetUserByEmailAsync("john@example.com")).ReturnsAsync(user);

        // Act
        var result = await _controller.GetUserByEmail("john@example.com");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedUser = Assert.IsType<User>(okResult.Value);
        Assert.Equal("john@example.com", returnedUser.Email);
    }

    /// <summary>
    /// Tests that GetUserByEmail returns NotFound when user does not exist.
    /// </summary>
    [Fact]
    public async Task GetUserByEmail_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        _mockUserService.Setup(s => s.GetUserByEmailAsync("nonexistent@example.com")).ReturnsAsync((User?)null);

        // Act
        var result = await _controller.GetUserByEmail("nonexistent@example.com");

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    #endregion

    #region CreateUser Tests

    /// <summary>
    /// Tests that CreateUser returns Created when successful.
    /// </summary>
    [Fact]
    public async Task CreateUser_ReturnsCreated_WhenSuccessful()
    {
        // Arrange
        var createDto = new CreateUserDto 
        { 
            FirebaseId = "fb1",
            CompanyId = 1,
            FirstName = "John", 
            LastName = "Doe", 
            Email = "john@example.com", 
            UserName = "johnd",
            Role = UserRole.Employee
        };
        var createdUser = new User { Id = 1, FirebaseId = "fb1", CompanyId = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", UserName = "johnd", Role = UserRole.Employee };
        _mockUserService.Setup(s => s.CreateUserAsync(It.IsAny<User>())).ReturnsAsync(createdUser);

        // Act
        var result = await _controller.CreateUser(createDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.GetUserById), createdResult.ActionName);
        var returnedUser = Assert.IsType<User>(createdResult.Value);
        Assert.Equal(1, returnedUser.Id);
    }

    /// <summary>
    /// Tests that CreateUser returns BadRequest when exception is thrown.
    /// </summary>
    [Fact]
    public async Task CreateUser_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        var createDto = new CreateUserDto 
        { 
            FirebaseId = "fb1",
            CompanyId = 1,
            FirstName = "John", 
            LastName = "Doe", 
            Email = "john@example.com", 
            UserName = "johnd",
            Role = UserRole.Employee
        };
        _mockUserService.Setup(s => s.CreateUserAsync(It.IsAny<User>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.CreateUser(createDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    #endregion

    #region DeleteUser Tests

    /// <summary>
    /// Tests that DeleteUser returns NoContent when successful.
    /// </summary>
    [Fact]
    public async Task DeleteUser_ReturnsNoContent_WhenSuccessful()
    {
        // Arrange
        _mockUserService.Setup(s => s.DeleteUserAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteUser(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    /// <summary>
    /// Tests that DeleteUser returns NotFound when user does not exist.
    /// </summary>
    [Fact]
    public async Task DeleteUser_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        _mockUserService.Setup(s => s.DeleteUserAsync(999)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteUser(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    #endregion

    #region UpdateUser Tests

    /// <summary>
    /// Tests that UpdateUser returns OK when successful.
    /// </summary>
    [Fact]
    public async Task UpdateUser_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var updateDto = new UpdateUserDto { FirstName = "Johnny", LastName = "Updated" };
        var updatedUser = new User { Id = 1, FirebaseId = "fb1", CompanyId = 1, FirstName = "Johnny", LastName = "Updated", Email = "john@example.com", UserName = "johnd", Role = UserRole.Employee };
        _mockUserService.Setup(s => s.UpdateUserAsync(1, It.IsAny<User>())).ReturnsAsync(updatedUser);

        // Act
        var result = await _controller.UpdateUser(1, updateDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedUser = Assert.IsType<User>(okResult.Value);
        Assert.Equal("Johnny", returnedUser.FirstName);
    }

    /// <summary>
    /// Tests that UpdateUser returns NotFound when user does not exist.
    /// </summary>
    [Fact]
    public async Task UpdateUser_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var updateDto = new UpdateUserDto { FirstName = "Johnny" };
        _mockUserService.Setup(s => s.UpdateUserAsync(999, It.IsAny<User>())).ReturnsAsync((User?)null);

        // Act
        var result = await _controller.UpdateUser(999, updateDto);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    /// <summary>
    /// Tests that UpdateUser returns BadRequest when exception is thrown.
    /// </summary>
    [Fact]
    public async Task UpdateUser_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        var updateDto = new UpdateUserDto { FirstName = "Johnny" };
        _mockUserService.Setup(s => s.UpdateUserAsync(1, It.IsAny<User>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.UpdateUser(1, updateDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    #endregion
}
