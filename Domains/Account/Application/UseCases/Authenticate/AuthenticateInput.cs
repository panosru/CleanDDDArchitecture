using Aviant.Application.UseCases;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Authenticate;

public sealed record AuthenticateInput(
    string Username,
    string Password,
    string? TwoFactorCode = null,
    string? RecoveryCode = null,
    string? TrustedDeviceToken = null,
    bool RememberDevice = false,
    string? DeviceName = null) : UseCaseInput
{
    internal string Username { get; } = Username;

    internal string Password { get; } = Password;

    internal string? TwoFactorCode { get; } = TwoFactorCode;

    internal string? RecoveryCode { get; } = RecoveryCode;

    internal string? TrustedDeviceToken { get; } = TrustedDeviceToken;

    internal bool RememberDevice { get; } = RememberDevice;

    internal string? DeviceName { get; } = DeviceName;
}
