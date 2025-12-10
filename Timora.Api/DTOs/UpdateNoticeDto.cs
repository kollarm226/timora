using System.ComponentModel.DataAnnotations;

namespace Timora.Api.DTOs
{
    /// <summary>
    /// Data transfer object for partially updating a notice.
    /// All properties are nullable to support partial updates.
    /// </summary>
    public class UpdateNoticeDto
    {
        /// <summary>
        /// Gets or sets the title of the notice.
        /// </summary>
        [MaxLength(200)]
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the content/body of the notice.
        /// </summary>
        [MaxLength(5000)]
        public string? Content { get; set; }
    }
}
