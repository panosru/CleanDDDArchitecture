using Aviant.Application.ApplicationEvents;
using Polly;

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
    private Random Random { get; } = new Random();

    public override Task Handle(
        CreatedTodoListApplicationEvent @event,
        CancellationToken               cancellationToken)
    {
        Console.WriteLine($"from 2 {@event.Name}");

        // 80% probability to fail
        if (Random.Next(100) <= 80)
            throw new ArgumentException("Test2");

        return Task.CompletedTask;
    }

    public override IAsyncPolicy RetryPolicy() =>
        Policy
           .Handle<ArgumentException>()
           .WaitAndRetryAsync(
                3,
                i => TimeSpan.FromSeconds(i));
}
