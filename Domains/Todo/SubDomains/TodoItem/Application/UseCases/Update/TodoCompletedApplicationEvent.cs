using Aviant.Application.ApplicationEvents;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Update;

internal sealed record TodoCompletedApplicationEvent(TodoItemViewModel CompletedTodo) : ApplicationEvent;

internal sealed class TodoCompletedApplicationEventHandler : ApplicationEventHandler<TodoCompletedApplicationEvent>
{
    public override Task Handle(TodoCompletedApplicationEvent @event, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Todo {@event.CompletedTodo.Title} Completed Event handled");

        return Task.CompletedTask;
    }
}
