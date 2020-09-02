namespace CleanDDDArchitecture.Application.Accounts
{
    using Aviant.DDD.Domain.Aggregates;
    using Aviant.DDD.Domain.Events;

    public class AccountUpdatedEvent : Event<AccountEntity, AccountId>
    {
        public string FirstName { get; }
        
        public string LastName { get; }
        
        public string Email { get; }
        
        private AccountUpdatedEvent()
        {}

        public AccountUpdatedEvent(AccountEntity accountEntity)
            : base(accountEntity)
        {
            FirstName = accountEntity.FirstName;
            LastName = accountEntity.LastName;
            Email = accountEntity.Email;
        }
    }
}