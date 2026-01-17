using System.Diagnostics.CodeAnalysis;

namespace Timora.Data.Entities
{
    /// <summary>
    /// Represents a document that belongs to a company in the system.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Document
    {
        /// <summary>
        /// Gets or sets the unique identifier for the document.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the ID of the company that owns this document.
        /// </summary>
        public int CompanyId { get; set; }

        /// <summary>
        /// Gets or sets the title of the document.
        /// </summary>
        public string Title { get; set; } = null!;

        /// <summary>
        /// Gets or sets the description of the document.
        /// </summary>
        public string Description { get; set; } = null!;

        /// <summary>
        /// Gets or sets the URL where the document file is stored.
        /// </summary>
        public string FileUrl { get; set; } = null!;

        /// <summary>
        /// Gets or sets when the document was created. Defaults to UTC now.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the company that owns this document.
        /// </summary>
        public Company Company { get; set; } = null!;
    }
}
