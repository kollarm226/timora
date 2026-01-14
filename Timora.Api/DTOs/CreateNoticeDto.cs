using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Timora.Api.DTOs
{
    /// <summary>
    /// Data transfer object for creating a new notice.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class CreateNoticeDto
    {
        /// <summary>
        /// Gets or sets the ID of the user creating the notice.
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the title of the notice.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = null!;

        /// <summary>
        /// Gets or sets the content of the notice.
        /// </summary>
        [Required]
        [MaxLength(2000)]
        public string Content { get; set; } = null!;
    }
}
