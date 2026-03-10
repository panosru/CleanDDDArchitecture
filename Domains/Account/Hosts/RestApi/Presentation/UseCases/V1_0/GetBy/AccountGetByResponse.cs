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
        Username  = accountUser.UserName ?? string.Empty;
        FirstName = accountUser.FirstName ?? string.Empty;
        LastName  = accountUser.LastName ?? string.Empty;
        Email     = accountUser.Email ?? string.Empty;
        Status    = accountUser.Status.ToString();
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

    /// <summary>
    /// </summary>
    public string Status { get; }
}
