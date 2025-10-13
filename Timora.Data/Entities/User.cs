namespace Timora.Data.Entities
{
    /// <summary>
    /// Represents a user in the system with authentication, role management, and relationships to holiday requests and notices.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user's Firebase account.
        /// </summary>
        public string FirebaseId { get; set; } = null!;

        /// <summary>
        /// Gets or sets the ID of the company the user belongs to.
        /// </summary>
        public int CompanyId { get; set; }

        /// <summary>
        /// Gets or sets the user's first name.
        /// </summary>
        public string FirstName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the user's last name.
        /// </summary>
        public string LastName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the user's email address. Must be unique across the system.
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Gets or sets the user's unique username for login.
        /// </summary>
        public string UserName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the user's role in the system. Defaults to Employee.
        /// </summary>
        public UserRole Role { get; set; } = UserRole.Employee;

        /// <summary>
        /// Gets or sets when the user account was created. Defaults to UTC now.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the company this user belongs to.
        /// </summary>
        public Company? Company { get; set; }

        /// <summary>
        /// Gets or sets the collection of holiday requests submitted by this user.
        /// </summary>
        public ICollection<HolidayRequest> HolidayRequests { get; set; } =
            new List<HolidayRequest>();

        /// <summary>
        /// Gets or sets the collection of holiday requests that this user has approved or denied (for employers).
        /// </summary>
        public ICollection<HolidayRequest> ResolvedHolidayRequests { get; set; } =
            new List<HolidayRequest>();

        /// <summary>
        /// Gets or sets the collection of notices created by this user.
        /// </summary>
        public ICollection<Notice> Notices { get; set; } = new List<Notice>();

        /// <summary>
        /// Gets the user's full name by combining first and last name.
        /// </summary>
        public string FullName => $"{FirstName} {LastName}";
    }
}
