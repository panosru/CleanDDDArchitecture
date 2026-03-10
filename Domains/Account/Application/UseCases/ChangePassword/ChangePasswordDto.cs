namespace CleanDDDArchitecture.Domains.Account.Application.UseCases.ChangePassword;

public sealed record ChangePasswordDto(string CurrentPassword, string NewPassword);
