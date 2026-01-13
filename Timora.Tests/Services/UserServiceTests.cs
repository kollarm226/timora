using Moq;
using Timora.Api.Repositories;
using Timora.Api.Services;
using Timora.Data.Entities;

namespace Timora.Tests.Services;

/// <summary>
/// Unit tests for the UserService.
/// </summary>
public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _userService = new UserService(_mockUserRepository.Object);
    }

    #region GetAllUsersAsync Tests

    /// <summary>
    /// Tests that GetAllUsersAsync returns all users from the repository.
    /// </summary>
    [Fact]
    public async Task GetAllUsersAsync_ReturnsAllUsers_WhenUsersExist()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = 1, FirebaseId = "fb1", CompanyId = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", UserName = "johnd", Role = UserRole.Employee },
            new User { Id = 2, FirebaseId = "fb2", CompanyId = 1, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com", UserName = "janes", Role = UserRole.Employer }
        };
        _mockUserRepository.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(users);

        // Act
        var result = await _userService.GetAllUsersAsync();

        // Assert
        Assert.Equal(2, result.Count());
        _mockUserRepository.Verify(r => r.GetAllUsersAsync(), Times.Once);
    }

    /// <summary>
    /// Tests that GetAllUsersAsync returns empty collection when no users exist.
    /// </summary>
    [Fact]
    public async Task GetAllUsersAsync_ReturnsEmptyCollection_WhenNoUsersExist()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetAllUsersAsync()).ReturnsAsync(new List<User>());

        // Act
        var result = await _userService.GetAllUsersAsync();

        // Assert
        Assert.Empty(result);
        _mockUserRepository.Verify(r => r.GetAllUsersAsync(), Times.Once);
    }

    #endregion

    #region GetUserByIdAsync Tests

    /// <summary>
    /// Tests that GetUserByIdAsync returns user when found.
    /// </summary>
    [Fact]
    public async Task GetUserByIdAsync_ReturnsUser_WhenUserExists()
    {
        // Arrange
        var user = new User { Id = 1, FirebaseId = "fb1", CompanyId = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", UserName = "johnd", Role = UserRole.Employee };
        _mockUserRepository.Setup(r => r.GetUserByIdAsync(1)).ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("John", result.FirstName);
        _mockUserRepository.Verify(r => r.GetUserByIdAsync(1), Times.Once);
    }

    /// <summary>
    /// Tests that GetUserByIdAsync returns null when user not found.
    /// </summary>
    [Fact]
    public async Task GetUserByIdAsync_ReturnsNull_WhenUserDoesNotExist()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetUserByIdAsync(999)).ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetUserByIdAsync(999);

        // Assert
        Assert.Null(result);
        _mockUserRepository.Verify(r => r.GetUserByIdAsync(999), Times.Once);
    }

    #endregion

    #region GetUserByEmailAsync Tests

    /// <summary>
    /// Tests that GetUserByEmailAsync returns user when found.
    /// </summary>
    [Fact]
    public async Task GetUserByEmailAsync_ReturnsUser_WhenUserExists()
    {
        // Arrange
        var user = new User { Id = 1, FirebaseId = "fb1", CompanyId = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", UserName = "johnd", Role = UserRole.Employee };
        _mockUserRepository.Setup(r => r.GetUserByEmailAsync("john@example.com")).ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserByEmailAsync("john@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("john@example.com", result.Email);
        _mockUserRepository.Verify(r => r.GetUserByEmailAsync("john@example.com"), Times.Once);
    }

    /// <summary>
    /// Tests that GetUserByEmailAsync returns null when user not found.
    /// </summary>
    [Fact]
    public async Task GetUserByEmailAsync_ReturnsNull_WhenUserDoesNotExist()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetUserByEmailAsync("notfound@example.com")).ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetUserByEmailAsync("notfound@example.com");

        // Assert
        Assert.Null(result);
        _mockUserRepository.Verify(r => r.GetUserByEmailAsync("notfound@example.com"), Times.Once);
    }

    #endregion

    #region CreateUserAsync Tests

    /// <summary>
    /// Tests that CreateUserAsync creates and returns new user.
    /// </summary>
    [Fact]
    public async Task CreateUserAsync_ReturnsCreatedUser_WhenSuccessful()
    {
        // Arrange
        var newUser = new User { FirebaseId = "fb3", CompanyId = 1, FirstName = "Bob", LastName = "Wilson", Email = "bob@example.com", UserName = "bobw", Role = UserRole.Employee };
        var createdUser = new User { Id = 3, FirebaseId = "fb3", CompanyId = 1, FirstName = "Bob", LastName = "Wilson", Email = "bob@example.com", UserName = "bobw", Role = UserRole.Employee };
        _mockUserRepository.Setup(r => r.CreateUserAsync(It.IsAny<User>())).ReturnsAsync(createdUser);

        // Act
        var result = await _userService.CreateUserAsync(newUser);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Id);
        Assert.Equal("Bob", result.FirstName);
        _mockUserRepository.Verify(r => r.CreateUserAsync(It.IsAny<User>()), Times.Once);
    }

    #endregion

    #region DeleteUserAsync Tests

    /// <summary>
    /// Tests that DeleteUserAsync returns true when user deleted successfully.
    /// </summary>
    [Fact]
    public async Task DeleteUserAsync_ReturnsTrue_WhenUserDeleted()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.DeleteUserAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _userService.DeleteUserAsync(1);

        // Assert
        Assert.True(result);
        _mockUserRepository.Verify(r => r.DeleteUserAsync(1), Times.Once);
    }

    /// <summary>
    /// Tests that DeleteUserAsync returns false when user not found.
    /// </summary>
    [Fact]
    public async Task DeleteUserAsync_ReturnsFalse_WhenUserNotFound()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.DeleteUserAsync(999)).ReturnsAsync(false);

        // Act
        var result = await _userService.DeleteUserAsync(999);

        // Assert
        Assert.False(result);
        _mockUserRepository.Verify(r => r.DeleteUserAsync(999), Times.Once);
    }

    #endregion

    #region UpdateUserAsync Tests

    /// <summary>
    /// Tests that UpdateUserAsync returns updated user when successful.
    /// </summary>
    [Fact]
    public async Task UpdateUserAsync_ReturnsUpdatedUser_WhenUserExists()
    {
        // Arrange
        var updateUser = new User { FirstName = "Johnny", LastName = "Doe", Email = "johnny@example.com", UserName = "johnnyd", Role = UserRole.Employer };
        var updatedUser = new User { Id = 1, FirebaseId = "fb1", CompanyId = 1, FirstName = "Johnny", LastName = "Doe", Email = "johnny@example.com", UserName = "johnnyd", Role = UserRole.Employer };
        _mockUserRepository.Setup(r => r.UpdateUserAsync(1, It.IsAny<User>())).ReturnsAsync(updatedUser);

        // Act
        var result = await _userService.UpdateUserAsync(1, updateUser);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Johnny", result.FirstName);
        Assert.Equal(UserRole.Employer, result.Role);
        _mockUserRepository.Verify(r => r.UpdateUserAsync(1, It.IsAny<User>()), Times.Once);
    }

    /// <summary>
    /// Tests that UpdateUserAsync returns null when user not found.
    /// </summary>
    [Fact]
    public async Task UpdateUserAsync_ReturnsNull_WhenUserNotFound()
    {
        // Arrange
        var updateUser = new User { FirstName = "Johnny", LastName = "Doe", Email = "johnny@example.com", UserName = "johnnyd", Role = UserRole.Employer };
        _mockUserRepository.Setup(r => r.UpdateUserAsync(999, It.IsAny<User>())).ReturnsAsync((User?)null);

        // Act
        var result = await _userService.UpdateUserAsync(999, updateUser);

        // Assert
        Assert.Null(result);
        _mockUserRepository.Verify(r => r.UpdateUserAsync(999, It.IsAny<User>()), Times.Once);
    }

    #endregion
}
