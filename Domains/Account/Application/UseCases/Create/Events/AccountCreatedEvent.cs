namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create.Events
{
    using Aggregates;
    using Aviant.DDD.Core.Events;

    public sealed class AccountCreatedEvent : Event<AccountAggregate, AccountAggregateId>
    {
        // ReSharper disable once UnusedMember.Local
        #pragma warning disable 8618
        private AccountCreatedEvent()
        { }
        #pragma warning restore 8618

        public AccountCreatedEvent(AccountAggregate accountAggregate)
            : base(accountAggregate)
        {
            Id        = accountAggregate.Id;
            UserName  = accountAggregate.UserName;
            Password  = accountAggregate.Password;
            FirstName = accountAggregate.FirstName;
            LastName  = accountAggregate.LastName;
            Email     = accountAggregate.Email;
        }

        public AccountAggregateId Id { get; private set; }

        public string UserName { get; private set; }

        public string Password { get; private set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public string Email { get; private set; }
    }
}