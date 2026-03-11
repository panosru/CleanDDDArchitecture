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
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Serilog;
using System.Text.Encodings.Web;
using System.Globalization;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;
using IdentityResult = Aviant.Application.Identity.IdentityResult;

namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Identity;

public sealed class IdentityService : IIdentityService, IAccountAuthenticationService, IAccountAdministrationService
{
    private const int EmailChangeTokenLifetimeHours = 24;
    private const int RecoveryCodesCount = 10;
    private static readonly HashSet<string> ReservedClaimTypes =
    [
        ClaimTypes.Role,
        ClaimTypes.NameIdentifier,
        ClaimTypes.Name,
        ClaimTypes.Email,
        JwtRegisteredClaimNames.Sub,
        JwtRegisteredClaimNames.Jti,
        JwtRegisteredClaimNames.Iat,
        JwtRegisteredClaimNames.NameId,
        JwtRegisteredClaimNames.UniqueName,
        JwtRegisteredClaimNames.Email,
        JwtRegisteredClaimNames.GivenName,
        JwtRegisteredClaimNames.FamilyName,
        "sid",
        "family_id"
    ];
    private static readonly string[] AdminRoles = ["root", "superadmin", "admin"];
    private static readonly string[] UserNotFoundErrors = ["User not found."];
    private static readonly string[] InvalidEmailChangeTokenErrors = ["Invalid token."];

    private readonly AccountDbContextWrite _accountDbContextWrite;
    private readonly Authenticator _authenticator;
    private readonly ConfirmEmail _confirmEmail;
    private readonly IDataProtector _emailChangeProtector;
    private readonly IHttpContextAccessor _httpContextAccessor;
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
        _accountDbContextWrite = accountDbContextWrite;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _roleManager = roleManager;
        _refreshSessionManager = new RefreshSessionManager(accountDbContextWrite, config, httpContextAccessor, userManager);
        _authenticator = new Authenticator(userManager, _refreshSessionManager);
        _confirmEmail = new ConfirmEmail(userManager);
        _emailChangeProtector = dataProtectionProvider.CreateProtector("CleanDDDArchitecture.Account.EmailChange");
    }

    public async Task<object?> AuthenticateAsync(
        string username,
        string password,
        string? twoFactorCode = null,
        string? recoveryCode = null,
        CancellationToken cancellationToken = default) =>
        await _authenticator
            .AuthenticateAsync(username, password, twoFactorCode, recoveryCode, cancellationToken)
            .ConfigureAwait(false);

    public async Task<MfaSetupTicket?> BeginMfaSetupAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString()).ConfigureAwait(false);

        if (user is null)
            return null;

        var key = await _userManager.GetAuthenticatorKeyAsync(user).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(key))
        {
            await _userManager.ResetAuthenticatorKeyAsync(user).ConfigureAwait(false);
            key = await _userManager.GetAuthenticatorKeyAsync(user).ConfigureAwait(false);
        }

        if (string.IsNullOrWhiteSpace(key))
            return null;

        return new MfaSetupTicket(
            FormatAuthenticatorKey(key),
            GenerateQrCodeUri(user.Email!, key),
            user.TwoFactorEnabled);
    }

    public async Task<MfaRecoveryCodesTicket?> EnableMfaAsync(
        Guid userId,
        string code,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString()).ConfigureAwait(false);

        if (user is null)
            return null;

        var normalizedCode = code.Replace(" ", string.Empty, StringComparison.Ordinal)
            .Replace("-", string.Empty, StringComparison.Ordinal);
        var isValid = await _userManager.VerifyTwoFactorTokenAsync(
                user,
                _userManager.Options.Tokens.AuthenticatorTokenProvider,
                normalizedCode)
            .ConfigureAwait(false);

        if (!isValid)
            return null;

        var enableResult = await _userManager.SetTwoFactorEnabledAsync(user, true).ConfigureAwait(false);
        if (!enableResult.Succeeded)
            return null;

        var recoveryCodes = (await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, RecoveryCodesCount)
                .ConfigureAwait(false))
            .ToArray();

        await _userManager.UpdateSecurityStampAsync(user).ConfigureAwait(false);
        await _refreshSessionManager.RevokeAllRefreshTokensAsync(user.Id, cancellationToken).ConfigureAwait(false);
        await RecordSecurityEventAsync(
                user.Id,
                user.Id,
                AccountSecurityEventTypes.MfaEnabled,
                "Authenticator app MFA was enabled.",
                cancellationToken)
            .ConfigureAwait(false);

        return new MfaRecoveryCodesTicket(recoveryCodes);
    }

    public async Task<IdentityResult> DisableMfaAsync(
        Guid userId,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString()).ConfigureAwait(false);

        if (user is null)
            return IdentityResult.Failure(UserNotFoundErrors);

        if (!await _userManager.CheckPasswordAsync(user, password).ConfigureAwait(false))
            return IdentityResult.Failure(["Invalid current password."]);

        if (!await _userManager.GetTwoFactorEnabledAsync(user).ConfigureAwait(false))
            return IdentityResult.Success();

        var disableResult = await _userManager.SetTwoFactorEnabledAsync(user, false).ConfigureAwait(false);
        if (!disableResult.Succeeded)
            return disableResult.ToApplicationResult();

        await _userManager.ResetAuthenticatorKeyAsync(user).ConfigureAwait(false);
        await _userManager.UpdateSecurityStampAsync(user).ConfigureAwait(false);
        await _refreshSessionManager.RevokeAllRefreshTokensAsync(user.Id, cancellationToken).ConfigureAwait(false);
        await RecordSecurityEventAsync(
                user.Id,
                user.Id,
                AccountSecurityEventTypes.MfaDisabled,
                "Authenticator app MFA was disabled.",
                cancellationToken)
            .ConfigureAwait(false);

        return IdentityResult.Success();
    }

    public async Task<MfaRecoveryCodesTicket?> RegenerateRecoveryCodesAsync(
        Guid userId,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString()).ConfigureAwait(false);

        if (user is null)
            return null;

        if (!await _userManager.CheckPasswordAsync(user, password).ConfigureAwait(false))
            return null;

        if (!await _userManager.GetTwoFactorEnabledAsync(user).ConfigureAwait(false))
            return null;

        var recoveryCodes = (await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, RecoveryCodesCount)
                .ConfigureAwait(false))
            .ToArray();

        await _userManager.UpdateSecurityStampAsync(user).ConfigureAwait(false);
        await _refreshSessionManager.RevokeAllRefreshTokensAsync(user.Id, cancellationToken).ConfigureAwait(false);
        await RecordSecurityEventAsync(
                user.Id,
                user.Id,
                AccountSecurityEventTypes.MfaRecoveryCodesRegenerated,
                "Recovery codes were regenerated.",
                cancellationToken)
            .ConfigureAwait(false);

        return new MfaRecoveryCodesTicket(recoveryCodes);
    }

    public async Task<IdentityResult> DeactivateAsync(
        Guid userId,
        string currentPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString()).ConfigureAwait(false);

        if (user is null)
            return IdentityResult.Failure(UserNotFoundErrors);

        if (!await _userManager.CheckPasswordAsync(user, currentPassword).ConfigureAwait(false))
            return IdentityResult.Failure(["Invalid current password."]);

        if (user.Status == AccountStatus.Deactivated)
            return IdentityResult.Success();

        return await ApplyStatusAsync(
                user,
                AccountStatus.Deactivated,
                "Self deactivated.",
                shouldLockOut: true,
                actorUserId: user.Id,
                cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IdentityResult> SuspendAsync(
        Guid actorUserId,
        Guid targetUserId,
        string? reason,
        CancellationToken cancellationToken = default)
    {
        if (!await IsAdministratorAsync(actorUserId).ConfigureAwait(false))
            return IdentityResult.Failure(["Administrator access is required."]);

        if (actorUserId == targetUserId)
            return IdentityResult.Failure(["Administrators cannot suspend themselves."]);

        var user = await _userManager.FindByIdAsync(targetUserId.ToString()).ConfigureAwait(false);

        if (user is null)
            return IdentityResult.Failure(UserNotFoundErrors);

        if (user.Status == AccountStatus.Deactivated)
            return IdentityResult.Failure(["Deactivated accounts cannot be suspended."]);

        if (user.Status == AccountStatus.Suspended)
            return IdentityResult.Success();

        return await ApplyStatusAsync(
                user,
                AccountStatus.Suspended,
                string.IsNullOrWhiteSpace(reason) ? "Suspended by administrator." : reason.Trim(),
                shouldLockOut: true,
                actorUserId: actorUserId,
                cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IdentityResult> UnsuspendAsync(
        Guid actorUserId,
        Guid targetUserId,
        CancellationToken cancellationToken = default)
    {
        if (!await IsAdministratorAsync(actorUserId).ConfigureAwait(false))
            return IdentityResult.Failure(["Administrator access is required."]);

        var user = await _userManager.FindByIdAsync(targetUserId.ToString()).ConfigureAwait(false);

        if (user is null)
            return IdentityResult.Failure(UserNotFoundErrors);

        if (user.Status == AccountStatus.Deactivated)
            return IdentityResult.Failure(["Deactivated accounts cannot be unsuspended."]);

        if (user.Status != AccountStatus.Suspended)
            return IdentityResult.Success();

        return await ApplyStatusAsync(
                user,
                AccountStatus.Active,
                "Unsuspended by administrator.",
                shouldLockOut: false,
                actorUserId: actorUserId,
                cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IdentityResult> UnlockAsync(
        Guid actorUserId,
        Guid targetUserId,
        CancellationToken cancellationToken = default)
    {
        if (!await IsAdministratorAsync(actorUserId).ConfigureAwait(false))
            return IdentityResult.Failure(["Administrator access is required."]);

        var user = await _userManager.FindByIdAsync(targetUserId.ToString()).ConfigureAwait(false);

        if (user is null)
            return IdentityResult.Failure(UserNotFoundErrors);

        if (_userManager.SupportsUserLockout)
        {
            await _userManager.SetLockoutEndDateAsync(user, null).ConfigureAwait(false);
            await _userManager.ResetAccessFailedCountAsync(user).ConfigureAwait(false);
        }

        await RecordSecurityEventAsync(
                user.Id,
                actorUserId,
                AccountSecurityEventTypes.AccountUnlocked,
                "Account was unlocked by an administrator.",
                cancellationToken)
            .ConfigureAwait(false);

        return IdentityResult.Success();
    }

    public async Task<IReadOnlyCollection<string>?> GetRolesAsync(
        Guid actorUserId,
        Guid targetUserId,
        CancellationToken cancellationToken = default)
    {
        if (!await IsAdministratorAsync(actorUserId).ConfigureAwait(false))
            return null;

        var user = await _userManager.FindByIdAsync(targetUserId.ToString()).ConfigureAwait(false);
        if (user is null)
            return null;

        return (await _userManager.GetRolesAsync(user).ConfigureAwait(false))
            .OrderBy(role => role, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public async Task<IdentityResult> ReplaceRolesAsync(
        Guid actorUserId,
        Guid targetUserId,
        IEnumerable<string> roles,
        CancellationToken cancellationToken = default)
    {
        if (!await IsAdministratorAsync(actorUserId).ConfigureAwait(false))
            return IdentityResult.Failure(["Administrator access is required."]);

        if (actorUserId == targetUserId)
            return IdentityResult.Failure(["Administrators cannot change their own role assignments."]);

        var user = await _userManager.FindByIdAsync(targetUserId.ToString()).ConfigureAwait(false);
        if (user is null)
            return IdentityResult.Failure(UserNotFoundErrors);

        var normalizedRoles = roles
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Select(role => role.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(role => role, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        foreach (var role in normalizedRoles)
        {
            if (await _roleManager.RoleExistsAsync(role).ConfigureAwait(false))
                continue;

            var createRoleResult = await _roleManager.CreateAsync(new AccountRole { Name = role }).ConfigureAwait(false);
            if (!createRoleResult.Succeeded)
                return createRoleResult.ToApplicationResult();
        }

        var currentRoles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
        var currentRoleSet = currentRoles.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var targetRoleSet = normalizedRoles.ToHashSet(StringComparer.OrdinalIgnoreCase);

        var rolesToRemove = currentRoles.Where(role => !targetRoleSet.Contains(role)).ToArray();
        var rolesToAdd = normalizedRoles.Where(role => !currentRoleSet.Contains(role)).ToArray();

        if (rolesToRemove.Length > 0)
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove).ConfigureAwait(false);
            if (!removeResult.Succeeded)
                return removeResult.ToApplicationResult();
        }

        if (rolesToAdd.Length > 0)
        {
            var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd).ConfigureAwait(false);
            if (!addResult.Succeeded)
                return addResult.ToApplicationResult();
        }

        await _userManager.UpdateSecurityStampAsync(user).ConfigureAwait(false);
        await _refreshSessionManager.RevokeAllRefreshTokensAsync(user.Id, cancellationToken).ConfigureAwait(false);
        await RecordSecurityEventAsync(
                user.Id,
                actorUserId,
                AccountSecurityEventTypes.RolesUpdated,
                normalizedRoles.Length == 0
                    ? "All role assignments were removed by an administrator."
                    : $"Role assignments were updated to: {string.Join(", ", normalizedRoles)}.",
                cancellationToken)
            .ConfigureAwait(false);

        return IdentityResult.Success();
    }

    public async Task<IReadOnlyCollection<AccountClaimDto>?> GetClaimsAsync(
        Guid actorUserId,
        Guid targetUserId,
        CancellationToken cancellationToken = default)
    {
        if (!await IsAdministratorAsync(actorUserId).ConfigureAwait(false))
            return null;

        var user = await _userManager.FindByIdAsync(targetUserId.ToString()).ConfigureAwait(false);
        if (user is null)
            return null;

        return (await _userManager.GetClaimsAsync(user).ConfigureAwait(false))
            .Where(claim => !ReservedClaimTypes.Contains(claim.Type))
            .Select(claim => new AccountClaimDto(claim.Type, claim.Value))
            .OrderBy(claim => claim.Type, StringComparer.OrdinalIgnoreCase)
            .ThenBy(claim => claim.Value, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public async Task<IdentityResult> ReplaceClaimsAsync(
        Guid actorUserId,
        Guid targetUserId,
        IEnumerable<AccountClaimDto> claims,
        CancellationToken cancellationToken = default)
    {
        if (!await IsAdministratorAsync(actorUserId).ConfigureAwait(false))
            return IdentityResult.Failure(["Administrator access is required."]);

        if (actorUserId == targetUserId)
            return IdentityResult.Failure(["Administrators cannot change their own direct claims."]);

        var user = await _userManager.FindByIdAsync(targetUserId.ToString()).ConfigureAwait(false);
        if (user is null)
            return IdentityResult.Failure(UserNotFoundErrors);

        var normalizedClaims = claims
            .Where(claim => !string.IsNullOrWhiteSpace(claim.Type) && !string.IsNullOrWhiteSpace(claim.Value))
            .Select(claim => new AccountClaimDto(claim.Type.Trim(), claim.Value.Trim()))
            .Distinct()
            .OrderBy(claim => claim.Type, StringComparer.OrdinalIgnoreCase)
            .ThenBy(claim => claim.Value, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var reservedType = normalizedClaims.FirstOrDefault(claim => ReservedClaimTypes.Contains(claim.Type));
        if (reservedType is not null)
            return IdentityResult.Failure([$"Claim type '{reservedType.Type}' is reserved and cannot be assigned directly."]);

        var currentClaims = await _userManager.GetClaimsAsync(user).ConfigureAwait(false);
        var removableClaims = currentClaims.Where(claim => !ReservedClaimTypes.Contains(claim.Type)).ToArray();
        if (removableClaims.Length > 0)
        {
            var removeResult = await _userManager.RemoveClaimsAsync(user, removableClaims).ConfigureAwait(false);
            if (!removeResult.Succeeded)
                return removeResult.ToApplicationResult();
        }

        if (normalizedClaims.Length > 0)
        {
            var addResult = await _userManager.AddClaimsAsync(
                    user,
                    normalizedClaims.Select(claim => new Claim(claim.Type, claim.Value)))
                .ConfigureAwait(false);
            if (!addResult.Succeeded)
                return addResult.ToApplicationResult();
        }

        await _userManager.UpdateSecurityStampAsync(user).ConfigureAwait(false);
        await _refreshSessionManager.RevokeAllRefreshTokensAsync(user.Id, cancellationToken).ConfigureAwait(false);
        await RecordSecurityEventAsync(
                user.Id,
                actorUserId,
                AccountSecurityEventTypes.ClaimsUpdated,
                normalizedClaims.Length == 0
                    ? "All direct user claims were removed by an administrator."
                    : $"Direct user claims were updated ({normalizedClaims.Length} claims).",
                cancellationToken)
            .ConfigureAwait(false);

        return IdentityResult.Success();
    }

    public async Task<IReadOnlyCollection<AccountSecurityEventDto>> GetOwnSecurityEventsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _accountDbContextWrite.SecurityEvents
            .Where(item => item.UserId == userId)
            .OrderByDescending(item => item.OccurredAtUtc)
            .Take(100)
            .Select(
                item =>
                    new AccountSecurityEventDto
                    {
                        Id = item.Id,
                        UserId = item.UserId,
                        ActorUserId = item.ActorUserId,
                        Type = item.Type,
                        Description = item.Description,
                        OccurredAtUtc = item.OccurredAtUtc,
                        IpAddress = item.IpAddress,
                        UserAgent = item.UserAgent
                    })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<AccountSecurityEventDto>?> GetSecurityEventsAsync(
        Guid actorUserId,
        Guid targetUserId,
        CancellationToken cancellationToken = default)
    {
        if (!await IsAdministratorAsync(actorUserId).ConfigureAwait(false))
            return null;

        var userExists = await _userManager.FindByIdAsync(targetUserId.ToString()).ConfigureAwait(false) is not null;
        if (!userExists)
            return null;

        return await GetOwnSecurityEventsAsync(targetUserId, cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<AccountAdminSummaryDto>?> SearchAccountsAsync(
        Guid actorUserId,
        string? query,
        string? status,
        string? role,
        CancellationToken cancellationToken = default)
    {
        if (!await IsAdministratorAsync(actorUserId).ConfigureAwait(false))
            return null;

        IQueryable<AccountUser> usersQuery = _userManager.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var trimmed = query.Trim();
            usersQuery = usersQuery.Where(
                user =>
                    (user.UserName != null && EF.Functions.ILike(user.UserName, $"%{trimmed}%"))
                 || (user.Email != null && EF.Functions.ILike(user.Email, $"%{trimmed}%"))
                 || EF.Functions.ILike(user.FirstName, $"%{trimmed}%")
                 || EF.Functions.ILike(user.LastName, $"%{trimmed}%"));
        }

        if (!string.IsNullOrWhiteSpace(status)
         && Enum.TryParse<AccountStatus>(status, true, out var parsedStatus))
            usersQuery = usersQuery.Where(user => user.Status == parsedStatus);

        var users = await usersQuery
            .OrderBy(user => user.Email)
            .Take(100)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        List<AccountAdminSummaryDto> result = [];
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(role)
             && !roles.Any(item => string.Equals(item, role.Trim(), StringComparison.OrdinalIgnoreCase)))
                continue;

            result.Add(
                new AccountAdminSummaryDto
                {
                    Id = user.Id,
                    Username = user.UserName ?? string.Empty,
                    FirstName = user.FirstName ?? string.Empty,
                    LastName = user.LastName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    EmailConfirmed = user.EmailConfirmed,
                    Status = user.Status.ToString(),
                    LastAccessed = user.LastAccessed,
                    Roles = roles.OrderBy(item => item, StringComparer.OrdinalIgnoreCase).ToArray()
                });
        }

        return result;
    }

    public async Task<EmailConfirmationTicket?> GenerateEmailConfirmationForUserAsync(
        Guid actorUserId,
        Guid targetUserId,
        CancellationToken cancellationToken = default)
    {
        if (!await IsAdministratorAsync(actorUserId).ConfigureAwait(false))
            return null;

        var user = await _userManager.FindByIdAsync(targetUserId.ToString()).ConfigureAwait(false);
        if (user is null || user.EmailConfirmed)
            return null;

        var ticket = await GenerateEmailConfirmationAsync(user.Email!, cancellationToken).ConfigureAwait(false);
        if (ticket is not null)
        {
            await RecordSecurityEventAsync(
                    user.Id,
                    actorUserId,
                    AccountSecurityEventTypes.EmailConfirmationRequested,
                    "Email confirmation was requested by an administrator.",
                    cancellationToken)
                .ConfigureAwait(false);
        }

        return ticket;
    }

    public async Task<PasswordResetTicket?> GeneratePasswordResetForUserAsync(
        Guid actorUserId,
        Guid targetUserId,
        CancellationToken cancellationToken = default)
    {
        if (!await IsAdministratorAsync(actorUserId).ConfigureAwait(false))
            return null;

        var user = await _userManager.FindByIdAsync(targetUserId.ToString()).ConfigureAwait(false);
        if (user is null)
            return null;

        var ticket = await GeneratePasswordResetAsync(user.Email!, cancellationToken).ConfigureAwait(false);
        if (ticket is not null)
        {
            await _refreshSessionManager.RevokeAllRefreshTokensAsync(user.Id, cancellationToken).ConfigureAwait(false);
            await RecordSecurityEventAsync(
                    user.Id,
                    actorUserId,
                    AccountSecurityEventTypes.PasswordResetRequested,
                    "Password reset was requested by an administrator.",
                    cancellationToken)
                .ConfigureAwait(false);
            await RecordSecurityEventAsync(
                    user.Id,
                    actorUserId,
                    AccountSecurityEventTypes.SessionsRevoked,
                    "All active sessions were revoked by an administrator due to a password reset.",
                    cancellationToken)
                .ConfigureAwait(false);
        }

        return ticket;
    }

    public async Task<int?> RevokeAllSessionsAsync(
        Guid actorUserId,
        Guid targetUserId,
        CancellationToken cancellationToken = default)
    {
        if (!await IsAdministratorAsync(actorUserId).ConfigureAwait(false))
            return null;

        var user = await _userManager.FindByIdAsync(targetUserId.ToString()).ConfigureAwait(false);
        if (user is null)
            return null;

        var revoked = await _refreshSessionManager.RevokeAllRefreshTokensAsync(user.Id, cancellationToken).ConfigureAwait(false);
        await RecordSecurityEventAsync(
                user.Id,
                actorUserId,
                AccountSecurityEventTypes.SessionsRevoked,
                revoked == 0
                    ? "An administrator requested session revocation, but there were no active sessions."
                    : $"An administrator revoked all active sessions ({revoked} sessions).",
                cancellationToken)
            .ConfigureAwait(false);

        return revoked;
    }

    public async Task<EmailConfirmationTicket?> GenerateEmailConfirmationAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);

        if (user is null || user.EmailConfirmed)
            return null;

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        await RecordSecurityEventAsync(
                user.Id,
                user.Id,
                AccountSecurityEventTypes.EmailConfirmationRequested,
                "Email confirmation was requested.",
                cancellationToken)
            .ConfigureAwait(false);

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
        await RecordSecurityEventAsync(
                user.Id,
                user.Id,
                AccountSecurityEventTypes.EmailChangeRequested,
                $"Email change was requested to '{newEmail}'.",
                cancellationToken)
            .ConfigureAwait(false);

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
        await RecordSecurityEventAsync(
                user.Id,
                user.Id,
                AccountSecurityEventTypes.PasswordResetRequested,
                "Password reset was requested.",
                cancellationToken)
            .ConfigureAwait(false);

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
        await RecordSecurityEventAsync(
                user.Id,
                user.Id,
                AccountSecurityEventTypes.PasswordResetCompleted,
                "Password was reset successfully.",
                cancellationToken)
            .ConfigureAwait(false);

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
        await RecordSecurityEventAsync(
                user.Id,
                user.Id,
                AccountSecurityEventTypes.PasswordChanged,
                "Password was changed successfully.",
                cancellationToken)
            .ConfigureAwait(false);

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
        await RevokeAllAndRecordAsync(userId, cancellationToken).ConfigureAwait(false);

    public async Task<IReadOnlyCollection<AccountSessionDto>> GetActiveSessionsAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await _refreshSessionManager.GetActiveSessionsAsync(userId, cancellationToken).ConfigureAwait(false);

    public async Task<bool> RevokeSessionAsync(
        Guid userId,
        Guid sessionId,
        CancellationToken cancellationToken = default)
    {
        var revoked = await _refreshSessionManager.RevokeSessionAsync(userId, sessionId, cancellationToken).ConfigureAwait(false);
        if (revoked)
        {
            await RecordSecurityEventAsync(
                    userId,
                    userId,
                    AccountSecurityEventTypes.SessionRevoked,
                    $"Session '{sessionId}' was revoked.",
                    cancellationToken)
                .ConfigureAwait(false);
        }

        return revoked;
    }

    public async Task<IdentityResult> ConfirmEmailAsync(
        string token,
        string email,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
        var result = await _confirmEmail.ConfirmEmailAsync(token, email).ConfigureAwait(false);

        if (result.Succeeded && user is not null)
        {
            await RecordSecurityEventAsync(
                    user.Id,
                    user.Id,
                    AccountSecurityEventTypes.EmailConfirmed,
                    "Email address was confirmed.",
                    cancellationToken)
                .ConfigureAwait(false);
        }

        return result;
    }

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
        await RecordSecurityEventAsync(
                user.Id,
                user.Id,
                AccountSecurityEventTypes.EmailChanged,
                $"Primary email address was changed to '{newEmail}'.",
                cancellationToken)
            .ConfigureAwait(false);

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

    private static string FormatAuthenticatorKey(string key)
    {
        StringBuilder result = new();
        int currentPosition = 0;

        while (currentPosition + 4 < key.Length)
        {
            result.Append(key.AsSpan(currentPosition, 4)).Append(' ');
            currentPosition += 4;
        }

        if (currentPosition < key.Length)
            result.Append(key.AsSpan(currentPosition));

        return result.ToString().ToLowerInvariant();
    }

    private static string GenerateQrCodeUri(string email, string unformattedKey)
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6",
            UrlEncoder.Default.Encode("CleanDDDArchitecture"),
            UrlEncoder.Default.Encode(email),
            UrlEncoder.Default.Encode(unformattedKey));
    }

    private async Task<bool> IsAdministratorAsync(Guid actorUserId)
    {
        var actor = await _userManager.FindByIdAsync(actorUserId.ToString()).ConfigureAwait(false);

        if (actor is null)
            return false;

        var roles = await _userManager.GetRolesAsync(actor).ConfigureAwait(false);

        return roles.Any(role => AdminRoles.Contains(role, StringComparer.OrdinalIgnoreCase));
    }

    private async Task<IdentityResult> ApplyStatusAsync(
        AccountUser user,
        AccountStatus status,
        string reason,
        bool shouldLockOut,
        Guid? actorUserId,
        CancellationToken cancellationToken)
    {
        user.Status = status;
        user.StatusReason = reason;
        user.StatusChangedAtUtc = DateTimeOffset.UtcNow;

        var updateResult = await _userManager.UpdateAsync(user).ConfigureAwait(false);
        if (!updateResult.Succeeded)
            return updateResult.ToApplicationResult();

        if (_userManager.SupportsUserLockout)
        {
            await _userManager.SetLockoutEndDateAsync(
                    user,
                    shouldLockOut ? DateTimeOffset.MaxValue : null)
                .ConfigureAwait(false);

            if (!shouldLockOut)
                await _userManager.ResetAccessFailedCountAsync(user).ConfigureAwait(false);
        }

        await _userManager.UpdateSecurityStampAsync(user).ConfigureAwait(false);
        await _refreshSessionManager.RevokeAllRefreshTokensAsync(user.Id, cancellationToken).ConfigureAwait(false);
        var eventType = status switch
        {
            AccountStatus.Deactivated => AccountSecurityEventTypes.AccountDeactivated,
            AccountStatus.Suspended => AccountSecurityEventTypes.AccountSuspended,
            AccountStatus.Active => AccountSecurityEventTypes.AccountUnsuspended,
            _ => nameof(AccountStatus)
        };
        await RecordSecurityEventAsync(user.Id, actorUserId, eventType, reason, cancellationToken).ConfigureAwait(false);

        return IdentityResult.Success();
    }

    private async Task<int> RevokeAllAndRecordAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var revokedCount = await _refreshSessionManager.RevokeAllRefreshTokensAsync(userId, cancellationToken).ConfigureAwait(false);
        if (revokedCount > 0)
        {
            await RecordSecurityEventAsync(
                    userId,
                    userId,
                    AccountSecurityEventTypes.SessionsRevoked,
                    $"All active sessions were revoked ({revokedCount} sessions).",
                    cancellationToken)
                .ConfigureAwait(false);
        }

        return revokedCount;
    }

    private async Task RecordSecurityEventAsync(
        Guid userId,
        Guid? actorUserId,
        string eventType,
        string description,
        CancellationToken cancellationToken)
    {
        _accountDbContextWrite.SecurityEvents.Add(
            new Persistence.Entities.AccountSecurityEvent
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ActorUserId = actorUserId,
                Type = eventType,
                Description = description,
                OccurredAtUtc = DateTimeOffset.UtcNow,
                IpAddress = GetRemoteIp(),
                UserAgent = GetUserAgent()
            });

        await _accountDbContextWrite.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private string? GetRemoteIp() => HttpContext.Connection.RemoteIpAddress?.ToString();

    private string? GetUserAgent() => HttpContext.Request.Headers.UserAgent.ToString();

    private HttpContext HttpContext =>
        _httpContextAccessor.HttpContext
        ?? throw new InvalidOperationException("No active HttpContext is available.");
}
