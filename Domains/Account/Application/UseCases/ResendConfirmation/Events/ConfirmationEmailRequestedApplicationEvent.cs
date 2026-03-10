using Aviant.Application.ApplicationEvents;
using Aviant.Application.Jobs;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ResendConfirmation.Jobs;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ResendConfirmation.Events;

internal sealed record ConfirmationEmailRequestedApplicationEvent(
    string Email,
    string FullName,
    string Token) : ApplicationEvent;

internal sealed class ConfirmationEmailRequestedApplicationEventHandler
    : ApplicationEventHandler<ConfirmationEmailRequestedApplicationEvent>
{
    private readonly IJobRunner _jobRunner;

    public ConfirmationEmailRequestedApplicationEventHandler(IJobRunner jobRunner) => _jobRunner = jobRunner;

    public override Task Handle(
        ConfirmationEmailRequestedApplicationEvent @event,
        CancellationToken cancellationToken)
    {
        _jobRunner.Run<SendConfirmationEmailJob, SendConfirmationEmailJobOptions>(
            options =>
            {
                options.Email = @event.Email;
                options.FullName = @event.FullName;
                options.Token = @event.Token;
            });

        return Task.CompletedTask;
    }
}
