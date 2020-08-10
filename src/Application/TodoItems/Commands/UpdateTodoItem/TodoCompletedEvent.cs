namespace CleanDDDArchitecture.Application.TodoItems.Commands.UpdateTodoItem
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Domain.Events;
    using Domain.Entities;

    public class TodoCompletedEvent : EventBase
    {
        public TodoCompletedEvent(TodoItemEntity completedTodo)
        {
            CompletedTodo = completedTodo;
        }

        public TodoItemEntity CompletedTodo { get; }
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