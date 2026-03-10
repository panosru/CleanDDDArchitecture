using System.Text;
using System.Web;
using Aviant.Core.Timing;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Identity.Mechanism;

/// <summary>
/// Handle user authentication
/// </summary>
internal sealed class Authenticator
{
    private readonly RefreshSessionManager _refreshSessionManager;
    private readonly UserManager<AccountUser> _userManager;

    /// <summary>
    /// Constructor
    /// </summary>
    internal Authenticator(
        UserManager<AccountUser> userManager,
        RefreshSessionManager refreshSessionManager)
    {
        _userManager = userManager;
        _refreshSessionManager = refreshSessionManager;
    }

    /// <summary>
    /// Main method to handle user authentication
    /// </summary>
    internal async Task<object?> AuthenticateAsync(
        string username,
        string password,
        CancellationToken cancellationToken)
    {
        var user = await FindUserAsync(username, cancellationToken).ConfigureAwait(false);
        if (user is null)
            return null;

        if (await IsUserLockedOutAsync(user).ConfigureAwait(false))
            return new { error = "Account is locked" };

        if (!await CheckPasswordAsync(user, password).ConfigureAwait(false))
            return new { error = "Invalid credentials" };

        if (!user.EmailConfirmed)
        {
            return new
            {
                error = "Confirm your email first",
                confirm_token = await GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false)
            };
        }

        await ResetAccessFailedCountIfNeededAsync(user).ConfigureAwait(false);

        var authResult = await _refreshSessionManager
            .IssueTokensAsync(user, cancellationToken)
            .ConfigureAwait(false);

        await UpdateLastAccessedAsync(user).ConfigureAwait(false);

        return authResult;
    }

    private async Task<AccountUser?> FindUserAsync(
        string username,
        CancellationToken cancellationToken)
    {
        return await _userManager.Users.FirstOrDefaultAsync(
                user => user.UserName == username,
                cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task<bool> IsUserLockedOutAsync(AccountUser user)
    {
        return _userManager.SupportsUserLockout
            && await _userManager.IsLockedOutAsync(user).ConfigureAwait(false);
    }

    private async Task<bool> CheckPasswordAsync(AccountUser user, string password)
    {
        if (await _userManager.CheckPasswordAsync(user, password).ConfigureAwait(false))
            return true;

        if (_userManager.SupportsUserLockout
         && await _userManager.GetLockoutEnabledAsync(user).ConfigureAwait(false))
            await _userManager.AccessFailedAsync(user).ConfigureAwait(false);

        return false;
    }

    private async Task<string> GenerateEmailConfirmationTokenAsync(AccountUser user)
    {
        return HttpUtility.UrlEncode(
            Convert.ToBase64String(
                Encoding.UTF8.GetBytes(
                    await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false))));
    }

    private async Task ResetAccessFailedCountIfNeededAsync(AccountUser user)
    {
        if (_userManager.SupportsUserLockout
         && 0 < await _userManager.GetAccessFailedCountAsync(user).ConfigureAwait(false))
            await _userManager.ResetAccessFailedCountAsync(user).ConfigureAwait(false);
    }

    private async Task UpdateLastAccessedAsync(AccountUser user)
    {
        var now = Clock.Now;

        user.LastAccessed = now.Kind switch
        {
            DateTimeKind.Utc => now,
            DateTimeKind.Unspecified => DateTime.SpecifyKind(now, DateTimeKind.Utc),
            _ => now.ToUniversalTime()
        };

        await _userManager.UpdateAsync(user).ConfigureAwait(false);
    }
}
