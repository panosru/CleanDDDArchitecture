namespace CleanDDDArchitecture.Domains.Account.Application.Identity
{
    using System.Collections.Specialized;

    public interface ITokenService
    {
        string BuildToken(
            NameValueCollection keys,
            string              subject,
            string              issuer,
            string              audience,
            AccountUser         user);

        bool ValidateToken(
            NameValueCollection keys,
            string              issuer,
            string              audience,
            string              token);
    }
}