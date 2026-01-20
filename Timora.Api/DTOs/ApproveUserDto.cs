using System.Diagnostics.CodeAnalysis;

namespace Timora.Api.DTOs
{
    /// <summary>
    /// Data transfer object for user approval operation.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ApproveUserDto
    {
        /// <summary>
        /// Gets or sets an optional comment from the approver.
        /// </summary>
        public string? Comment { get; set; }
    }
}
