namespace CleanDDDArchitecture.Domain.Events
{
    using Aviant.DDD.Domain.Event;
    using Entities;

    public class TodoCompleted : Base
    {
        public TodoItem CompletedTodo { get; private set; }

        public TodoCompleted(TodoItem completedTodo)
        {
            CompletedTodo = completedTodo;
        }
    }
}