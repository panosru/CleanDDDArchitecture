namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.PhoneVerification;

public sealed record VerifyPhoneVerificationDto(string PhoneNumber, string Code);
