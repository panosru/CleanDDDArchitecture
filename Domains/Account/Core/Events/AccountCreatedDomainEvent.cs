// ReSharper disable MemberCanBeInternal

using Aviant.Core.EventSourcing.DomainEvents;
using CleanDDDArchitecture.Domains.Account.Core.Aggregates;

namespace CleanDDDArchitecture.Domains.Account.Core.Events;

public sealed record AccountCreatedDomainEvent : DomainEvent<AccountAggregate, AccountAggregateId>
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
        FirstName      = accountAggregate.FirstName;
        LastName       = accountAggregate.LastName;
        Email          = accountAggregate.Email;
        Roles          = accountAggregate.Roles;
        EmailConfirmed = accountAggregate.EmailConfirmed;
    }

    public AccountAggregateId Id { get; private set; }

    public string UserName { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string Email { get; private set; }

    public IEnumerable<string> Roles { get; private set; }

    public bool EmailConfirmed { get; private set; }
}
