namespace CleanDDDArchitecture.Domains.Account.Core.ValueObjects
{
    using System;
    using Aviant.DDD.Core.ValueObjects;
    using Exceptions;

    internal sealed class AdAccountValueObject : ValueObject //TODO: Utilise this VO for Account Creation
    {
        #pragma warning disable 8618
        private AdAccountValueObject()
            #pragma warning restore 8618
        { }

        public string Domain { get; private set; }

        public string Name { get; private set; }

        public static AdAccountValueObject For(string accountString)
        {
            var account = new AdAccountValueObject();

            try
            {
                var index = accountString.IndexOf("\\", StringComparison.Ordinal);
                account.Domain = accountString.Substring(0, index);
                account.Name   = accountString.Substring(index + 1);
            }
            catch (Exception ex)
            {
                throw new AdAccountInvalidException(accountString, ex);
            }

            return account;
        }

        public static implicit operator string(AdAccountValueObject accountValueObject) =>
            accountValueObject.ToString();

        public static explicit operator AdAccountValueObject(string accountString) => For(accountString);

        public override string ToString() => $"{Domain}\\{Name}";
    }
}