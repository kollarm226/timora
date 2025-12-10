using System.ComponentModel.DataAnnotations;

namespace Timora.Api.DTOs
{
    /// <summary>
    /// Data transfer object for partially updating a company.
    /// All properties are nullable to support partial updates.
    /// </summary>
    public class UpdateCompanyDto
    {
        /// <summary>
        /// Gets or sets the company name.
        /// </summary>
        [MaxLength(200)]
        public string? Name { get; set; }
    }
}
