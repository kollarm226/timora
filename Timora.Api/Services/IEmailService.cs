namespace Timora.Api.Services
{
    /// <summary>
    /// Service interface for sending email notifications.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends a holiday request status notification email.
        /// </summary>
        /// <param name="toEmail">The recipient's email address.</param>
        /// <param name="employeeName">The name of the employee who submitted the request.</param>
        /// <param name="status">The new status of the holiday request (Approved/Denied).</param>
        /// <param name="startDate">The start date of the holiday.</param>
        /// <param name="endDate">The end date of the holiday.</param>
        /// <param name="resolverComment">Optional comment from the resolver.</param>
        /// <returns>True if the email was sent successfully; otherwise, false.</returns>
        Task<bool> SendHolidayRequestStatusEmailAsync(
            string toEmail,
            string employeeName,
            string status,
            DateTime startDate,
            DateTime endDate,
            string? resolverComment);

        /// <summary>
        /// Sends a user approval notification email.
        /// </summary>
        /// <param name="toEmail">The recipient's email address.</param>
        /// <param name="userName">The name of the approved user.</param>
        /// <param name="companyName">The name of the company.</param>
        /// <returns>True if the email was sent successfully; otherwise, false.</returns>
        Task<bool> SendUserApprovalEmailAsync(
            string toEmail,
            string userName,
            string companyName);
    }
}
