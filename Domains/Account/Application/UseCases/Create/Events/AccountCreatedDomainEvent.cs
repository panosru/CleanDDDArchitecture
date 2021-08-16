// ReSharper disable MemberCanBeInternal

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create.Events
{
    using System.Collections.Generic;
    using Aggregates;
    using Aviant.DDD.Core.DomainEvents;

    public sealed class AccountCreatedDomainEvent : DomainEvent<AccountAggregate, AccountAggregateId>
    {
        // ReSharper disable once UnusedMember.Local
        #pragma warning disable 8618
        private AccountCreatedDomainEvent()
        { }
        #pragma warning restore 8618

        public AccountCreatedDomainEvent(AccountAggregate accountAggregate)
            : base(accountAggregate)
        {
            Id             = accountAggregate.Id;
            UserName       = accountAggregate.UserName;
            Password       = accountAggregate.Password;
            FirstName      = accountAggregate.FirstName;
            LastName       = accountAggregate.LastName;
            Email          = accountAggregate.Email;
            Roles          = accountAggregate.Roles;
            EmailConfirmed = accountAggregate.EmailConfirmed;
        }

        public AccountAggregateId Id { get; private set; }

        public string UserName { get; private set; }

        public string Password { get; private set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public string Email { get; private set; }

        public IEnumerable<string> Roles { get; private set; }

        public bool EmailConfirmed { get; private set; }
    }
}