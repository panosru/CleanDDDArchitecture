namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.UpdateDetails.Events
{
    using Aggregates;
    using Aviant.DDD.Core.Events;

    internal sealed class AccountUpdatedEvent : Event<AccountAggregate, AccountAggregateId>
    {
        // ReSharper disable once UnusedMember.Local
        #pragma warning disable 8618
        private AccountUpdatedEvent()
        { }
        #pragma warning restore 8618

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