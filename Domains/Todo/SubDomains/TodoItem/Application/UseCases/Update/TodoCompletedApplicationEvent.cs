namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Update
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.ApplicationEvents;

    internal sealed class TodoCompletedApplicationEvent : ApplicationEvent
    {
        public TodoCompletedApplicationEvent(TodoItemViewModel completedTodo) => CompletedTodo = completedTodo;

        public TodoItemViewModel CompletedTodo { get; }
    }

    internal sealed class TodoCompletedApplicationEventHandler : ApplicationEventHandler<TodoCompletedApplicationEvent>
    {
        public override Task Handle(TodoCompletedApplicationEvent @event, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Todo {@event.CompletedTodo.Title} Completed Event handled");

            return Task.CompletedTask;
        }
    }
}