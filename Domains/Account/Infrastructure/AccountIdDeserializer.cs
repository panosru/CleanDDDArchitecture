using System.Text;
using CleanDDDArchitecture.Domains.Account.Application.Aggregates;
using Confluent.Kafka;

namespace CleanDDDArchitecture.Domains.Account.Infrastructure;

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
