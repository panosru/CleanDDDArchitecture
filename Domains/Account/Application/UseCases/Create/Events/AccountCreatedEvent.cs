namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create.Events
{
    using Aggregates;
    using Aviant.DDD.Domain.Events;

    public class AccountCreatedEvent : Event<AccountAggregate, AccountAggregateId>
    {
        private AccountCreatedEvent()
        { }

        public AccountCreatedEvent(AccountAggregate accountAggregate)
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