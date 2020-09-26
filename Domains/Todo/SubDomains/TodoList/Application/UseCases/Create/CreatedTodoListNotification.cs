namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Notifications;

    public class CreatedTodoListNotification : Notification
    {
        public string Name { get; set; }
    }

    public class TodoCreatedNotificationHandler : NotificationHandler<CreatedTodoListNotification>
    {
        public override Task Handle(CreatedTodoListNotification notification, CancellationToken cancellationToken)
        {
            Console.WriteLine(notification.Name);

            return Task.CompletedTask;
        }
    }
}