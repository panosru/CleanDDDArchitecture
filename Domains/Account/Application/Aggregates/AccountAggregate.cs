// ReSharper disable MemberCanBeInternal

using Aviant.Core.Entities;
using Aviant.Core.EventSourcing.Aggregates;
using Aviant.Core.EventSourcing.DomainEvents;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ChangeEmail.Events;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.ConfirmEmail.Events;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.Create.Events;
using CleanDDDArchitecture.Domains.Account.Application.UseCases.UpdateDetails.Events;

namespace CleanDDDArchitecture.Domains.Account.Application.Aggregates;

public sealed class AccountAggregate
    : Aggregate<AccountAggregate, AccountAggregateId>,
      IActivationStatus
{
    // ReSharper disable once UnusedMember.Local
    #pragma warning disable 8618
    private AccountAggregate()
    { }
    #pragma warning restore 8618

    private AccountAggregate(
        AccountAggregateId  aggregateId,
        string              userName,
        string              firstName,
        string              lastName,
        string              email,
        IEnumerable<string> roles,
        bool                emailConfirmed)
        : base(aggregateId)
    {
        UserName       = userName;
        FirstName      = firstName;
        LastName       = lastName;
        Email          = email;
        Roles          = roles;
        EmailConfirmed = emailConfirmed;

        AddEvent(new AccountCreatedDomainEvent(this));
    }

    public string UserName { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string Email { get; private set; }

    public IEnumerable<string> Roles { get; private set; }

    public bool EmailConfirmed { get; private set; }

    #region IActivationAudited Members

    public bool IsActive { get; set; } = true;

    public Guid? ActivationStatusModifiedBy { get; set; }

    #endregion

    internal static AccountAggregate Create(
        Guid                id,
        string              username,
        string              firstname,
        string              lastname,
        string              email,
        IEnumerable<string> roles,
        bool                emailConfirmed)
    {
        return new AccountAggregate(
            new AccountAggregateId(id),
            username,
            firstname,
            lastname,
            email,
            roles,
            emailConfirmed);
    }

    internal void ChangeDetails(
        string firstname,
        string lastname,
        string email)
    {
        FirstName = firstname;
        LastName  = lastname;
        Email     = email;

        AddEvent(new AccountUpdatedDomainEvent(this));
    }

    internal void ConfirmEmail()
    {
        if (EmailConfirmed)
            return;

        EmailConfirmed = true;

        AddEvent(new AccountEmailConfirmedDomainEvent(this));
    }

    internal void ChangeEmail(string email)
    {
        if (string.Equals(Email, email, StringComparison.OrdinalIgnoreCase))
            return;

        UserName = email;
        Email = email;
        EmailConfirmed = true;

        AddEvent(new AccountEmailChangedDomainEvent(this));
    }

    protected override void Apply(IDomainEvent<AccountAggregateId> @event)
    {
        switch (@event)
        {
            case AccountCreatedDomainEvent c:
                Id             = c.AggregateId;
                UserName       = c.UserName;
                FirstName      = c.FirstName;
                LastName       = c.LastName;
                Email          = c.Email;
                Roles          = c.Roles;
                EmailConfirmed = c.EmailConfirmed;
                break;

            case AccountUpdatedDomainEvent u:
                FirstName = u.FirstName;
                LastName  = u.LastName;
                Email     = u.Email;
                break;

            case AccountEmailConfirmedDomainEvent c:
                EmailConfirmed = c.EmailConfirmed;
                break;

            case AccountEmailChangedDomainEvent c:
                UserName = c.UserName;
                Email = c.Email;
                EmailConfirmed = c.EmailConfirmed;
                break;
        }
    }
}
