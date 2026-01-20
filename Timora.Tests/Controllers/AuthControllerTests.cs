using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Timora.Api.Controllers;
using Timora.Api.DTOs;
using Timora.Api.Services;
using Timora.Data.Entities;

namespace Timora.Tests.Controllers;

/// <summary>
/// Unit tests for the AuthController.
/// </summary>
public class AuthControllerTests
{
    private readonly Mock<ILogger<AuthController>> _mockLogger;
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<ICompanyService> _mockCompanyService;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockLogger = new Mock<ILogger<AuthController>>();
        _mockUserService = new Mock<IUserService>();
        _mockCompanyService = new Mock<ICompanyService>();
        _controller = new AuthController(
            _mockLogger.Object,
            _mockUserService.Object,
            _mockCompanyService.Object
        );
    }

    /// <summary>
    /// Tests that GetCurrentUser returns OK with user claims when authenticated.
    /// </summary>
    [Fact]
    public async Task GetCurrentUser_ReturnsOk_WithUserClaims()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "123"),
            new Claim(ClaimTypes.Email, "test@example.com"),
            new Claim(ClaimTypes.Name, "testuser"),
            new Claim(ClaimTypes.GivenName, "Test"),
            new Claim(ClaimTypes.Surname, "User"),
            new Claim(ClaimTypes.Role, "Employee"),
            new Claim("CompanyId", "1"),
            new Claim("FirebaseUid", "firebase-uid-123")
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = await _controller.GetCurrentUser();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    /// <summary>
    /// Tests that GetCurrentUser returns NotFound when claims are missing and fallback lookup fails.
    /// </summary>
    [Fact]
    public async Task GetCurrentUser_ReturnsNotFound_WhenClaimsAreMissingAndFallbackFails()
    {
        // Arrange
        var claims = new List<Claim>();
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = await _controller.GetCurrentUser();

        // Assert - now returns NotFound when user can't be identified via claims or fallback
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenFirebaseUidMissing()
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = BuildUser() }
        };

        var response = await _controller.Login(new LoginDto { Username = "u", Password = "p" });

        Assert.IsType<UnauthorizedObjectResult>(response);
        _mockUserService.Verify(us => us.GetUserByFirebaseIdAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Login_ReturnsNotFound_WhenUserMissing()
    {
        var user = BuildUser(firebaseUid: "uid-1");
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        _mockUserService.Setup(us => us.GetUserByFirebaseIdAsync("uid-1"))
            .ReturnsAsync((User?)null);

        var response = await _controller.Login(new LoginDto { Username = "u", Password = "p" });

        Assert.IsType<NotFoundObjectResult>(response);
    }

    [Fact]
    public async Task Login_ReturnsBadRequest_WhenUserHasNoCompany()
    {
        var userClaims = BuildUser(firebaseUid: "uid-1");
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = userClaims }
        };

        _mockUserService.Setup(us => us.GetUserByFirebaseIdAsync("uid-1"))
            .ReturnsAsync(new User { Id = 2, FirebaseId = "uid-1", CompanyId = 0, Email = "test@example.com", UserName = "u", IsApproved = true });

        var response = await _controller.Login(new LoginDto { Username = "u", Password = "p" });

        Assert.IsType<BadRequestObjectResult>(response);
    }

    [Fact]
    public async Task Login_ReturnsForbidden_WhenUserNotApproved()
    {
        var userClaims = BuildUser(firebaseUid: "uid-1");
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = userClaims }
        };

        _mockUserService.Setup(us => us.GetUserByFirebaseIdAsync("uid-1"))
            .ReturnsAsync(new User 
            { 
                Id = 2, 
                FirebaseId = "uid-1", 
                CompanyId = 5, 
                Email = "pending@example.com", 
                UserName = "pending",
                IsApproved = false 
            });

        var response = await _controller.Login(new LoginDto { Username = "pending", Password = "p" });

        var result = Assert.IsType<ObjectResult>(response);
        Assert.Equal(StatusCodes.Status403Forbidden, result.StatusCode);
    }

    [Fact]
    public async Task Login_ReturnsOk_WhenCompanyMatches()
    {
        var principal = BuildUser(firebaseUid: "uid-1");
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        _mockUserService.Setup(us => us.GetUserByFirebaseIdAsync("uid-1"))
            .ReturnsAsync(new User
            {
                Id = 5,
                FirebaseId = "uid-1",
                CompanyId = 7,
                Email = "ok@example.com",
                UserName = "ok",
                FirstName = "Ok",
                LastName = "User",
                Role = UserRole.Employee,
                Company = new Company { Id = 7, Name = "Comp" },
                IsApproved = true
            });

        var response = await _controller.Login(new LoginDto { Username = "ok", Password = "p" });

        var ok = Assert.IsType<OkObjectResult>(response);
        Assert.NotNull(ok.Value);
    }

    [Fact]
    public async Task Register_ReturnsUnauthorized_WhenFirebaseUidMissing()
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = BuildUser() }
        };

        var response = await _controller.Register(new RegisterUserDto
        {
            FirstName = "A",
            LastName = "B",
            UserName = "u",
            CompanyId = 1
        });

        Assert.IsType<UnauthorizedObjectResult>(response);
        _mockUserService.Verify(us => us.GetUserByFirebaseIdAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Register_ReturnsConflict_WhenUserAlreadyExists()
    {
        var principal = BuildUser(firebaseUid: "uid-1", email: "conflict@example.com");
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        _mockUserService.Setup(us => us.GetUserByFirebaseIdAsync("uid-1"))
            .ReturnsAsync(new User { Id = 10, FirebaseId = "uid-1" });

        var response = await _controller.Register(new RegisterUserDto
        {
            FirstName = "A",
            LastName = "B",
            UserName = "u",
            Email = "conflict@example.com",
            CompanyId = 2
        });

        Assert.IsType<ConflictObjectResult>(response);
    }

    [Fact]
    public async Task Register_JoinsExistingCompany_WhenCompanyIdProvided()
    {
        var principal = BuildUser(firebaseUid: "uid-1", email: "join@example.com");
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        _mockUserService.Setup(us => us.GetUserByFirebaseIdAsync("uid-1"))
            .ReturnsAsync((User?)null);

        _mockUserService.Setup(us => us.GetUserByEmailAsync("join@example.com"))
            .ReturnsAsync((User?)null);

        _mockCompanyService.Setup(cs => cs.GetCompanyByIdAsync(5))
            .ReturnsAsync(new Company { Id = 5, Name = "Existing" });

        _mockUserService.Setup(us => us.CreateUserAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => { u.Id = 99; return u; });

        var response = await _controller.Register(new RegisterUserDto
        {
            FirstName = "A",
            LastName = "B",
            UserName = "u",
            Email = "join@example.com",
            CompanyId = 5
        });

        Assert.IsType<CreatedAtActionResult>(response);
    }

    [Fact]
    public async Task Register_CreatesCompany_WhenCompanyNameProvided()
    {
        var principal = BuildUser(firebaseUid: "uid-2", email: "new@example.com");
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        _mockUserService.Setup(us => us.GetUserByFirebaseIdAsync("uid-2"))
            .ReturnsAsync((User?)null);

        _mockUserService.Setup(us => us.GetUserByEmailAsync("new@example.com"))
            .ReturnsAsync((User?)null);

        _mockCompanyService.Setup(cs => cs.CreateCompanyAsync(It.IsAny<Company>()))
            .ReturnsAsync(new Company { Id = 11, Name = "NewCo" });

        _mockUserService.Setup(us => us.CreateUserAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => { u.Id = 77; return u; });

        var response = await _controller.Register(new RegisterUserDto
        {
            FirstName = "A",
            LastName = "B",
            UserName = "u",
            Email = "new@example.com",
            CompanyName = "NewCo"
        });

        Assert.IsType<CreatedAtActionResult>(response);
    }

    private static ClaimsPrincipal BuildUser(string? firebaseUid = null, string? email = null)
    {
        var claims = new List<Claim>();
        if (!string.IsNullOrWhiteSpace(firebaseUid))
        {
            claims.Add(new Claim("FirebaseUid", firebaseUid));
        }

        if (!string.IsNullOrWhiteSpace(email))
        {
            claims.Add(new Claim(ClaimTypes.Email, email));
        }

        var identity = new ClaimsIdentity(claims, "TestAuth");
        return new ClaimsPrincipal(identity);
    }
}
