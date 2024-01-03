// ReSharper disable MemberCanBeInternal

using Aviant.Core.Entities;
using Aviant.Core.EventSourcing.Aggregates;
using Aviant.Core.EventSourcing.DomainEvents;
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
        string              password,
        string              firstName,
        string              lastName,
        string              email,
        IEnumerable<string> roles,
        bool                emailConfirmed)
        : base(aggregateId)
    {
        UserName       = userName;
        Password       = password;
        FirstName      = firstName;
        LastName       = lastName;
        Email          = email;
        Roles          = roles;
        EmailConfirmed = emailConfirmed;

        AddEvent(new AccountCreatedDomainEvent(this));
    }

    public string UserName { get; private set; }

    public string Password { get; private set; }

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
        string              username,
        string              password,
        string              firstname,
        string              lastname,
        string              email,
        IEnumerable<string> roles,
        bool                emailConfirmed)
    {
        AccountAggregateId id = new(Guid.NewGuid());

        return new AccountAggregate(
            id,
            username,
            password,
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

    protected override void Apply(IDomainEvent<AccountAggregateId> @event)
    {
        switch (@event)
        {
            case AccountCreatedDomainEvent c:
                Id             = c.AggregateId;
                UserName       = c.UserName;
                Password       = c.Password;
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
        }
    }
}
