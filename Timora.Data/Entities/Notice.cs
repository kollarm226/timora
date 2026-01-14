using System.Diagnostics.CodeAnalysis;

namespace Timora.Data.Entities
{
    /// <summary>
    /// Represents a notice or announcement that can be created and viewed by users in the system.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Notice
    {
        /// <summary>
        /// Gets or sets the unique identifier for the notice.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who created this notice.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the title of the notice.
        /// </summary>
        public string Title { get; set; } = null!;

        /// <summary>
        /// Gets or sets the content/body of the notice.
        /// </summary>
        public string Content { get; set; } = null!;

        /// <summary>
        /// Gets or sets when the notice was created. Defaults to UTC now.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the user who created this notice.
        /// </summary>
        public User User { get; set; } = null!;
    }
}
