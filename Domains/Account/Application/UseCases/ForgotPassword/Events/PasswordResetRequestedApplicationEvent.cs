using Aviant.Application.ApplicationEvents;
using Aviant.Application.Jobs;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ForgotPassword.Jobs;

namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ForgotPassword.Events;

internal sealed record PasswordResetRequestedApplicationEvent(
    string Email,
    string FullName,
    string Token) : ApplicationEvent;

internal sealed class PasswordResetRequestedApplicationEventHandler
    : ApplicationEventHandler<PasswordResetRequestedApplicationEvent>
{
    private readonly IJobRunner _jobRunner;

    public PasswordResetRequestedApplicationEventHandler(IJobRunner jobRunner) => _jobRunner = jobRunner;

    public override Task Handle(
        PasswordResetRequestedApplicationEvent @event,
        CancellationToken cancellationToken)
    {
        _jobRunner.Run<SendPasswordResetEmailJob, SendPasswordResetEmailJobOptions>(
            options =>
            {
                options.Email = @event.Email;
                options.FullName = @event.FullName;
                options.Token = @event.Token;
            });

        return Task.CompletedTask;
    }
}
