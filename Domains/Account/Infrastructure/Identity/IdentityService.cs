using CleanDDDArchitecture.Domains.Account.Infrastructure.Identity.Mechanism;
using Aviant.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IdentityResult = Aviant.Application.Identity.IdentityResult;

namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Identity;

public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<AccountUser> _userManager;

    private readonly Authenticator _authenticator;
    
    private readonly ConfirmEmail _confirmEmail;

    public IdentityService(
        UserManager<AccountUser>    userManager,
        IAccountDomainConfiguration config)
    {
        _userManager = userManager;
        _authenticator = new Authenticator(userManager, config);
        _confirmEmail = new ConfirmEmail(userManager);
    }

    #region IIdentityService Members

    public async Task<object?> AuthenticateAsync(
        string            username,
        string            password,
        CancellationToken cancellationToken = default) =>
        await _authenticator.AuthenticateAsync(username, password, cancellationToken)
           .ConfigureAwait(false);

    public async Task<IdentityResult> ConfirmEmailAsync(
        string            token,
        string            email,
        CancellationToken cancellationToken = default) =>
        await _confirmEmail.ConfirmEmailAsync(token, email)
           .ConfigureAwait(false);

    public async Task<string> GetUserNameAsync(
        Guid              userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users.FirstAsync(u => u.Id == userId, cancellationToken)
           .ConfigureAwait(false);

        return user.UserName;
    }

    public async Task<(IdentityResult Result, Guid UserId)> CreateUserAsync(
        string            username,
        string            password,
        CancellationToken cancellationToken = default)
    {
        AccountUser user = new()
        {
            UserName = username,
            Email    = username
        };

        var result = await _userManager.CreateAsync(user, password)
           .ConfigureAwait(false);

        return (result.ToApplicationResult(), user.Id);
    }

    public async Task<IdentityResult> DeleteUserAsync(
        Guid              userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString()).ConfigureAwait(false);

        // Check if the user exists
        if (user is null)
        {
            // Optionally, handle the scenario when the user is not found.
            // For example, return a failure result or log the incident.
            return IdentityResult.Failure(new[] { "User not found." });
        }
            
        // Proceed with deletion
        return await DeleteUserAsync(user, cancellationToken).ConfigureAwait(false);
    }

    #endregion

    public async Task<IdentityResult> DeleteUserAsync(
        AccountUser       user,
        CancellationToken cancellationToken = default)
    {
        var result = await _userManager.DeleteAsync(user)
           .ConfigureAwait(false);

        return result.ToApplicationResult();
    }
}

