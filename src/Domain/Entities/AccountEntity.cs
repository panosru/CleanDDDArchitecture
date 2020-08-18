namespace CleanDDDArchitecture.Domain.Entities
{
    using System;
    using Aviant.DDD.Domain.Entities;

    public class AccountEntity : EntityBase<int>,
        IActivationAudited
    {
        public Guid UserId { get; set; }

        public bool IsActive { get; set; } = true;
        
        public Guid? ActivationStatusModifiedBy { get; set; }
    }
}