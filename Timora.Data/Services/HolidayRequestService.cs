using Timora.Data.Entities;

namespace Timora.Data.Services
{
    /// <summary>
    /// Provides business logic operations for managing holiday requests, including approval workflows,
    /// validation, and conflict detection.
    /// </summary>
    public class HolidayRequestService
    {
        /// <summary>
        /// Determines whether a holiday request can be approved based on its current status.
        /// </summary>
        /// <param name="request">The holiday request to check.</param>
        /// <returns>True if the request can be approved, false otherwise.</returns>
        public bool CanBeApproved(HolidayRequest request)
        {
            return request.Status == HolidayRequestStatus.Pending;
        }

        /// <summary>
        /// Determines whether a holiday request can be cancelled based on its current status.
        /// </summary>
        /// <param name="request">The holiday request to check.</param>
        /// <returns>True if the request can be cancelled, false otherwise.</returns>
        public bool CanBeCancelled(HolidayRequest request)
        {
            return request.Status == HolidayRequestStatus.Pending;
        }

        /// <summary>
        /// Approves a holiday request, updating its status and resolution details.
        /// </summary>
        /// <param name="request">The holiday request to approve.</param>
        /// <param name="approverId">The ID of the user approving the request.</param>
        /// <param name="comment">Optional comment explaining the approval decision.</param>
        /// <exception cref="InvalidOperationException">Thrown when the request cannot be approved in its current state.</exception>
        public void ApproveRequest(HolidayRequest request, int approverId, string? comment = null)
        {
            if (!CanBeApproved(request))
                throw new InvalidOperationException("Holiday request cannot be approved in its current state");

            request.Status = HolidayRequestStatus.Approved;
            request.ResolvedAt = DateTime.UtcNow;
            request.ResolvedByUserId = approverId;
            request.ResolverComment = comment;
        }

        /// <summary>
        /// Denies a holiday request, updating its status and resolution details.
        /// </summary>
        /// <param name="request">The holiday request to deny.</param>
        /// <param name="deniedById">The ID of the user denying the request.</param>
        /// <param name="comment">Optional comment explaining the denial decision.</param>
        /// <exception cref="InvalidOperationException">Thrown when the request cannot be denied in its current state.</exception>
        public void DenyRequest(HolidayRequest request, int deniedById, string? comment = null)
        {
            if (!CanBeApproved(request))
                throw new InvalidOperationException("Holiday request cannot be denied in its current state");

            request.Status = HolidayRequestStatus.Denied;
            request.ResolvedAt = DateTime.UtcNow;
            request.ResolvedByUserId = deniedById;
            request.ResolverComment = comment;
        }

        /// <summary>
        /// Cancels a holiday request, updating its status and resolution timestamp.
        /// </summary>
        /// <param name="request">The holiday request to cancel.</param>
        /// <exception cref="InvalidOperationException">Thrown when the request cannot be cancelled in its current state.</exception>
        public void CancelRequest(HolidayRequest request)
        {
            if (!CanBeCancelled(request))
                throw new InvalidOperationException("Holiday request cannot be cancelled in its current state");

            request.Status = HolidayRequestStatus.Cancelled;
            request.ResolvedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Checks if a holiday request conflicts with any existing approved requests for the same user.
        /// </summary>
        /// <param name="request">The holiday request to check for conflicts.</param>
        /// <param name="existingRequests">The collection of existing holiday requests to check against.</param>
        /// <returns>True if there is a date conflict with an approved request, false otherwise.</returns>
        public bool HasDateConflict(HolidayRequest request, IEnumerable<HolidayRequest> existingRequests)
        {
            return existingRequests.Any(existing =>
                existing.Id != request.Id &&
                existing.Status == HolidayRequestStatus.Approved &&
                request.StartDate <= existing.EndDate &&
                request.EndDate >= existing.StartDate);
        }

        /// <summary>
        /// Validates that a holiday request has a valid date range (end >= start, start >= today).
        /// </summary>
        /// <param name="request">The holiday request to validate.</param>
        /// <returns>True if the date range is valid, false otherwise.</returns>
        public bool IsValidDateRange(HolidayRequest request)
        {
            return request.StartDate <= request.EndDate && 
                   request.StartDate >= DateTime.Today;
        }
    }
}