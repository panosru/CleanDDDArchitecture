namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create.Events
{
    #region

    using Aggregates;
    using Aviant.DDD.Core.Events;

    #endregion

    public class AccountCreatedEvent : Event<AccountAggregate, AccountAggregateId>
    {
        private AccountCreatedEvent()
        { }

        public AccountCreatedEvent(AccountAggregate accountAggregate)
            : base(accountAggregate)
        {
            UserName  = accountAggregate.UserName;
            Password  = accountAggregate.Password;
            FirstName = accountAggregate.FirstName;
            LastName  = accountAggregate.LastName;
            Email     = accountAggregate.Email;
        }

        public string UserName { get; private set; }

        public string Password { get; private set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public string Email { get; private set; }
    }
}