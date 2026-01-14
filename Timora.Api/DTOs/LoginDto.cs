using System.ComponentModel.DataAnnotations;

namespace Timora.Api.DTOs;

/// <summary>
/// Data transfer object for login validation.
/// Contains the company ID that the user claims to be logging into.
/// Backend validates this against the user's actual company affiliation.
/// </summary>
public class LoginDto
{
    /// <summary>
    /// Gets or sets the company ID the user is attempting to log into.
    /// Must match the user's actual company ID from the database.
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Company ID must be a positive number.")]
    public int CompanyId { get; set; }
}
