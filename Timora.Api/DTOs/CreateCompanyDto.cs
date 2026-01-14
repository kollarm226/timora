using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Timora.Api.DTOs
{
    /// <summary>
    /// Data transfer object for creating a new company.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CreateCompanyDto
    {
        /// <summary>
        /// Gets or sets the company name.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;
    }
}
