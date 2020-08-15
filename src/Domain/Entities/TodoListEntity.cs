namespace CleanDDDArchitecture.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using Aviant.DDD.Domain.Entities;

    public class TodoListEntity : EntityBase<int>, 
        ICreationAudited, IModificationAudited, IDeletionAudited, ISoftDelete
    {
        public string Title { get; set; }

        public string Colour { get; set; }

        public IList<TodoItemEntity> Items { get; } = new List<TodoItemEntity>();

        public DateTime Created { get; set; }
        
        public Guid CreatedBy { get; set; }
        
        public DateTime? Deleted { get; set; }
        
        public Guid? DeletedBy { get; set; }
        
        public DateTime? LastModified { get; set; }
        
        public Guid? LastModifiedBy { get; set; }
        
        public bool IsDeleted { get; set; }
    }
}