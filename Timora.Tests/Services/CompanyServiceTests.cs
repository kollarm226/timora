using Moq;
using Timora.Api.Repositories;
using Timora.Api.Services;
using Timora.Data.Entities;

namespace Timora.Tests.Services;

/// <summary>
/// Unit tests for the CompanyService.
/// </summary>
public class CompanyServiceTests
{
    private readonly Mock<ICompanyRepository> _mockCompanyRepository;
    private readonly CompanyService _companyService;

    public CompanyServiceTests()
    {
        _mockCompanyRepository = new Mock<ICompanyRepository>();
        _companyService = new CompanyService(_mockCompanyRepository.Object);
    }

    #region GetAllCompaniesAsync Tests

    /// <summary>
    /// Tests that GetAllCompaniesAsync returns all companies from the repository.
    /// </summary>
    [Fact]
    public async Task GetAllCompaniesAsync_ReturnsAllCompanies_WhenCompaniesExist()
    {
        // Arrange
        var companies = new List<Company>
        {
            new Company { Id = 1, Name = "Acme Corp" },
            new Company { Id = 2, Name = "Tech Solutions" }
        };
        _mockCompanyRepository.Setup(r => r.GetAllCompaniesAsync()).ReturnsAsync(companies);

        // Act
        var result = await _companyService.GetAllCompaniesAsync();

        // Assert
        Assert.Equal(2, result.Count());
        _mockCompanyRepository.Verify(r => r.GetAllCompaniesAsync(), Times.Once);
    }

    /// <summary>
    /// Tests that GetAllCompaniesAsync returns empty collection when no companies exist.
    /// </summary>
    [Fact]
    public async Task GetAllCompaniesAsync_ReturnsEmptyCollection_WhenNoCompaniesExist()
    {
        // Arrange
        _mockCompanyRepository.Setup(r => r.GetAllCompaniesAsync()).ReturnsAsync(new List<Company>());

        // Act
        var result = await _companyService.GetAllCompaniesAsync();

        // Assert
        Assert.Empty(result);
        _mockCompanyRepository.Verify(r => r.GetAllCompaniesAsync(), Times.Once);
    }

    #endregion

    #region GetCompanyByIdAsync Tests

    /// <summary>
    /// Tests that GetCompanyByIdAsync returns company when found.
    /// </summary>
    [Fact]
    public async Task GetCompanyByIdAsync_ReturnsCompany_WhenCompanyExists()
    {
        // Arrange
        var company = new Company { Id = 1, Name = "Acme Corp" };
        _mockCompanyRepository.Setup(r => r.GetCompanyByIdAsync(1)).ReturnsAsync(company);

        // Act
        var result = await _companyService.GetCompanyByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Acme Corp", result.Name);
        _mockCompanyRepository.Verify(r => r.GetCompanyByIdAsync(1), Times.Once);
    }

    /// <summary>
    /// Tests that GetCompanyByIdAsync returns null when company not found.
    /// </summary>
    [Fact]
    public async Task GetCompanyByIdAsync_ReturnsNull_WhenCompanyDoesNotExist()
    {
        // Arrange
        _mockCompanyRepository.Setup(r => r.GetCompanyByIdAsync(999)).ReturnsAsync((Company?)null);

        // Act
        var result = await _companyService.GetCompanyByIdAsync(999);

        // Assert
        Assert.Null(result);
        _mockCompanyRepository.Verify(r => r.GetCompanyByIdAsync(999), Times.Once);
    }

    #endregion

    #region CreateCompanyAsync Tests

    /// <summary>
    /// Tests that CreateCompanyAsync creates and returns new company.
    /// </summary>
    [Fact]
    public async Task CreateCompanyAsync_ReturnsCreatedCompany_WhenSuccessful()
    {
        // Arrange
        var newCompany = new Company { Name = "New Company" };
        var createdCompany = new Company { Id = 3, Name = "New Company" };
        _mockCompanyRepository.Setup(r => r.CreateCompanyAsync(It.IsAny<Company>())).ReturnsAsync(createdCompany);

        // Act
        var result = await _companyService.CreateCompanyAsync(newCompany);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Id);
        Assert.Equal("New Company", result.Name);
        _mockCompanyRepository.Verify(r => r.CreateCompanyAsync(It.IsAny<Company>()), Times.Once);
    }

    #endregion

    #region DeleteCompanyAsync Tests

    /// <summary>
    /// Tests that DeleteCompanyAsync returns true when company deleted successfully.
    /// </summary>
    [Fact]
    public async Task DeleteCompanyAsync_ReturnsTrue_WhenCompanyDeleted()
    {
        // Arrange
        _mockCompanyRepository.Setup(r => r.DeleteCompanyAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _companyService.DeleteCompanyAsync(1);

        // Assert
        Assert.True(result);
        _mockCompanyRepository.Verify(r => r.DeleteCompanyAsync(1), Times.Once);
    }

    /// <summary>
    /// Tests that DeleteCompanyAsync returns false when company not found.
    /// </summary>
    [Fact]
    public async Task DeleteCompanyAsync_ReturnsFalse_WhenCompanyNotFound()
    {
        // Arrange
        _mockCompanyRepository.Setup(r => r.DeleteCompanyAsync(999)).ReturnsAsync(false);

        // Act
        var result = await _companyService.DeleteCompanyAsync(999);

        // Assert
        Assert.False(result);
        _mockCompanyRepository.Verify(r => r.DeleteCompanyAsync(999), Times.Once);
    }

    #endregion

    #region UpdateCompanyAsync Tests

    /// <summary>
    /// Tests that UpdateCompanyAsync returns updated company when successful.
    /// </summary>
    [Fact]
    public async Task UpdateCompanyAsync_ReturnsUpdatedCompany_WhenCompanyExists()
    {
        // Arrange
        var updateCompany = new Company { Name = "Updated Acme Corp" };
        var updatedCompany = new Company { Id = 1, Name = "Updated Acme Corp" };
        _mockCompanyRepository.Setup(r => r.UpdateCompanyAsync(1, It.IsAny<Company>())).ReturnsAsync(updatedCompany);

        // Act
        var result = await _companyService.UpdateCompanyAsync(1, updateCompany);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Updated Acme Corp", result.Name);
        _mockCompanyRepository.Verify(r => r.UpdateCompanyAsync(1, It.IsAny<Company>()), Times.Once);
    }

    /// <summary>
    /// Tests that UpdateCompanyAsync returns null when company not found.
    /// </summary>
    [Fact]
    public async Task UpdateCompanyAsync_ReturnsNull_WhenCompanyNotFound()
    {
        // Arrange
        var updateCompany = new Company { Name = "Updated Company" };
        _mockCompanyRepository.Setup(r => r.UpdateCompanyAsync(999, It.IsAny<Company>())).ReturnsAsync((Company?)null);

        // Act
        var result = await _companyService.UpdateCompanyAsync(999, updateCompany);

        // Assert
        Assert.Null(result);
        _mockCompanyRepository.Verify(r => r.UpdateCompanyAsync(999, It.IsAny<Company>()), Times.Once);
    }

    #endregion
}
