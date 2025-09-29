namespace Timora.Data.Entities
{
    /// <summary>
    /// Defines the possible states of a holiday request throughout its lifecycle.
    /// </summary>
    public enum HolidayRequestStatus
    {
        /// <summary>
        /// The holiday request is awaiting approval or denial from an employer.
        /// </summary>
        Pending = 0,
        
        /// <summary>
        /// The holiday request has been approved by an employer.
        /// </summary>
        Approved = 1,
        
        /// <summary>
        /// The holiday request has been denied by an employer.
        /// </summary>
        Denied = 2,
        
        /// <summary>
        /// The holiday request has been cancelled by the requesting employee.
        /// </summary>
        Cancelled = 3,
    }
}
