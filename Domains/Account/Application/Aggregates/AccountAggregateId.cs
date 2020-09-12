namespace CleanDDDArchitecture.Domains.Account.Application.Aggregates
{
    #region

    using System;
    using Aviant.DDD.Domain.Aggregates;
    using Newtonsoft.Json;

    #endregion

    public class AccountAggregateId : AggregateId<Guid>
    {
        [JsonConstructor]
        public AccountAggregateId(Guid key)
            : base(key)
        { }
    }
}