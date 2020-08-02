namespace CleanArchitecture.Domain.Entities
{
    using Enums;
    using System;
    using Aviant.DDD.Domain.Entity;

    public class TodoItem : Auditable
    {
        public int Id { get; set; }

        public int ListId { get; set; }

        public string Title { get; set; }

        public string Note { get; set; }

        public bool Done { get; set; }

        public DateTime? Reminder { get; set; }

        public PriorityLevel Priority { get; set; }


        public TodoList List { get; set; }
    }
}
