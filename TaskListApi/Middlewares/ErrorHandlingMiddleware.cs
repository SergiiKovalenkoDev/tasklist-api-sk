using Cortex.Mediator;
using TaskListApi.Notifications;

namespace TaskListApi.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var mediator = context.RequestServices.GetRequiredService<IMediator>();
            await mediator.PublishAsync(new ErrorNotification(ex, context));
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
    }
}
