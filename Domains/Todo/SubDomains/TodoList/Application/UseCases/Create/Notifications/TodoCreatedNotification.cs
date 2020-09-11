namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoList.Application.UseCases.Create.Notifications
{
    #region

    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Notifications;

    #endregion

    public class TodoCreatedNotification : Notification
    {
        public string Name { get; set; }
    }

    public class TodoCreatedNotificationHandler : NotificationHandler<TodoCreatedNotification>
    {
        public override Task Handle(TodoCreatedNotification notification, CancellationToken cancellationToken)
        {
            Console.WriteLine(notification.Name);

            return Task.CompletedTask;
        }
    }
}