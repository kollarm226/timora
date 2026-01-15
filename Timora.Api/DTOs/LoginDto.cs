using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Timora.Api.DTOs;

/// <summary>
/// Data transfer object for login validation.
/// </summary>
[ExcludeFromCodeCoverage]
public class LoginDto
{
    /// <summary>
    /// Gets or sets the user's username.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = null!;

    /// <summary>
    /// Gets or sets the user's password.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Password { get; set; } = null!;
}
