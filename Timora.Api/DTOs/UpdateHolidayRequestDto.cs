using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Timora.Data.Entities;

namespace Timora.Api.DTOs
{
    /// <summary>
    /// Data transfer object for partially updating a holiday request.
    /// All properties are nullable to support partial updates.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UpdateHolidayRequestDto
    {
        /// <summary>
        /// Gets or sets the start date of the requested holiday period.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date of the requested holiday period.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the reason provided for the holiday request.
        /// </summary>
        [MaxLength(1000)]
        public string? Reason { get; set; }

        /// <summary>
        /// Gets or sets the current status of the holiday request.
        /// </summary>
        public HolidayRequestStatus? Status { get; set; }

        /// <summary>
        /// Gets or sets the ID of the user who approved or denied this request.
        /// </summary>
        public int? ResolvedByUserId { get; set; }

        /// <summary>
        /// Gets or sets an optional comment from the resolver explaining their decision.
        /// </summary>
        [MaxLength(1000)]
        public string? ResolverComment { get; set; }
    }
}
