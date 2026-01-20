using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using Resend;
using Timora.Api.Services;

namespace Timora.Tests.Services;

/// <summary>
/// Unit tests for the EmailService.
/// </summary>
public class EmailServiceTests
{
    private readonly Mock<IResend> _mockResend;
    private readonly Mock<ILogger<EmailService>> _mockLogger;
    private readonly EmailService _emailService;

    public EmailServiceTests()
    {
        _mockResend = new Mock<IResend>();
        _mockLogger = new Mock<ILogger<EmailService>>();
        _emailService = new EmailService(_mockResend.Object, _mockLogger.Object);
    }

    #region SendHolidayRequestStatusEmailAsync Tests

    /// <summary>
    /// Tests that SendHolidayRequestStatusEmailAsync returns true when email is sent successfully.
    /// </summary>
    [Fact]
    public async Task SendHolidayRequestStatusEmailAsync_ReturnsTrue_WhenEmailSentSuccessfully()
    {
        // Arrange
        var response = new ResendResponse<Guid>(Guid.NewGuid(), null);
        _mockResend.Setup(r => r.EmailSendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _emailService.SendHolidayRequestStatusEmailAsync(
            "test@example.com",
            "John Doe",
            "Approved",
            new DateTime(2026, 1, 15),
            new DateTime(2026, 1, 20),
            "Enjoy your vacation!");

        // Assert
        Assert.True(result);
        _mockResend.Verify(r => r.EmailSendAsync(
            It.Is<EmailMessage>(e => e.Subject == "Holiday Request Approved"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that SendHolidayRequestStatusEmailAsync returns false when email fails to send.
    /// </summary>
    [Fact]
    public async Task SendHolidayRequestStatusEmailAsync_ReturnsFalse_WhenEmailFailsToSend()
    {
        // Arrange
        var exception = new ResendException(HttpStatusCode.BadRequest, ErrorType.ValidationError, "Send failed");
        var response = new ResendResponse<Guid>(exception, null);
        _mockResend.Setup(r => r.EmailSendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _emailService.SendHolidayRequestStatusEmailAsync(
            "test@example.com",
            "John Doe",
            "Denied",
            new DateTime(2026, 1, 15),
            new DateTime(2026, 1, 20),
            null);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Tests that SendHolidayRequestStatusEmailAsync returns false when exception is thrown.
    /// </summary>
    [Fact]
    public async Task SendHolidayRequestStatusEmailAsync_ReturnsFalse_WhenExceptionThrown()
    {
        // Arrange
        _mockResend.Setup(r => r.EmailSendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Network error"));

        // Act
        var result = await _emailService.SendHolidayRequestStatusEmailAsync(
            "test@example.com",
            "John Doe",
            "Approved",
            new DateTime(2026, 1, 15),
            new DateTime(2026, 1, 20),
            null);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region SendUserApprovalEmailAsync Tests

    /// <summary>
    /// Tests that SendUserApprovalEmailAsync returns true when email is sent successfully.
    /// </summary>
    [Fact]
    public async Task SendUserApprovalEmailAsync_ReturnsTrue_WhenEmailSentSuccessfully()
    {
        // Arrange
        var response = new ResendResponse<Guid>(Guid.NewGuid(), null);
        _mockResend.Setup(r => r.EmailSendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _emailService.SendUserApprovalEmailAsync(
            "newuser@example.com",
            "Jane Smith",
            "Acme Corp");

        // Assert
        Assert.True(result);
        _mockResend.Verify(r => r.EmailSendAsync(
            It.Is<EmailMessage>(e => e.Subject == "Your Account Has Been Approved - Welcome to Timora!"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Tests that SendUserApprovalEmailAsync returns false when email fails to send.
    /// </summary>
    [Fact]
    public async Task SendUserApprovalEmailAsync_ReturnsFalse_WhenEmailFailsToSend()
    {
        // Arrange
        var exception = new ResendException(HttpStatusCode.BadRequest, ErrorType.ValidationError, "Send failed");
        var response = new ResendResponse<Guid>(exception, null);
        _mockResend.Setup(r => r.EmailSendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _emailService.SendUserApprovalEmailAsync(
            "newuser@example.com",
            "Jane Smith",
            "Acme Corp");

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Tests that SendUserApprovalEmailAsync returns false when exception is thrown.
    /// </summary>
    [Fact]
    public async Task SendUserApprovalEmailAsync_ReturnsFalse_WhenExceptionThrown()
    {
        // Arrange
        _mockResend.Setup(r => r.EmailSendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Network error"));

        // Act
        var result = await _emailService.SendUserApprovalEmailAsync(
            "newuser@example.com",
            "Jane Smith",
            "Acme Corp");

        // Assert
        Assert.False(result);
    }

    #endregion
}
