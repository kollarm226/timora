using System.Diagnostics.CodeAnalysis;
using Timora.Data.Entities;

namespace Timora.Api.DTOs
{
    /// <summary>
    /// Data transfer object for returning user data with approval status.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UserResponseDto
    {
        /// <summary>
        /// Gets or sets the user's unique identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Gets or sets the user's username.
        /// </summary>
        public string UserName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the user's first name.
        /// </summary>
        public string FirstName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the user's last name.
        /// </summary>
        public string LastName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the user's role.
        /// </summary>
        public string Role { get; set; } = null!;

        /// <summary>
        /// Gets or sets the company ID.
        /// </summary>
        public int CompanyId { get; set; }

        /// <summary>
        /// Gets or sets the company name.
        /// </summary>
        public string? CompanyName { get; set; }

        /// <summary>
        /// Gets or sets whether the user is approved.
        /// </summary>
        public bool IsApproved { get; set; }

        /// <summary>
        /// Gets or sets the ID of the approver.
        /// </summary>
        public int? ApprovedBy { get; set; }

        /// <summary>
        /// Gets or sets when the user was approved.
        /// </summary>
        public DateTime? ApprovedAt { get; set; }

        /// <summary>
        /// Gets or sets when the user was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Creates a UserResponseDto from a User entity.
        /// </summary>
        public static UserResponseDto FromUser(User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString(),
                CompanyId = user.CompanyId,
                CompanyName = user.Company?.Name,
                IsApproved = user.IsApproved,
                ApprovedBy = user.ApprovedBy,
                ApprovedAt = user.ApprovedAt,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
