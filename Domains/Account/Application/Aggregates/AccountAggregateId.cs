namespace CleanDDDArchitecture.Domains.Account.Application.Aggregates;

using Aviant.EventSourcing.Core.Aggregates;
using Newtonsoft.Json;

public sealed class AccountAggregateId : AggregateId<Guid>
{
    [JsonConstructor]
    public AccountAggregateId(Guid key)
        : base(key)
    { }
}
