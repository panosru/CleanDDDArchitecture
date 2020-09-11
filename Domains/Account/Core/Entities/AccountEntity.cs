namespace CleanDDDArchitecture.Domains.Account.Core.Entities
{
    #region

    using System;
    using Aviant.DDD.Domain.Entities;

    #endregion

    public class AccountEntity
        : Entity<int>,
          IActivationAudited
    {
        public Guid UserId { get; }

        public string FirstName { get; protected set; }

        public string LastName { get; protected set; }

        public string Email { get; protected set; }

        #region IActivationAudited Members

        public bool IsActive { get; set; } = true;

        public Guid? ActivationStatusModifiedBy { get; set; }

        #endregion
    }
}