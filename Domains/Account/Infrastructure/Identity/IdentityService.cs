using Aviant.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Core;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;
using CleanDDDArchitecture.Domains.Account.Infrastructure.Identity.Mechanism;
using CleanDDDArchitecture.Domains.Account.Infrastructure.Persistence.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using IdentityResult = Aviant.Application.Identity.IdentityResult;

namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Identity;

public sealed class IdentityService : IIdentityService, IAccountAuthenticationService
{
    private static readonly string[] UserNotFoundErrors = ["User not found."];

    private readonly Authenticator _authenticator;
    private readonly ConfirmEmail _confirmEmail;
    private readonly RefreshSessionManager _refreshSessionManager;
    private readonly RoleManager<AccountRole> _roleManager;
    private readonly UserManager<AccountUser> _userManager;

    public IdentityService(
        UserManager<AccountUser> userManager,
        RoleManager<AccountRole> roleManager,
        IAccountDomainConfiguration config,
        AccountDbContextWrite accountDbContextWrite,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _refreshSessionManager = new RefreshSessionManager(accountDbContextWrite, config, httpContextAccessor);
        _authenticator = new Authenticator(userManager, _refreshSessionManager);
        _confirmEmail = new ConfirmEmail(userManager);
    }

    public async Task<object?> AuthenticateAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default) =>
        await _authenticator.AuthenticateAsync(username, password, cancellationToken).ConfigureAwait(false);

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
}
