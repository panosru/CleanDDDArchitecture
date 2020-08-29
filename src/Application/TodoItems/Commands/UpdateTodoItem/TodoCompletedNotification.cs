namespace CleanDDDArchitecture.Application.TodoItems.Commands.UpdateTodoItem
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Aviant.DDD.Application.Notifications;

    public class TodoCompletedNotification : Notification
    {
        public TodoCompletedNotification(TodoItemDto completedTodo)
        {
            CompletedTodo = completedTodo;
        }

        public TodoItemDto CompletedTodo { get; }
    }

    public class TodoCompletedNotificationHandler : NotificationHandler<TodoCompletedNotification>
    {
        public override Task Handle(TodoCompletedNotification notification, CancellationToken cancellationToken)
        {
            Console.WriteLine("Todo Completed Event handled");

            return Task.CompletedTask;
        }
    }
}