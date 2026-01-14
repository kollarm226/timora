using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Timora.Data.Entities;

namespace Timora.Api.DTOs
{
    /// <summary>
    /// Data transfer object for creating a new user.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CreateUserDto
    {
        /// <summary>
        /// Gets or sets the Firebase UID for the user.
        /// </summary>
        [Required]
        [MaxLength(128)]
        public string FirebaseId { get; set; } = null!;

        /// <summary>
        /// Gets or sets the company ID the user belongs to.
        /// </summary>
        [Required]
        public int CompanyId { get; set; }

        /// <summary>
        /// Gets or sets the user's first name.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the user's last name.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = null!;

        /// <summary>
        /// Gets or sets the user's username.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the user's role. Defaults to Employee.
        /// </summary>
        public UserRole Role { get; set; } = UserRole.Employee;
    }
}
