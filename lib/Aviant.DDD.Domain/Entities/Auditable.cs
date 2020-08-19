namespace Aviant.DDD.Domain.Entities
{
    using System;

    public interface IAuditedEntity
    {
    }

    public interface IHasCreationTime : IAuditedEntity
    {
        public DateTime Created { get; set; }
    }

    public interface ICreationAudited : IHasCreationTime
    {
        public Guid CreatedBy { get; set; }
    }

    public interface IHasModificationTime : IAuditedEntity
    {
        public DateTime? LastModified { get; set; }
    }

    public interface IModificationAudited : IHasModificationTime
    {
        public Guid? LastModifiedBy { get; set; }
    }

    public interface IHasDeletionTime : IAuditedEntity
    {
        DateTime? Deleted { get; set; }
    }

    public interface IDeletionAudited : IHasDeletionTime
    {
        Guid? DeletedBy { get; set; }
    }

    public interface IHasActivationStatus : IAuditedEntity
    {
        public bool IsActive { get; set; }
    }

    public interface IActivationAudited : IHasActivationStatus
    {
        public Guid? ActivationStatusModifiedBy { get; set; }
    }
}