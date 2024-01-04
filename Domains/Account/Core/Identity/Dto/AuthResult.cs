namespace CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

/// <summary>
/// Represents the result of an authentication operation.
/// </summary>
public class AuthResult
{
    /// <summary>
    /// Gets or sets the token type.
    /// Typically "Bearer" in OAuth2.
    /// </summary>
    public string TokenType { get; set; }

    /// <summary>
    /// Gets or sets the serialized access token.
    /// This token is used for authenticating subsequent requests.
    /// </summary>
    public string AccessToken { get; set; }

    /// <summary>
    /// Gets or sets the number of seconds until the access token expires.
    /// This is typically used by clients to know when to refresh the token.
    /// </summary>
    public long ExpiresIn { get; set; }

    /// <summary>
    /// Gets or sets the serialized refresh token.
    /// This token is used to obtain a new access token once the current one expires.
    /// </summary>
    public string RefreshToken { get; set; }
}
