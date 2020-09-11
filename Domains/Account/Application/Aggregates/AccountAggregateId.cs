namespace CleanDDDArchitecture.Domains.Account.Application.Aggregates
{
    #region

    using Aviant.DDD.Domain.Aggregates;
    using Newtonsoft.Json;

    #endregion

    public class AccountAggregateId : AggregateId<int>
    {
        [JsonConstructor]
        public AccountAggregateId(int key)
            : base(key)
        { }
    }
}