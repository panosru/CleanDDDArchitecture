// ReSharper disable MemberCanBeInternal

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.UpdateDetails.Events;

using Aggregates;
using Aviant.DDD.Core.DomainEvents;
using Aviant.DDD.Core.EventBus;
using Polly;

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

    #region Nested type: AccountUpdatedDomainEventConsumer

    internal sealed class AccountUpdatedDomainEventConsumer : DomainEventHandler<AccountUpdatedDomainEvent>
    {
        public override Task Handle(
            EventReceived<AccountUpdatedDomainEvent> @event,
            CancellationToken                        cancellationToken) =>
            throw new NotImplementedException();

        public override IAsyncPolicy RetryPolicy() =>
            Policy
               .Handle<ArgumentOutOfRangeException>()
               .WaitAndRetryAsync(
                    3,
                    i => TimeSpan.FromSeconds(i));
    }

    #endregion
}
