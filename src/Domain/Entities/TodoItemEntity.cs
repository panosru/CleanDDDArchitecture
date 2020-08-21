namespace CleanDDDArchitecture.Domain.Entities
{
    using System;
    using Aviant.DDD.Domain.Entities;
    using Aviant.DDD.Domain.Enums;

    public sealed class TodoItemEntity
        : Entity<int>,
            ICreationAudited,
            IModificationAudited,
            IDeletionAudited,
            ISoftDelete
    {
        public int ListId { get; set; }

        public string Title { get; set; }

        public string Note { get; set; }

        public DateTime? Reminder { get; set; }

        public bool IsCompleted { get; set; } = false;

        public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;

        public State State { get; set; } = State.Active;

        #region .:: Navigation Properties ::.

        public TodoListEntity List { get; set; }

        #endregion

        public DateTime Created { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime? Deleted { get; set; }

        public Guid? DeletedBy { get; set; }

        public DateTime? LastModified { get; set; }

        public Guid? LastModifiedBy { get; set; }

        public bool IsDeleted { get; set; }
    }
}