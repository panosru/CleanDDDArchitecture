namespace CleanDDDArchitecture.Application.Accounts
{
    using Aviant.DDD.Domain.Aggregates;
    using Aviant.DDD.Domain.Events;

    public class AccountCreatedEvent : Event<AccountEntity, AccountId>
    {
        public string FirstName { get; private set; }
        
        public string LastName { get; private set; }
        
        public string Email { get; private set; }
        
        private AccountCreatedEvent()
        {}

        public AccountCreatedEvent(AccountEntity accountEntity)
            : base(accountEntity)
        {
            FirstName = accountEntity.FirstName;
            LastName = accountEntity.LastName;
            Email = accountEntity.Email;
        }
    }
}