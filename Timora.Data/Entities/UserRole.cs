namespace Timora.Data.Entities
{
    /// <summary>
    /// Defines the different roles a user can have in the system, determining their permissions and capabilities.
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// Standard employee who can create holiday requests and notices.
        /// </summary>
        Employee = 0,
        
        /// <summary>
        /// Employer who can approve/deny holiday requests from employees and has elevated permissions.
        /// </summary>
        Employer = 1,
        
        /// <summary>
        /// Administrator with full system access and management capabilities.
        /// </summary>
        Admin = 2,
    }
}
