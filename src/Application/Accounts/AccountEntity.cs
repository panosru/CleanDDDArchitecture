namespace CleanDDDArchitecture.Application.Accounts
{
    using System;
    using Aviant.DDD.Domain.Aggregates;
    using Aviant.DDD.Domain.Entities;
    using Aviant.DDD.Domain.Events;

    public class AccountEntity
        : AggregateRoot<AccountEntity, AccountId>,
            IActivationAudited
    {
        public Guid UserId { get; private set; }

        public bool IsActive { get; set; } = true;

        public Guid? ActivationStatusModifiedBy { get; set; }
        
        public string FirstName { get; private set; }
        
        public string LastName { get; private set; }
        
        public string Email { get; private set; }
        
        private AccountEntity()
        {}

        private AccountEntity(
            AccountId id,
            string firstName,
            string lastName,
            string email)
            : base(id)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            
            AddEvent(new AccountCreatedEvent(this));
        }

        private AccountEntity(
            AccountId id,
            Guid userId,
            string email)
            : base(id)
        {
            UserId = userId;
            Email = email;
        }

        public static AccountEntity Create(string firstname, string lastname, string email)
        {
            int unixTimestamp = (int)(DateTime.UtcNow
                .Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            
            var id = new AccountId(unixTimestamp);
            
            return new AccountEntity(id, firstname, lastname, email);
        }

        public static AccountEntity Create(Guid userId, string email)
        {
            int unixTimestamp = (int)(DateTime.UtcNow
                .Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            
            var id = new AccountId(unixTimestamp);
            
            return new AccountEntity(id, userId, email);
        }

        public void ChangeDetails(string firstname, string lastname, string email)
        {
            FirstName = firstname;
            LastName = lastname;
            Email = email;
            
            AddEvent(new AccountUpdatedEvent(this));
        }

        protected override void Apply(IEvent<AccountId> @event)
        {
            switch (@event)
            {
                case AccountCreatedEvent c:
                    Id = c.AggregateId;
                    FirstName = c.FirstName;
                    LastName = c.LastName;
                    Email = c.Email;
                    break;
                
                case AccountUpdatedEvent u:
                    FirstName = u.FirstName;
                    LastName = u.LastName;
                    Email = u.Email;
                    break;
            }
        }
    }
}