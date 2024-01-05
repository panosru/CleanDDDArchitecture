// ReSharper disable UnusedAutoPropertyAccessor.Global

using CleanDDDArchitecture.Domains.Account.Application.Identity;

namespace CleanDDDArchitecture.Domains.Account.Hosts.RestApi.Presentation.UseCases.V1_0.GetBy;

/// <summary>
/// </summary>
internal readonly struct AccountGetByResponse
{
    /// <summary>
    /// </summary>
    /// <param name="accountUser"></param>
    public AccountGetByResponse(AccountUser accountUser)
    {
        Username  = accountUser.UserName;
        FirstName = accountUser.FirstName;
        LastName  = accountUser.LastName;
        Email     = accountUser.Email;
    }

    /// <summary>
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// </summary>
    public string FirstName { get; }

    /// <summary>
    /// </summary>
    public string LastName { get; }

    /// <summary>
    /// </summary>
    public string Email { get; }
}