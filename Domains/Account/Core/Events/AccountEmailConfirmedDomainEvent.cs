using Aviant.Core.EventSourcing.DomainEvents;
using CleanDDDArchitecture.Domains.Account.Core.Aggregates;

namespace CleanDDDArchitecture.Domains.Account.Core.Events;

public sealed record AccountEmailConfirmedDomainEvent : DomainEvent<AccountAggregate, AccountAggregateId>
{
    #pragma warning disable 8618
    private AccountEmailConfirmedDomainEvent()
    { }
    #pragma warning restore 8618

    public AccountEmailConfirmedDomainEvent(AccountAggregate accountAggregate)
        : base(accountAggregate)
    {
        Email = accountAggregate.Email;
        EmailConfirmed = accountAggregate.EmailConfirmed;
    }

    public string Email { get; private set; }

    public bool EmailConfirmed { get; private set; }
}
