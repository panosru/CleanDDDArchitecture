using Aviant.Application.ApplicationEvents;
using Aviant.Application.Jobs;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.Create.Jobs;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.Create.Events;

internal sealed record AccountCreatedApplicationEvent(string Email, bool EmailConfirmed) : ApplicationEvent;

internal sealed class AccountCreatedApplicationEventHandler : ApplicationEventHandler<AccountCreatedApplicationEvent>
{
    private readonly IJobRunner _jobRunner;

    public AccountCreatedApplicationEventHandler(IJobRunner jobRunner) => _jobRunner = jobRunner;

    public override Task Handle(
        AccountCreatedApplicationEvent @event,
        CancellationToken              cancellationToken)
    {
        if (@event.EmailConfirmed)
            return Task.CompletedTask;

        _jobRunner.Run<SendEmailConfirmJob, SendEmailConfirmJobOptions>(
            options => options.Email = @event.Email);

        return Task.CompletedTask;
    }
}
