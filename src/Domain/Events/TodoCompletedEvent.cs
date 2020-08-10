namespace CleanDDDArchitecture.Domain.Events
{
    using Aviant.DDD.Domain.Events;
    using Entities;

    public class TodoCompletedEvent : EventBase
    {
        public TodoItemEntity CompletedTodo { get; private set; }

        public TodoCompletedEvent(TodoItemEntity completedTodo)
        {
            CompletedTodo = completedTodo;
        }
    }
}