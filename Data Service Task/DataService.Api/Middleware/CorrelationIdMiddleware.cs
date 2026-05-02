using Serilog.Context;

namespace DataService.Api.Middleware;

/// <summary>
/// Reads the <c>X-Correlation-Id</c> request header, or generates a new ID if absent.
/// Propagates the value to:
/// <list type="bullet">
///   <item><c>HttpContext.Items</c> — consumed by controllers and exception handlers.</item>
///   <item>The response header — allows clients to correlate requests with log entries.</item>
///   <item>Serilog's <c>LogContext</c> — every log line for this request carries <c>CorrelationId</c>.</item>
/// </list>
/// </summary>
public sealed class CorrelationIdMiddleware
{
    public const string HeaderName = "X-Correlation-Id";
    public const string ItemsKey = "CorrelationId";

    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string correlationId = context.Request.Headers[HeaderName].FirstOrDefault()
            ?? Guid.NewGuid().ToString("N");

        context.Items[ItemsKey] = correlationId;

        // Always echo the correlation ID back so the caller can trace their request.
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        // Push into Serilog LogContext so every downstream log line carries CorrelationId.
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}
