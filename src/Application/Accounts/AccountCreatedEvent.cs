namespace CleanDDDArchitecture.Application.Accounts
{
    using Aviant.DDD.Domain.Events;

    public class AccountCreatedEvent : Event<AccountEntity, AccountId>
    {
        private AccountCreatedEvent()
        { }

        public AccountCreatedEvent(AccountEntity accountEntity)
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