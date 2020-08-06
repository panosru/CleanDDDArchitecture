namespace CleanDDDArchitecture.Domain.Entities
{
    using System;
    using Aviant.DDD.Domain.Entity;
    using Aviant.DDD.Domain.Enum;
    using Events;

    public class TodoItem : Base<int>, ICreationAudited, IModificationAudited, IDeletionAudited, ISoftDelete
    {
        public int ListId { get; set; }

        public string Title { get; set; }

        public string Note { get; set; }

        public bool Done { get; set; }

        public DateTime? Reminder { get; set; }

        public TodoList List { get; set; }
        
        public DateTime Created { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? LastModified { get; set; }
        public Guid? LastModifiedBy { get; set; }
        public DateTime? Deleted { get; set; }
        public Guid? DeletedBy { get; set; }
        public bool IsDeleted { get; set; }
        
        public bool IsCompleted { get;  private set; }

        public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;

        public State State { get; set; } = State.Active;
        
        public void MarkComplete()
        {
            IsCompleted = true;
            Events.Add(new TodoCompleted(this));
        }

        public void MarkUnComplete()
        {
            IsCompleted = false;
        }
    }
}