namespace CleanDDDArchitecture.Domains.Account.Infrastructure
{
    #region

    using System;
    using System.Text;
    using Application.Aggregates;
    using Confluent.Kafka;

    #endregion

    public class AccountIdDeserializer : IDeserializer<AccountAggregateId>
    {
        #region IDeserializer<AccountAggregateId> Members

        public AccountAggregateId Deserialize(
            ReadOnlySpan<byte>   data,
            bool                 isNull,
            SerializationContext context)
        {
            var decodedData = Encoding.UTF8.GetString(data.ToArray());

            return new AccountAggregateId(int.Parse(decodedData));
        }

        #endregion
    }
}