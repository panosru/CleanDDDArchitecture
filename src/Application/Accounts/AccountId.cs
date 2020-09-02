namespace CleanDDDArchitecture.Application.Accounts
{
    using Aviant.DDD.Domain.Aggregates;
    using Newtonsoft.Json;

    public class AccountId : AggregateId<int>
    {
        [JsonConstructor]
        public AccountId(int key)
            : base(key)
        {}
    }
}