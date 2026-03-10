namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ResetPassword;

public sealed record ResetPasswordDto(string Email, string Token, string Password);
