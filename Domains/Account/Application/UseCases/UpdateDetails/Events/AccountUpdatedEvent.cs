namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.UpdateDetails.Events
{
    using Aggregates;
    using Aviant.DDD.Core.Events;

    public class AccountUpdatedEvent : Event<AccountAggregate, AccountAggregateId>
    {
        private AccountUpdatedEvent()
        { }

        public AccountUpdatedEvent(AccountAggregate accountAggregate)
            : base(accountAggregate)
        {
            FirstName = accountAggregate.FirstName;
            LastName  = accountAggregate.LastName;
            Email     = accountAggregate.Email;
        }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public string Email { get; private set; }
    }
}