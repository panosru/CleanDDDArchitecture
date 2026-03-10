using Aviant.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Core;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;
using CleanDDDArchitecture.Domains.Account.Infrastructure.Identity.Mechanism;
using CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Contexts;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Json;
using Serilog;
using IdentityResult = Aviant.Application.Identity.IdentityResult;

namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Identity;

public sealed class IdentityService : IIdentityService, IAccountAuthenticationService
{
    private const int EmailChangeTokenLifetimeHours = 24;
    private static readonly string[] UserNotFoundErrors = ["User not found."];
    private static readonly string[] InvalidEmailChangeTokenErrors = ["Invalid token."];

    private readonly Authenticator _authenticator;
    private readonly ConfirmEmail _confirmEmail;
    private readonly IDataProtector _emailChangeProtector;
    private readonly RefreshSessionManager _refreshSessionManager;
    private readonly RoleManager<AccountRole> _roleManager;
    private readonly UserManager<AccountUser> _userManager;

    public IdentityService(
        UserManager<AccountUser> userManager,
        RoleManager<AccountRole> roleManager,
        IAccountDomainConfiguration config,
        AccountDbContextWrite accountDbContextWrite,
        IDataProtectionProvider dataProtectionProvider,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _refreshSessionManager = new RefreshSessionManager(accountDbContextWrite, config, httpContextAccessor);
        _authenticator = new Authenticator(userManager, _refreshSessionManager);
        _confirmEmail = new ConfirmEmail(userManager);
        _emailChangeProtector = dataProtectionProvider.CreateProtector("CleanDDDArchitecture.Account.EmailChange");
    }

    public async Task<object?> AuthenticateAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default) =>
        await _authenticator.AuthenticateAsync(username, password, cancellationToken).ConfigureAwait(false);

    public async Task<EmailConfirmationTicket?> GenerateEmailConfirmationAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);

        if (user is null || user.EmailConfirmed)
            return null;

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        return new EmailConfirmationTicket(user.Email!, user.FullName, encodedToken);
    }

    public async Task<EmailChangeTicket?> GenerateEmailChangeAsync(
        Guid userId,
        string newEmail,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString()).ConfigureAwait(false);

        if (user is null)
            return null;

        if (string.Equals(user.Email, newEmail, StringComparison.OrdinalIgnoreCase))
            return null;

        if (await _userManager.FindByEmailAsync(newEmail).ConfigureAwait(false) is not null)
            return null;

        EmailChangeTokenPayload payload = new(
            user.Id,
            user.Email!,
            newEmail,
            user.SecurityStamp ?? string.Empty,
            DateTimeOffset.UtcNow.AddHours(EmailChangeTokenLifetimeHours));
        var protectedBytes = _emailChangeProtector.Protect(JsonSerializer.SerializeToUtf8Bytes(payload));
        var encodedToken = WebEncoders.Base64UrlEncode(protectedBytes);

        return new EmailChangeTicket(user.Email!, newEmail, user.FullName, encodedToken);
    }

    public async Task<PasswordResetTicket?> GeneratePasswordResetAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);

        if (user is null || !user.EmailConfirmed)
            return null;

        var token = await _userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        return new PasswordResetTicket(user.Id, user.Email!, user.FullName, encodedToken);
    }

    public async Task<IdentityResult> ResetPasswordAsync(
        string email,
        string token,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);

        if (user is null)
            return IdentityResult.Failure(UserNotFoundErrors);

        var decodedToken = DecodeToken(token);
        var result = await _userManager.ResetPasswordAsync(user, decodedToken, newPassword).ConfigureAwait(false);

        if (!result.Succeeded)
            return result.ToApplicationResult();

        await _userManager.UpdateSecurityStampAsync(user).ConfigureAwait(false);
        await _refreshSessionManager.RevokeAllRefreshTokensAsync(user.Id, cancellationToken).ConfigureAwait(false);

        return IdentityResult.Success();
    }

    public async Task<IdentityResult> ChangePasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString()).ConfigureAwait(false);

        if (user is null)
            return IdentityResult.Failure(UserNotFoundErrors);

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword).ConfigureAwait(false);

        if (!result.Succeeded)
            return result.ToApplicationResult();

        await _userManager.UpdateSecurityStampAsync(user).ConfigureAwait(false);
        await _refreshSessionManager.RevokeAllRefreshTokensAsync(user.Id, cancellationToken).ConfigureAwait(false);

        return IdentityResult.Success();
    }

    public async Task<AuthResult?> RefreshAsync(
        string refreshToken,
        CancellationToken cancellationToken = default) =>
        await _refreshSessionManager.RefreshAsync(refreshToken, cancellationToken).ConfigureAwait(false);

    public async Task<bool> RevokeRefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default) =>
        await _refreshSessionManager.RevokeRefreshTokenAsync(refreshToken, cancellationToken).ConfigureAwait(false);

    public async Task<int> RevokeAllRefreshTokensAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await _refreshSessionManager.RevokeAllRefreshTokensAsync(userId, cancellationToken).ConfigureAwait(false);

    public async Task<IReadOnlyCollection<AccountSessionDto>> GetActiveSessionsAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await _refreshSessionManager.GetActiveSessionsAsync(userId, cancellationToken).ConfigureAwait(false);

    public async Task<bool> RevokeSessionAsync(
        Guid userId,
        Guid sessionId,
        CancellationToken cancellationToken = default) =>
        await _refreshSessionManager.RevokeSessionAsync(userId, sessionId, cancellationToken).ConfigureAwait(false);

    public async Task<IdentityResult> ConfirmEmailAsync(
        string token,
        string email,
        CancellationToken cancellationToken = default) =>
        await _confirmEmail.ConfirmEmailAsync(token, email).ConfigureAwait(false);

    public async Task<IdentityResult> ConfirmEmailChangeAsync(
        string currentEmail,
        string newEmail,
        string token,
        CancellationToken cancellationToken = default)
    {
        var payload = DecodeEmailChangeToken(token);
        if (payload is null
         || payload.ExpiresAtUtc <= DateTimeOffset.UtcNow
         || !string.Equals(payload.CurrentEmail, currentEmail, StringComparison.OrdinalIgnoreCase)
         || !string.Equals(payload.NewEmail, newEmail, StringComparison.OrdinalIgnoreCase))
        {
            Log.Warning(
                "Email change token validation failed before user lookup. CurrentEmail={CurrentEmail} NewEmail={NewEmail} HasPayload={HasPayload} ExpiresAt={ExpiresAtUtc} PayloadCurrentEmail={PayloadCurrentEmail} PayloadNewEmail={PayloadNewEmail}",
                currentEmail,
                newEmail,
                payload is not null,
                payload?.ExpiresAtUtc,
                payload?.CurrentEmail,
                payload?.NewEmail);
            return IdentityResult.Failure(InvalidEmailChangeTokenErrors);
        }

        var user = await _userManager.FindByIdAsync(payload.UserId.ToString()).ConfigureAwait(false);

        if (user is null)
        {
            var alreadyChangedUser = await _userManager.FindByEmailAsync(newEmail).ConfigureAwait(false);

            if (alreadyChangedUser is not null
             && alreadyChangedUser.Id == payload.UserId
             && alreadyChangedUser.EmailConfirmed
             && string.Equals(alreadyChangedUser.UserName, newEmail, StringComparison.OrdinalIgnoreCase))
                return IdentityResult.Success();

            return IdentityResult.Failure(UserNotFoundErrors);
        }

        if (!string.Equals(user.SecurityStamp, payload.SecurityStamp, StringComparison.Ordinal))
        {
            Log.Warning(
                "Email change token rejected due to security stamp mismatch. UserId={UserId} CurrentStamp={CurrentStamp} PayloadStamp={PayloadStamp}",
                user.Id,
                user.SecurityStamp,
                payload.SecurityStamp);
            return IdentityResult.Failure(InvalidEmailChangeTokenErrors);
        }

        if (string.Equals(user.Email, newEmail, StringComparison.OrdinalIgnoreCase)
         && string.Equals(user.UserName, newEmail, StringComparison.OrdinalIgnoreCase)
         && user.EmailConfirmed)
            return IdentityResult.Success();

        if (!string.Equals(user.Email, currentEmail, StringComparison.OrdinalIgnoreCase))
        {
            Log.Warning(
                "Email change token rejected due to stale current email. UserId={UserId} ActualEmail={ActualEmail} ExpectedEmail={ExpectedEmail}",
                user.Id,
                user.Email,
                currentEmail);
            return IdentityResult.Failure(InvalidEmailChangeTokenErrors);
        }

        var existingUser = await _userManager.FindByEmailAsync(newEmail).ConfigureAwait(false);
        if (existingUser is not null && existingUser.Id != user.Id)
            return IdentityResult.Failure([_userManager.ErrorDescriber.DuplicateEmail(newEmail).Description]);

        user.Email = newEmail;
        user.NormalizedEmail = _userManager.NormalizeEmail(newEmail);
        user.UserName = newEmail;
        user.NormalizedUserName = _userManager.NormalizeName(newEmail);
        user.EmailConfirmed = true;

        var updateResult = await _userManager.UpdateAsync(user).ConfigureAwait(false);
        if (!updateResult.Succeeded)
            return updateResult.ToApplicationResult();

        await _userManager.UpdateSecurityStampAsync(user).ConfigureAwait(false);
        await _refreshSessionManager.RevokeAllRefreshTokensAsync(user.Id, cancellationToken).ConfigureAwait(false);

        return IdentityResult.Success();
    }

    public async Task<string> GetUserNameAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users.FirstAsync(item => item.Id == userId, cancellationToken)
            .ConfigureAwait(false);

        return user.UserName;
    }

    public async Task<Guid?> GetUserIdByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);

        return user?.Id;
    }

    public async Task<(IdentityResult Result, Guid UserId)> CreateUserAsync(
        string username,
        string password,
        string firstName,
        string lastName,
        IEnumerable<string> roles,
        bool emailConfirmed,
        CancellationToken cancellationToken = default)
    {
        AccountUser user = new()
        {
            UserName = username,
            Email = username,
            FirstName = firstName,
            LastName = lastName,
            EmailConfirmed = emailConfirmed
        };

        var result = await _userManager.CreateAsync(user, password).ConfigureAwait(false);

        if (!result.Succeeded)
            return (result.ToApplicationResult(), user.Id);

        var normalizedRoles = roles
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (normalizedRoles.Length == 0)
            return (IdentityResult.Success(), user.Id);

        foreach (var role in normalizedRoles)
        {
            if (await _roleManager.RoleExistsAsync(role).ConfigureAwait(false))
                continue;

            var createRoleResult = await _roleManager.CreateAsync(new AccountRole { Name = role })
                .ConfigureAwait(false);

            if (!createRoleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user).ConfigureAwait(false);
                return (createRoleResult.ToApplicationResult(), Guid.Empty);
            }
        }

        var addRolesResult = await _userManager.AddToRolesAsync(user, normalizedRoles).ConfigureAwait(false);

        if (addRolesResult.Succeeded)
            return (IdentityResult.Success(), user.Id);

        await _userManager.DeleteAsync(user).ConfigureAwait(false);

        return (addRolesResult.ToApplicationResult(), Guid.Empty);
    }

    public async Task<IdentityResult> DeleteUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString()).ConfigureAwait(false);

        if (user is null)
            return IdentityResult.Failure(UserNotFoundErrors);

        return await DeleteUserAsync(user, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IdentityResult> DeleteUserAsync(
        AccountUser user,
        CancellationToken cancellationToken = default)
    {
        var result = await _userManager.DeleteAsync(user).ConfigureAwait(false);

        return result.ToApplicationResult();
    }

    private static string DecodeToken(string token)
    {
        try
        {
            return Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        }
        catch (FormatException)
        {
            return token;
        }
    }

    private EmailChangeTokenPayload? DecodeEmailChangeToken(string token)
    {
        try
        {
            var protectedBytes = WebEncoders.Base64UrlDecode(token);
            var bytes = _emailChangeProtector.Unprotect(protectedBytes);

            return JsonSerializer.Deserialize<EmailChangeTokenPayload>(bytes);
        }
        catch
        {
            Log.Warning("Failed to decode email change token.");
            return null;
        }
    }

    private sealed record EmailChangeTokenPayload(
        Guid UserId,
        string CurrentEmail,
        string NewEmail,
        string SecurityStamp,
        DateTimeOffset ExpiresAtUtc);
}
