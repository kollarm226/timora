using Resend;

namespace Timora.Api.Services
{
    /// <summary>
    /// Service implementation for sending email notifications using Resend.
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IResend _resend;
        private readonly ILogger<EmailService> _logger;
        private const string FromEmail = "onboarding@resend.dev";

        /// <summary>
        /// Initializes a new instance of the EmailService.
        /// </summary>
        /// <param name="resend">The Resend client.</param>
        /// <param name="logger">The logger instance.</param>
        public EmailService(IResend resend, ILogger<EmailService> logger)
        {
            _resend = resend;
            _logger = logger;
        }

        /// <summary>
        /// Sends a holiday request status notification email.
        /// </summary>
        public async Task<bool> SendHolidayRequestStatusEmailAsync(
            string toEmail,
            string employeeName,
            string status,
            DateTime startDate,
            DateTime endDate,
            string? resolverComment)
        {
            try
            {
                var subject = $"Holiday Request {status}";
                var htmlBody = BuildEmailBody(employeeName, status, startDate, endDate, resolverComment);

                var message = new EmailMessage
                {
                    From = FromEmail,
                    To = toEmail,
                    Subject = subject,
                    HtmlBody = htmlBody
                };

                var response = await _resend.EmailSendAsync(message);

                if (response.Success)
                {
                    _logger.LogInformation(
                        "Holiday request status email sent successfully to {Email}",
                        toEmail);
                    return true;
                }

                _logger.LogWarning(
                    "Failed to send holiday request status email to {Email}: {Error}",
                    toEmail,
                    response.Exception?.Message);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error sending holiday request status email to {Email}",
                    toEmail);
                return false;
            }
        }

        private static string BuildEmailBody(
            string employeeName,
            string status,
            DateTime startDate,
            DateTime endDate,
            string? resolverComment)
        {
            var statusColor = status == "Approved" ? "#28a745" : "#dc3545";
            var commentSection = string.IsNullOrWhiteSpace(resolverComment)
                ? string.Empty
                : $@"<p><strong>Comment:</strong> {resolverComment}</p>";

            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: {statusColor}; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background-color: #f9f9f9; }}
        .footer {{ text-align: center; padding: 10px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Holiday Request {status}</h1>
        </div>
        <div class='content'>
            <p>Dear {employeeName},</p>
            <p>Your holiday request has been <strong>{status.ToLower()}</strong>.</p>
            <p><strong>Period:</strong> {startDate:MMMM dd, yyyy} - {endDate:MMMM dd, yyyy}</p>
            {commentSection}
            <p>If you have any questions, please contact your manager.</p>
        </div>
        <div class='footer'>
            <p>This is an automated message from Timora.</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
