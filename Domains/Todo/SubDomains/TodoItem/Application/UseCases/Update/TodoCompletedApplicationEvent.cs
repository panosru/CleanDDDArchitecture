using Aviant.Application.ApplicationEvents;
using Microsoft.Extensions.Logging;

namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Update;

internal sealed record TodoCompletedApplicationEvent(TodoItemViewModel CompletedTodo) : ApplicationEvent;

internal sealed partial class TodoCompletedApplicationEventHandler(ILogger<TodoCompletedApplicationEventHandler> logger)
    : ApplicationEventHandler<TodoCompletedApplicationEvent>
{
    public override Task Handle(TodoCompletedApplicationEvent @event, CancellationToken cancellationToken)
    {
        LogCompleted(@event.CompletedTodo.Id, @event.CompletedTodo.Title);

        return Task.CompletedTask;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Todo {Id} ({Title}) completed")]
    private partial void LogCompleted(int id, string title);
}
