// ReSharper disable MemberCanBeInternal

namespace CleanDDDArchitecture.Domains.Account.Application.Aggregates
{
    using System;
    using Aviant.DDD.Core.Aggregates;
    using Aviant.DDD.Core.Entities;
    using Aviant.DDD.Core.Events;
    using UseCases.Create.Events;
    using UseCases.UpdateDetails.Events;

    public sealed class AccountAggregate
        : Aggregate<AccountAggregate, AccountAggregateId>,
          IActivationAudited
    {
        // ReSharper disable once UnusedMember.Local
        #pragma warning disable 8618
        private AccountAggregate()
        { }
        #pragma warning restore 8618

        private AccountAggregate(
            AccountAggregateId aggregateId,
            string             userName,
            string             password,
            string             firstName,
            string             lastName,
            string             email)
            : base(aggregateId)
        {
            UserName  = userName;
            Password  = password;
            FirstName = firstName;
            LastName  = lastName;
            Email     = email;

            AddEvent(new AccountCreatedEvent(this));
        }

        // private AccountAggregate(
        //     AccountAggregateId aggregateId,
        //     Guid               userId,
        //     string             email)
        //     : base(aggregateId)
        // {
        //     UserId = userId;
        //     Email  = email;
        // }

        public string UserName { get; private set; }

        public string Password { get; private set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public string Email { get; private set; }

        #region IActivationAudited Members

        public bool IsActive { get; set; } = true;

        public Guid? ActivationStatusModifiedBy { get; set; }

        #endregion

        internal static AccountAggregate Create(
            string username,
            string password,
            string firstname,
            string lastname,
            string email)
        {
            var id = new AccountAggregateId(Guid.NewGuid());

            return new AccountAggregate(
                id,
                username,
                password,
                firstname,
                lastname,
                email);
        }

        // public static AccountAggregate Create(Guid userId, string email)
        // {
        //     var unixTimestamp = (int) DateTime.UtcNow
        //        .Subtract(new DateTime(1970, 1, 1))
        //        .TotalSeconds;
        //
        //     var id = new AccountAggregateId(unixTimestamp);
        //
        //     return new AccountAggregate(id, userId, email);
        // }

        internal void ChangeDetails(
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
                    UserName  = c.UserName;
                    Password  = c.Password;
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