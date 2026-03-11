namespace CleanDDDArchitecture.Domains.Account.Application.Identity;

public static class AccountSecurityEventTypes
{
    public const string PasswordResetRequested = nameof(PasswordResetRequested);
    public const string PasswordResetCompleted = nameof(PasswordResetCompleted);
    public const string PasswordChanged = nameof(PasswordChanged);
    public const string EmailConfirmationRequested = nameof(EmailConfirmationRequested);
    public const string EmailConfirmed = nameof(EmailConfirmed);
    public const string EmailChangeRequested = nameof(EmailChangeRequested);
    public const string EmailChanged = nameof(EmailChanged);
    public const string MfaEnabled = nameof(MfaEnabled);
    public const string MfaDisabled = nameof(MfaDisabled);
    public const string MfaRecoveryCodesRegenerated = nameof(MfaRecoveryCodesRegenerated);
    public const string AccountDeactivated = nameof(AccountDeactivated);
    public const string AccountSuspended = nameof(AccountSuspended);
    public const string AccountUnsuspended = nameof(AccountUnsuspended);
    public const string AccountUnlocked = nameof(AccountUnlocked);
    public const string SessionsRevoked = nameof(SessionsRevoked);
    public const string SessionRevoked = nameof(SessionRevoked);
    public const string RolesUpdated = nameof(RolesUpdated);
    public const string ClaimsUpdated = nameof(ClaimsUpdated);
    public const string ExternalLoginLinked = nameof(ExternalLoginLinked);
    public const string ExternalLoginUnlinked = nameof(ExternalLoginUnlinked);
    public const string ExternalLoginSignedIn = nameof(ExternalLoginSignedIn);
    public const string ExternalAccountCreated = nameof(ExternalAccountCreated);
}
