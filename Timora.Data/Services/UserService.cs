using Timora.Data.Entities;

namespace Timora.Data.Services
{
    /// <summary>
    /// Provides business logic operations for user management, role-based permissions,
    /// and authorization checks.
    /// </summary>
    public class UserService
    {
        /// <summary>
        /// Determines whether a user has employer-level privileges (Employer or Admin role).
        /// </summary>
        /// <param name="user">The user to check.</param>
        /// <returns>True if the user is an Employer or Admin, false otherwise.</returns>
        public bool IsEmployer(User user)
        {
            return user.Role == UserRole.Employer || user.Role == UserRole.Admin;
        }

        /// <summary>
        /// Determines whether a user can approve holiday requests based on their role.
        /// </summary>
        /// <param name="user">The user to check.</param>
        /// <returns>True if the user can approve requests, false otherwise.</returns>
        public bool CanApproveRequests(User user)
        {
            return IsEmployer(user);
        }

        /// <summary>
        /// Determines whether a user can create notices in the system.
        /// </summary>
        /// <param name="user">The user to check.</param>
        /// <returns>True if the user can create notices (currently all users can).</returns>
        public bool CanCreateNotices(User user)
        {
            // Assuming all users can create notices based on your requirements
            return true;
        }

        /// <summary>
        /// Determines whether a user can view all holiday requests in the system (not just their own).
        /// </summary>
        /// <param name="user">The user to check.</param>
        /// <returns>True if the user can view all holiday requests, false otherwise.</returns>
        public bool CanViewAllHolidayRequests(User user)
        {
            return IsEmployer(user);
        }

        /// <summary>
        /// Determines whether a user can edit another user's information based on role hierarchy.
        /// </summary>
        /// <param name="currentUser">The user attempting to perform the edit.</param>
        /// <param name="targetUser">The user whose information is being edited.</param>
        /// <returns>True if the current user can edit the target user, false otherwise.</returns>
        public bool CanEditUser(User currentUser, User targetUser)
        {
            // Admin can edit anyone, employers can edit employees, users can edit themselves
            return currentUser.Role == UserRole.Admin ||
                   (currentUser.Role == UserRole.Employer && targetUser.Role == UserRole.Employee) ||
                   currentUser.Id == targetUser.Id;
        }
    }
}