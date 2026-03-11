namespace CleanDDDArchitecture.Domains.Account.Application.Notifications;

public interface IPhoneVerificationSender
{
    Task SendVerificationCodeAsync(
        string phoneNumber,
        string code,
        bool isChangeRequest,
        CancellationToken cancellationToken = default);
}
