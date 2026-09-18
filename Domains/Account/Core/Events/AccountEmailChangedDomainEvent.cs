// ReSharper disable MemberCanBeInternal

using Aviant.Core.EventSourcing.DomainEvents;
using CleanDDDArchitecture.Domains.Account.Core.Aggregates;

namespace CleanDDDArchitecture.Domains.Account.Core.Events;

public sealed record AccountEmailChangedDomainEvent : DomainEvent<AccountAggregate, AccountAggregateId>
{
    #pragma warning disable 8618
    private AccountEmailChangedDomainEvent()
    { }
    #pragma warning restore 8618

    public AccountEmailChangedDomainEvent(AccountAggregate accountAggregate)
        : base(accountAggregate)
    {
        UserName = accountAggregate.UserName;
        Email = accountAggregate.Email;
        EmailConfirmed = accountAggregate.EmailConfirmed;
    }

    public string UserName { get; private set; }

    public string Email { get; private set; }

    public bool EmailConfirmed { get; private set; }
}
