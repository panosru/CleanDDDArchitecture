using Aviant.Application.ApplicationEvents;
namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create;

internal sealed record CreatedTodoListApplicationEvent(string Name) : ApplicationEvent
{
    public string Name { get; set; } = Name;
}

internal sealed class TodoCreatedApplicationEventHandler : ApplicationEventHandler<CreatedTodoListApplicationEvent>
{
    public override Task Handle(
        CreatedTodoListApplicationEvent @event,
        CancellationToken               cancellationToken)
    {
        Console.WriteLine(@event.Name);

        return Task.CompletedTask;
    }
}

internal sealed class TodoCreatedApplicationEventHandler2 : ApplicationEventHandler<CreatedTodoListApplicationEvent>
{
    public override Task Handle(
        CreatedTodoListApplicationEvent @event,
        CancellationToken               cancellationToken)
    {
        return Task.CompletedTask;
    }
}
