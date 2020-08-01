using System;

namespace Aviant.DDD.Domain.Entity
{
    public interface IAudited
    {
    }

    public interface IHasCreationTime : IAudited
    {
        public DateTime Created { get; set; }
    }

    public interface ICreationAudited : IHasCreationTime
    {
        public string CreatedBy { get; set; }
    }


    public interface IHasModificationTime : IAudited
    {
        public DateTime? LastModified { get; set; }
    }

    public interface IModificationAudited : IHasModificationTime
    {
        public string? LastModifiedBy { get; set; }
    }


    public interface IHasDeletionTime : IAudited
    {
        DateTime? Deleted { get; set; }
    }

    public interface IDeletionAudited : IHasDeletionTime
    {
        string? DeletedBy { get; set; }
    }
}