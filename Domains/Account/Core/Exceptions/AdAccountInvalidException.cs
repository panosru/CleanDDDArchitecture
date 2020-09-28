namespace CleanDDDArchitecture.Domains.Account.Core.Exceptions
{
    using System;

    internal sealed class AdAccountInvalidException : Exception
    {
        public AdAccountInvalidException(string adAccount, Exception ex)
            : base($"AD Account \"{adAccount}\" is invalid.", ex)
        { }
    }
}