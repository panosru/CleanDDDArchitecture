// ReSharper disable MemberCanBeInternal

using Aviant.Core.Entities;
using Aviant.Core.EventSourcing.Aggregates;
using Aviant.Core.EventSourcing.DomainEvents;
using CleanDDDArchitecture.Domains.Account.Core.Events;
using CleanDDDArchitecture.Domains.Account.Core.ValueObjects;

namespace CleanDDDArchitecture.Domains.Account.Core.Aggregates;

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

    /// <summary>
    ///     Opens an account. The email address doubles as the user name.
    /// </summary>
    public static AccountAggregate Create(
        Guid                id,
        EmailAddress        email,
        PersonName          name,
        IEnumerable<string> roles,
        bool                emailConfirmed)
    {
        ArgumentNullException.ThrowIfNull(email);
        ArgumentNullException.ThrowIfNull(name);

        return new AccountAggregate(
            new AccountAggregateId(id),
            email.Value,
            name.First,
            name.Last,
            email.Value,
            roles,
            emailConfirmed);
    }

    public void Rename(PersonName name)
    {
        ArgumentNullException.ThrowIfNull(name);

        if (FirstName == name.First && LastName == name.Last)
            return;

        FirstName = name.First;
        LastName  = name.Last;

        AddEvent(new AccountUpdatedDomainEvent(this));
    }

    public void ConfirmEmail()
    {
        if (EmailConfirmed)
            return;

        EmailConfirmed = true;

        AddEvent(new AccountEmailConfirmedDomainEvent(this));
    }

    /// <summary>
    ///     Switches to a new email address whose ownership has just been proven through the
    ///     change-email confirmation link, so the new address is confirmed.
    /// </summary>
    public void ConfirmEmailChange(EmailAddress newEmail)
    {
        ArgumentNullException.ThrowIfNull(newEmail);

        if (string.Equals(Email, newEmail.Value, StringComparison.OrdinalIgnoreCase))
            return;

        UserName       = newEmail.Value;
        Email          = newEmail.Value;
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
