namespace CleanDDDArchitecture.Application.TodoItems.Commands.UpdateTodoItem
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Domain.Events;

    public class TodoCompletedEvent : EventBase
    {
        public TodoCompletedEvent(TodoItemDto completedTodo)
        {
            CompletedTodo = completedTodo;
        }

        public TodoItemDto CompletedTodo { get; }
    }

    public class TodoCompletedEventHandler : EventHandlerBase<TodoCompletedEvent>
    {
        public override Task Handle(TodoCompletedEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine("Todo Completed Event handled");

            return Task.CompletedTask;
        }
    }
}