namespace Aviant.DDD.Domain.Entity
{
    using System;

    public abstract class Auditable : ICreationAudited, IModificationAudited, IDeletionAudited
    {
        public Guid CreatedBy { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Deleted { get; set; }

        public Guid? DeletedBy { get; set; }

        public Guid? LastModifiedBy { get; set; }

        public DateTime? LastModified { get; set; }
    }
}