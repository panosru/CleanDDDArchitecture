namespace CleanDDDArchitecture.Domains.Account.Application.Aggregates
{
    using System;
    using Aviant.DDD.Core.Aggregates;
    using Newtonsoft.Json;

    public sealed class AccountAggregateId : AggregateId<Guid>
    {
        [JsonConstructor]
        public AccountAggregateId(Guid key)
            : base(key)
        { }
    }
}