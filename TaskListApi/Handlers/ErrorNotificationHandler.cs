using Cortex.Mediator.Notifications;
using TaskListApi.Notifications;

namespace TaskListApi.Handlers;

public class ErrorNotificationHandler : INotificationHandler<ErrorNotification>
{
    public Task Handle(ErrorNotification notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Error: {notification.Exception.Message}");
        return Task.CompletedTask;
    }
}
