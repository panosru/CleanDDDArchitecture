namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.ApplicationEvents;
    using Polly;

    internal sealed class CreatedTodoListApplicationEvent : ApplicationEvent
    {
        public CreatedTodoListApplicationEvent(string name) => Name = name;

        public string Name { get; set; }
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
            Console.WriteLine($"from 2 {@event.Name}");

            throw new ArgumentException("Test2");
        }

        public override IAsyncPolicy RetryPolicy() =>
            Policy
               .Handle<ArgumentException>()
               .WaitAndRetryAsync(
                    3,
                    i => TimeSpan.FromSeconds(i));
    }
}