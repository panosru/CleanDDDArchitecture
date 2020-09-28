namespace CleanDDDArchitecture.Domains.Account.Infrastructure
{
    using System;
    using System.Text;
    using Application.Aggregates;
    using Confluent.Kafka;

    internal sealed class AccountIdDeserializer : IDeserializer<AccountAggregateId>
    {
        #region IDeserializer<AccountAggregateId> Members

        public AccountAggregateId Deserialize(
            ReadOnlySpan<byte>   data,
            bool                 isNull,
            SerializationContext context)
        {
            var decodedData = Encoding.UTF8.GetString(data.ToArray());

            return new AccountAggregateId(new Guid(decodedData));
        }

        #endregion
    }
}