using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Timora.Api.DTOs
{
    /// <summary>
    /// Data transfer object for creating a new document.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CreateDocumentDto
    {
        /// <summary>
        /// Gets or sets the title of the document.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = null!;

        /// <summary>
        /// Gets or sets the description of the document.
        /// </summary>
        [Required]
        [MaxLength(2000)]
        public string Description { get; set; } = null!;

        /// <summary>
        /// Gets or sets the URL where the document file is stored.
        /// </summary>
        [Required]
        [MaxLength(2000)]
        public string FileUrl { get; set; } = null!;

        /// <summary>
        /// Gets or sets the ID of the company that owns this document.
        /// </summary>
        [Required]
        public int CompanyId { get; set; }
    }
}
