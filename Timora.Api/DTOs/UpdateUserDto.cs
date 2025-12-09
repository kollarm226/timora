using System.ComponentModel.DataAnnotations;
using Timora.Data.Entities;

namespace Timora.Api.DTOs
{
    /// <summary>
    /// Data transfer object for partially updating a user.
    /// All properties are nullable to support partial updates.
    /// </summary>
    public class UpdateUserDto
    {
        /// <summary>
        /// Gets or sets the company ID the user belongs to.
        /// </summary>
        public int? CompanyId { get; set; }

        /// <summary>
        /// Gets or sets the user's first name.
        /// </summary>
        [MaxLength(100)]
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets the user's last name.
        /// </summary>
        [MaxLength(100)]
        public string? LastName { get; set; }

        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        [EmailAddress]
        [MaxLength(255)]
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the user's username.
        /// </summary>
        [MaxLength(50)]
        public string? UserName { get; set; }

        /// <summary>
        /// Gets or sets the user's role.
        /// </summary>
        public UserRole? Role { get; set; }
    }
}
