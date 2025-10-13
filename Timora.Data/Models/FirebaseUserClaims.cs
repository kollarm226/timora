namespace Timora.Data.Models;

/// <summary>
/// Represents the claims extracted from a Firebase authentication token.
/// Used for validating and syncing Firebase users with the local User entity.
/// </summary>
public class FirebaseUserClaims
{
    /// <summary>
    /// Gets or sets the Firebase user unique identifier (UID).
    /// This is the primary key for linking Firebase users to User.FirebaseId.
    /// </summary>
    public string Uid { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's email address from Firebase token.
    /// Used for validation and initial user provisioning.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets whether the email has been verified in Firebase.
    /// Can be used to enforce email verification requirements.
    /// </summary>
    public bool EmailVerified { get; set; }
}
