namespace CleanDDDArchitecture.Application.TodoLists.Commands.CreateTodoList.Events
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Domain.Events;

    public class TodoCreatedEvent : Event
    {
        public string Name { get; set; }
    }

    public class TodoCreatedEventHandler : Aviant.DDD.Domain.Events.EventHandler<TodoCreatedEvent>
    {
        public override Task Handle(TodoCreatedEvent @event, CancellationToken cancellationToken)
        {
            Console.WriteLine(@event.Name);

            return Task.CompletedTask;
        }
    }
}