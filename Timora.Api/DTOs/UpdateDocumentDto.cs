using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Timora.Api.DTOs
{
    /// <summary>
    /// Data transfer object for updating an existing document.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UpdateDocumentDto
    {
        /// <summary>
        /// Gets or sets the title of the document.
        /// </summary>
        [MaxLength(200)]
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the description of the document.
        /// </summary>
        [MaxLength(2000)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the URL where the document file is stored.
        /// </summary>
        [MaxLength(2000)]
        public string? FileUrl { get; set; }
    }
}
