using System;

namespace Aviant.DDD.Domain.Entity
{
    public abstract class Auditable : ICreationAudited, IModificationAudited, IDeletionAudited
    {
        public Guid CreatedBy { get; set; }

        public DateTime Created { get; set; }

        public Guid? LastModifiedBy { get; set; }

        public DateTime? LastModified { get; set; }
        
        public DateTime? Deleted { get; set; }
        
        public Guid? DeletedBy { get; set; }
    }
}