namespace Timora.Data.Entities
{
    /// <summary>
    /// Represents a holiday request submitted by an employee for approval by an employer.
    /// Tracks the request lifecycle from submission through approval/denial.
    /// </summary>
    public class HolidayRequest
    {
        /// <summary>
        /// Gets or sets the unique identifier for the holiday request.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Gets or sets the ID of the user who submitted this holiday request.
        /// </summary>
        public int UserId { get; set; }
        
        /// <summary>
        /// Gets or sets the start date of the requested holiday period.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date of the requested holiday period.
        /// </summary>
        public DateTime EndDate { get; set; }
        
        /// <summary>
        /// Gets or sets the reason provided for the holiday request.
        /// </summary>
        public string Reason { get; set; } = null!;
        
        /// <summary>
        /// Gets or sets the current status of the holiday request. Defaults to Pending.
        /// </summary>
        public HolidayRequestStatus Status { get; set; } = HolidayRequestStatus.Pending;
        
        /// <summary>
        /// Gets or sets when the holiday request was submitted. Defaults to UTC now.
        /// </summary>
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Gets or sets when the holiday request was resolved (approved/denied/cancelled). Null if still pending.
        /// </summary>
        public DateTime? ResolvedAt { get; set; }
        
        /// <summary>
        /// Gets or sets the ID of the user who approved or denied this request. Null if still pending.
        /// </summary>
        public int? ResolvedByUserId { get; set; }
        
        /// <summary>
        /// Gets or sets an optional comment from the resolver explaining their decision.
        /// </summary>
        public string? ResolverComment { get; set; }

        /// <summary>
        /// Gets or sets the user who submitted this holiday request.
        /// </summary>
        public User User { get; set; } = null!;
        
        /// <summary>
        /// Gets or sets the user who resolved (approved/denied) this request. Null if still pending.
        /// </summary>
        public User? ResolvedBy { get; set; }

        /// <summary>
        /// Gets the total number of days requested, calculated from start and end dates (inclusive).
        /// </summary>
        public int TotalDaysRequested => (EndDate - StartDate).Days + 1;
    }
}
