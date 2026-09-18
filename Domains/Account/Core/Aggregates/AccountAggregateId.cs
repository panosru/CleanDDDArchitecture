using Aviant.Core.EventSourcing.Aggregates;
using Newtonsoft.Json;

namespace CleanDDDArchitecture.Domains.Account.Core.Aggregates;

public sealed class AccountAggregateId : AggregateId<Guid>
{
    [JsonConstructor]
    public AccountAggregateId(Guid key)
        : base(key)
    { }
}
