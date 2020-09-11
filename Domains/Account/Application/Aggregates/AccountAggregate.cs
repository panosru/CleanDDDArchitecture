namespace CleanDDDArchitecture.Domains.Account.Application.Aggregates
{
    using System;
    using Aviant.DDD.Domain.Aggregates;
    using Aviant.DDD.Domain.Entities;
    using Aviant.DDD.Domain.Events;
    using UseCases.Create.Events;
    using UseCases.UpdateDetails.Events;

    public class AccountAggregate
        : Aggregate<AccountAggregate, AccountAggregateId>,
          IActivationAudited
    {
        private AccountAggregate()
        { }

        private AccountAggregate(
            AccountAggregateId aggregateId,
            string    firstName,
            string    lastName,
            string    email)
            : base(aggregateId)
        {
            FirstName = firstName;
            LastName  = lastName;
            Email     = email;

            AddEvent(new AccountCreatedEvent(this));
        }

        private AccountAggregate(
            AccountAggregateId aggregateId,
            Guid      userId,
            string    email)
            : base(aggregateId)
        {
            UserId = userId;
            Email  = email;
        }

        public Guid UserId { get; private set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public string Email { get; private set; }

    #region IActivationAudited Members

        public bool IsActive { get; set; } = true;

        public Guid? ActivationStatusModifiedBy { get; set; }

    #endregion

        public static AccountAggregate Create(
            string firstname,
            string lastname,
            string email)
        {
            var unixTimestamp = (int) DateTime.UtcNow
                                              .Subtract(new DateTime(1970, 1, 1))
                                              .TotalSeconds;

            var id = new AccountAggregateId(unixTimestamp);

            return new AccountAggregate(
                id,
                firstname,
                lastname,
                email);
        }

        public static AccountAggregate Create(Guid userId, string email)
        {
            var unixTimestamp = (int) DateTime.UtcNow
                                              .Subtract(new DateTime(1970, 1, 1))
                                              .TotalSeconds;

            var id = new AccountAggregateId(unixTimestamp);

            return new AccountAggregate(id, userId, email);
        }

        public void ChangeDetails(
            string firstname,
            string lastname,
            string email)
        {
            FirstName = firstname;
            LastName  = lastname;
            Email     = email;

            AddEvent(new AccountUpdatedEvent(this));
        }

        protected override void Apply(IEvent<AccountAggregateId> @event)
        {
            switch (@event)
            {
                case AccountCreatedEvent c:
                    Id        = c.AggregateId;
                    FirstName = c.FirstName;
                    LastName  = c.LastName;
                    Email     = c.Email;

                    break;

                case AccountUpdatedEvent u:
                    FirstName = u.FirstName;
                    LastName  = u.LastName;
                    Email     = u.Email;

                    break;
            }
        }
    }
}