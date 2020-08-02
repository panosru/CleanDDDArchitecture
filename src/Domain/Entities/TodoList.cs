namespace CleanArchitecture.Domain.Entities
{
    using System.Collections.Generic;
    using Aviant.DDD.Domain.Entity;

    public class TodoList : Auditable
    {
        public TodoList()
        {
            Items = new List<TodoItem>();
        }

        public int Id { get; set; }

        public string Title { get; set; }

        public string Colour { get; set; }

        public IList<TodoItem> Items { get; private set; }
    }
}
