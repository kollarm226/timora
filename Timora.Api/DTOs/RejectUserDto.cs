using System.Diagnostics.CodeAnalysis;

namespace Timora.Api.DTOs
{
    /// <summary>
    /// Data transfer object for user rejection operation.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RejectUserDto
    {
        /// <summary>
        /// Gets or sets the reason for rejection.
        /// </summary>
        public string? Reason { get; set; }
    }
}
