namespace CleanDDDArchitecture.Domain.ValueObjects
{
    using System;
    using Aviant.DDD.Domain.ValueObjects;
    using Exceptions;

    public class AdAccountValueObject : ValueObject //TODO: Utilise this VO for Account Creation 
    {
        private AdAccountValueObject()
        {
        }

        public string Domain { get; private set; }

        public string Name { get; private set; }

        public static AdAccountValueObject For(string accountString)
        {
            var account = new AdAccountValueObject();

            try
            {
                var index = accountString.IndexOf("\\", StringComparison.Ordinal);
                account.Domain = accountString.Substring(0, index);
                account.Name = accountString.Substring(index + 1);
            }
            catch (Exception ex)
            {
                throw new AdAccountInvalidException(accountString, ex);
            }

            return account;
        }

        public static implicit operator string(AdAccountValueObject accountValueObject)
        {
            return accountValueObject.ToString();
        }

        public static explicit operator AdAccountValueObject(string accountString)
        {
            return For(accountString);
        }

        public override string ToString()
        {
            return $"{Domain}\\{Name}";
        }
    }
}