namespace CleanDDDArchitecture.Domains.Account.Core.Exceptions
{
    #region

    using System;

    #endregion

    public class AdAccountInvalidException : Exception
    {
        public AdAccountInvalidException(string adAccount, Exception ex)
            : base($"AD Account \"{adAccount}\" is invalid.", ex)
        { }
    }
}