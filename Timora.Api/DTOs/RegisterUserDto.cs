using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Timora.Api.DTOs
{
    /// <summary>
    /// Data transfer object for user registration.
    /// Either CompanyId (to join existing company) or CompanyName (to create new company) must be provided.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RegisterUserDto : IValidatableObject
    {
        /// <summary>
        /// Gets or sets the ID of an existing company to join.
        /// If provided, user will be registered as Employee.
        /// </summary>
        public int? CompanyId { get; set; }

        /// <summary>
        /// Gets or sets the name of a new company to create.
        /// If provided, user will be registered as Employer (company admin).
        /// </summary>
        [MaxLength(200)]
        public string? CompanyName { get; set; }

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
        /// Gets or sets the user's unique username for login.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; } = null!;

        /// <summary>
        /// Validates that either CompanyId or CompanyName is provided, but not both.
        /// </summary>
        /// <param name="validationContext">The validation context.</param>
        /// <returns>A collection of validation results.</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!CompanyId.HasValue && string.IsNullOrWhiteSpace(CompanyName))
            {
                yield return new ValidationResult(
                    "Either CompanyId (to join existing company) or CompanyName (to create new company) must be provided.",
                    new[] { nameof(CompanyId), nameof(CompanyName) }
                );
            }

            if (CompanyId.HasValue && !string.IsNullOrWhiteSpace(CompanyName))
            {
                yield return new ValidationResult(
                    "Cannot specify both CompanyId and CompanyName. Choose one: join existing company (CompanyId) or create new company (CompanyName).",
                    new[] { nameof(CompanyId), nameof(CompanyName) }
                );
            }
        }
    }
}
