using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Timora.Api.Controllers;
using Timora.Api.DTOs;
using Timora.Api.Services;
using Timora.Data.Entities;

namespace Timora.Tests.Controllers;

/// <summary>
/// Unit tests for the CompaniesController.
/// </summary>
public class CompaniesControllerTests
{
    private readonly Mock<ICompanyService> _mockCompanyService;
    private readonly Mock<ILogger<CompaniesController>> _mockLogger;
    private readonly CompaniesController _controller;

    public CompaniesControllerTests()
    {
        _mockCompanyService = new Mock<ICompanyService>();
        _mockLogger = new Mock<ILogger<CompaniesController>>();
        _controller = new CompaniesController(_mockCompanyService.Object, _mockLogger.Object);
    }

    #region GetAllCompanies Tests

    /// <summary>
    /// Tests that GetAllCompanies returns OK with a list of companies.
    /// </summary>
    [Fact]
    public async Task GetAllCompanies_ReturnsOk_WithListOfCompanies()
    {
        // Arrange
        var companies = new List<Company>
        {
            new Company { Id = 1, Name = "Company A" },
            new Company { Id = 2, Name = "Company B" }
        };
        _mockCompanyService.Setup(s => s.GetAllCompaniesAsync()).ReturnsAsync(companies);

        // Act
        var result = await _controller.GetAllCompanies();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCompanies = Assert.IsAssignableFrom<IEnumerable<Company>>(okResult.Value);
        Assert.Equal(2, returnedCompanies.Count());
    }

    /// <summary>
    /// Tests that GetAllCompanies returns OK with empty list when no companies exist.
    /// </summary>
    [Fact]
    public async Task GetAllCompanies_ReturnsOk_WithEmptyList_WhenNoCompaniesExist()
    {
        // Arrange
        _mockCompanyService.Setup(s => s.GetAllCompaniesAsync()).ReturnsAsync(new List<Company>());

        // Act
        var result = await _controller.GetAllCompanies();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCompanies = Assert.IsAssignableFrom<IEnumerable<Company>>(okResult.Value);
        Assert.Empty(returnedCompanies);
    }

    #endregion

    #region GetCompanyById Tests

    /// <summary>
    /// Tests that GetCompanyById returns OK when company exists.
    /// </summary>
    [Fact]
    public async Task GetCompanyById_ReturnsOk_WhenCompanyExists()
    {
        // Arrange
        var company = new Company { Id = 1, Name = "Test Company" };
        _mockCompanyService.Setup(s => s.GetCompanyByIdAsync(1)).ReturnsAsync(company);

        // Act
        var result = await _controller.GetCompanyById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCompany = Assert.IsType<Company>(okResult.Value);
        Assert.Equal(1, returnedCompany.Id);
        Assert.Equal("Test Company", returnedCompany.Name);
    }

    /// <summary>
    /// Tests that GetCompanyById returns NotFound when company does not exist.
    /// </summary>
    [Fact]
    public async Task GetCompanyById_ReturnsNotFound_WhenCompanyDoesNotExist()
    {
        // Arrange
        _mockCompanyService.Setup(s => s.GetCompanyByIdAsync(999)).ReturnsAsync((Company?)null);

        // Act
        var result = await _controller.GetCompanyById(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    #endregion

    #region CreateCompany Tests

    /// <summary>
    /// Tests that CreateCompany returns Created when successful.
    /// </summary>
    [Fact]
    public async Task CreateCompany_ReturnsCreated_WhenSuccessful()
    {
        // Arrange
        var createDto = new CreateCompanyDto { Name = "New Company" };
        var createdCompany = new Company { Id = 1, Name = "New Company" };
        _mockCompanyService.Setup(s => s.CreateCompanyAsync(It.IsAny<Company>())).ReturnsAsync(createdCompany);

        // Act
        var result = await _controller.CreateCompany(createDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.GetCompanyById), createdResult.ActionName);
        var returnedCompany = Assert.IsType<Company>(createdResult.Value);
        Assert.Equal(1, returnedCompany.Id);
    }

    /// <summary>
    /// Tests that CreateCompany returns BadRequest when exception is thrown.
    /// </summary>
    [Fact]
    public async Task CreateCompany_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        var createDto = new CreateCompanyDto { Name = "New Company" };
        _mockCompanyService.Setup(s => s.CreateCompanyAsync(It.IsAny<Company>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.CreateCompany(createDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    #endregion

    #region DeleteCompany Tests

    /// <summary>
    /// Tests that DeleteCompany returns NoContent when successful.
    /// </summary>
    [Fact]
    public async Task DeleteCompany_ReturnsNoContent_WhenSuccessful()
    {
        // Arrange
        _mockCompanyService.Setup(s => s.DeleteCompanyAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteCompany(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    /// <summary>
    /// Tests that DeleteCompany returns NotFound when company does not exist.
    /// </summary>
    [Fact]
    public async Task DeleteCompany_ReturnsNotFound_WhenCompanyDoesNotExist()
    {
        // Arrange
        _mockCompanyService.Setup(s => s.DeleteCompanyAsync(999)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteCompany(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    #endregion

    #region UpdateCompany Tests

    /// <summary>
    /// Tests that UpdateCompany returns OK when successful.
    /// </summary>
    [Fact]
    public async Task UpdateCompany_ReturnsOk_WhenSuccessful()
    {
        // Arrange
        var updateDto = new UpdateCompanyDto { Name = "Updated Company" };
        var updatedCompany = new Company { Id = 1, Name = "Updated Company" };
        _mockCompanyService.Setup(s => s.UpdateCompanyAsync(1, It.IsAny<Company>())).ReturnsAsync(updatedCompany);

        // Act
        var result = await _controller.UpdateCompany(1, updateDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCompany = Assert.IsType<Company>(okResult.Value);
        Assert.Equal("Updated Company", returnedCompany.Name);
    }

    /// <summary>
    /// Tests that UpdateCompany returns NotFound when company does not exist.
    /// </summary>
    [Fact]
    public async Task UpdateCompany_ReturnsNotFound_WhenCompanyDoesNotExist()
    {
        // Arrange
        var updateDto = new UpdateCompanyDto { Name = "Updated Company" };
        _mockCompanyService.Setup(s => s.UpdateCompanyAsync(999, It.IsAny<Company>())).ReturnsAsync((Company?)null);

        // Act
        var result = await _controller.UpdateCompany(999, updateDto);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    /// <summary>
    /// Tests that UpdateCompany returns BadRequest when exception is thrown.
    /// </summary>
    [Fact]
    public async Task UpdateCompany_ReturnsBadRequest_WhenExceptionThrown()
    {
        // Arrange
        var updateDto = new UpdateCompanyDto { Name = "Updated Company" };
        _mockCompanyService.Setup(s => s.UpdateCompanyAsync(1, It.IsAny<Company>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.UpdateCompany(1, updateDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    #endregion
}
