using Aviant.Application.ApplicationEvents;
using Microsoft.Extensions.Logging;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create;

internal sealed record CreatedTodoListApplicationEvent(string Name) : ApplicationEvent
{
    public string Name { get; set; } = Name;
}

internal sealed partial class TodoCreatedApplicationEventHandler(ILogger<TodoCreatedApplicationEventHandler> logger)
    : ApplicationEventHandler<CreatedTodoListApplicationEvent>
{
    public override Task Handle(
        CreatedTodoListApplicationEvent @event,
        CancellationToken               cancellationToken)
    {
        LogListCreated(@event.Name);

        return Task.CompletedTask;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Todo list {Name} created")]
    private partial void LogListCreated(string name);
}
