// ReSharper disable MemberCanBeInternal

using Aviant.Core.EventSourcing.DomainEvents;
using CleanDDDArchitecture.Domains.Account.Core.Aggregates;

namespace CleanDDDArchitecture.Domains.Account.Core.Events;

public sealed record AccountUpdatedDomainEvent : DomainEvent<AccountAggregate, AccountAggregateId>
{
    // ReSharper disable once UnusedMember.Local
    #pragma warning disable 8618
    private AccountUpdatedDomainEvent()
    { }
    #pragma warning restore 8618

    public AccountUpdatedDomainEvent(AccountAggregate accountAggregate)
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
