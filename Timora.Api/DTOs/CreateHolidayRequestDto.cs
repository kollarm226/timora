using System.ComponentModel.DataAnnotations;

namespace Timora.Api.DTOs
{
    /// <summary>
    /// Data transfer object for creating a new holiday request.
    /// </summary>
    public class CreateHolidayRequestDto
    {
        /// <summary>
        /// Gets or sets the ID of the user submitting the request.
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the start date of the holiday period.
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date of the holiday period.
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets the reason for the holiday request.
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } = null!;
    }
}
