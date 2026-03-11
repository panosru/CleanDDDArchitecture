// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Authenticate;

public sealed class AuthenticateDto : IValidatableObject
{
    public string? Username { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [Required]
    public string Password { get; set; } = string.Empty;

    public string? TwoFactorCode { get; set; }

    public string? RecoveryCode { get; set; }

    public string? TrustedDeviceToken { get; set; }

    public bool RememberDevice { get; set; }

    public string? DeviceName { get; set; }

    public string LoginIdentifier =>
        !string.IsNullOrWhiteSpace(Email)
            ? Email
            : Username ?? string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Username) && string.IsNullOrWhiteSpace(Email))
        {
            yield return new ValidationResult(
                "Either username or email is required.",
                [nameof(Username), nameof(Email)]);
        }
    }
}
