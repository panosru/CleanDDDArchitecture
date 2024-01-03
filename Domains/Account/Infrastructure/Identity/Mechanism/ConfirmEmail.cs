using CleanDDDArchitecture.Domains.Account.Application.Identity;
using Microsoft.AspNetCore.Identity;
using IdentityResult = Aviant.Application.Identity.IdentityResult;

namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Identity.Mechanism;

/// <summary>
/// Confirm user's email
/// </summary>
internal class ConfirmEmail
{
    private readonly UserManager<AccountUser> _userManager;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="userManager"></param>
    internal ConfirmEmail(
        UserManager<AccountUser>    userManager)
    {
        _userManager = userManager;
    }
    
    /// <summary>
    /// Confirm user's email
    /// </summary>
    /// <param name="token"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    internal async Task<IdentityResult> ConfirmEmailAsync(
        string            token,
        string            email)
    {
        // Find user by email
        var user = await FindUserByEmailAsync(email);
        if (user is null)
            return IdentityResult.Failure(new[] { "Invalid email." });

        // Check if email is already confirmed
        if (user.EmailConfirmed)
            return IdentityResult.Failure(new[] { "Email already confirmed." });

        // Attempt to confirm email
        return await ConfirmUserEmailAsync(user, token);
    }
    
    /// <summary>
    /// Find a user by their email
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    private async Task<AccountUser?> FindUserByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email).ConfigureAwait(false);
    }

    /// <summary>
    /// Confirm the user's email
    /// </summary>
    /// <param name="user"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    private async Task<IdentityResult> ConfirmUserEmailAsync(AccountUser user, string token)
    {
        var result = await _userManager.ConfirmEmailAsync(user, token).ConfigureAwait(false);

        return !result.Succeeded 
            ? IdentityResult.Failure(new[] { result.Errors.First().Description }) 
            : IdentityResult.Success();
    }
}
