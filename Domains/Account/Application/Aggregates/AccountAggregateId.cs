namespace CleanDDDArchitecture.Domains.Account.Application.Aggregates
{
    using Aviant.DDD.Domain.Aggregates;
    using Newtonsoft.Json;

    public class AccountAggregateId : AggregateId<int>
    {
        [JsonConstructor]
        public AccountAggregateId(int key)
            : base(key)
        { }
    }
}