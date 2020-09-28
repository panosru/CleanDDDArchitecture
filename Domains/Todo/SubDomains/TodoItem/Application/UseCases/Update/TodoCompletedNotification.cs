namespace CleanDDDArchitecture.Domains.Todo.SubDomains.TodoItem.Application.UseCases.Update
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Notifications;

    internal sealed class TodoCompletedNotification : Notification
    {
        public TodoCompletedNotification(TodoItemViewModel completedTodo) => CompletedTodo = completedTodo;

        public TodoItemViewModel CompletedTodo { get; }
    }

    internal sealed class TodoCompletedNotificationHandler : NotificationHandler<TodoCompletedNotification>
    {
        public override Task Handle(TodoCompletedNotification notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Todo {notification.CompletedTodo.Title} Completed Event handled");

            return Task.CompletedTask;
        }
    }
}