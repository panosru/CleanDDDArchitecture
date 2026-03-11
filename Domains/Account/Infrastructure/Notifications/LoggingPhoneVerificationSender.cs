using CleanDDDArchitecture.Domains.Account.Application.Notifications;
using Serilog;

namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Notifications;

public sealed class LoggingPhoneVerificationSender : IPhoneVerificationSender
{
    public Task SendVerificationCodeAsync(
        string phoneNumber,
        string code,
        bool isChangeRequest,
        CancellationToken cancellationToken = default)
    {
        Log.Warning(
            "Demo phone verification code issued. PhoneNumber={PhoneNumber} Purpose={Purpose} Code={Code}",
            phoneNumber,
            isChangeRequest ? "change" : "add",
            code);

        return Task.CompletedTask;
    }
}
