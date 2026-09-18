using CleanDDDArchitecture.Domains.Account.Application.Notifications;
using Microsoft.Extensions.Logging;

namespace CleanDDDArchitecture.Domains.Account.Infrastructure.Notifications;

public sealed class LoggingPhoneVerificationSender(ILogger<LoggingPhoneVerificationSender> logger) : IPhoneVerificationSender
{
    public Task SendVerificationCodeAsync(
        string phoneNumber,
        string code,
        bool isChangeRequest,
        CancellationToken cancellationToken = default)
    {
        logger.LogWarning(
            "Demo phone verification code issued. PhoneNumber={PhoneNumber} Purpose={Purpose} Code={Code}",
            phoneNumber,
            isChangeRequest ? "change" : "add",
            code);

        return Task.CompletedTask;
    }
}
