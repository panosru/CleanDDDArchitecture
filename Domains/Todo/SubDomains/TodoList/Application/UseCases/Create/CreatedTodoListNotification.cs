namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Notifications;
    using Polly;

    internal sealed class CreatedTodoListNotification : Notification
    {
        public CreatedTodoListNotification(string name) => Name = name;

        public string Name { get; set; }
    }

    internal sealed class TodoCreatedNotificationHandler : NotificationHandler<CreatedTodoListNotification>
    {
        public override Task Handle(CreatedTodoListNotification notification, CancellationToken cancellationToken)
        {
            Console.WriteLine(notification.Name);

            return Task.CompletedTask;
        }
    }

    internal sealed class TodoCreatedNotificationHandler2 : NotificationHandler<CreatedTodoListNotification>
    {
        public override Task Handle(CreatedTodoListNotification notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"from 2 {notification.Name}");

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