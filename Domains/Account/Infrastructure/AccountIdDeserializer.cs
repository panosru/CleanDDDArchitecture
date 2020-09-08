namespace CleanDDDArchitecture.Domains.Account.Infrastructure
{
    using System;
    using System.Text;
    using Application.Aggregates;
    using Confluent.Kafka;

    public class AccountIdDeserializer : IDeserializer<AccountId>
    {
    #region IDeserializer<AccountId> Members

        public AccountId Deserialize(
            ReadOnlySpan<byte>   data,
            bool                 isNull,
            SerializationContext context)
        {
            var decodedData = Encoding.UTF8.GetString(data.ToArray());

            return new AccountId(int.Parse(decodedData));
        }

    #endregion
    }
}