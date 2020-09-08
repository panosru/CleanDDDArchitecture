namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.UpdateDetails.Events
{
    using Aggregates;
    using Aviant.DDD.Domain.Events;

    public class AccountUpdatedEvent : Event<AccountEntity, AccountId>
    {
        private AccountUpdatedEvent()
        { }

        public AccountUpdatedEvent(AccountEntity accountEntity)
            : base(accountEntity)
        {
            FirstName = accountEntity.FirstName;
            LastName  = accountEntity.LastName;
            Email     = accountEntity.Email;
        }

        public string FirstName { get; }

        public string LastName { get; }

        public string Email { get; }
    }
}