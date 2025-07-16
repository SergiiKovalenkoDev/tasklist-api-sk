using Cortex.Mediator.Notifications;

namespace TaskListApi.Notifications;

public class ErrorNotification : INotification
{
    public Exception Exception { get; }
    public HttpContext Context { get; }

    public ErrorNotification(Exception exception, HttpContext context)
    {
        Exception = exception;
        Context = context;
    }
}
