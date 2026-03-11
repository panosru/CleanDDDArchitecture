using Aviant.Application.Identity;
using CleanDDDArchitecture.Domains.Account.Core.Identity.Dto;

namespace CleanDDDArchitecture.Domains.Account.Application.Identity;

public sealed record AccountDeletionRequestResult(
    IdentityResult Result,
    AccountDeletionTicket? Ticket);
