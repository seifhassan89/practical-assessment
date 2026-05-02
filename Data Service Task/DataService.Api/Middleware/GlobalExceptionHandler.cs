using DataService.Api.Exceptions;
using DataService.Api.Models;
using Microsoft.AspNetCore.Diagnostics;

namespace DataService.Api.Middleware;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        string correlationId = httpContext.Items[CorrelationIdMiddleware.ItemsKey] as string
            ?? httpContext.TraceIdentifier;

        // Cancelled requests are not errors — log at a lower level to keep noise down.
        if (exception is OperationCanceledException)
        {
            _logger.LogInformation(
                "Request cancelled | CorrelationId={CorrelationId} | Path={Path}",
                correlationId,
                httpContext.Request.Path);
        }
        else
        {
            _logger.LogError(
                exception,
                "Unhandled {ExceptionType} | CorrelationId={CorrelationId} | Path={Path} | Message={Message}",
                exception.GetType().Name,
                correlationId,
                httpContext.Request.Path,
                exception.Message);
        }

        (int statusCode, string title) = MapException(exception);

        ErrorApiResponse errorResponse = ApiResponse.Fail(
            title: title,
            status: statusCode,
            detail: exception.Message,
            instance: httpContext.Request.Path,
            correlationId: correlationId);

        // Validation errors surface field-level details when available.
        if (exception is ValidationException validationEx && validationEx.Errors.Count > 0)
        {
            errorResponse.AddExtension("errors", validationEx.Errors);
        }

        // Stack trace and exception type are only surfaced in Development to prevent
        // information disclosure in production environments.
        if (_environment.IsDevelopment())
        {
            errorResponse.AddExtension("stackTrace", exception.StackTrace);
            errorResponse.AddExtension("exceptionType", exception.GetType().FullName);
        }

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsJsonAsync(errorResponse, cancellationToken);

        return true;
    }

    /// <summary>
    /// Maps an exception to an HTTP status code and title.
    /// Custom <see cref="AppException"/> subclasses carry their own status — all other
    /// exception types are resolved via a deterministic fallback table.
    /// </summary>
    private static (int StatusCode, string Title) MapException(Exception exception) => exception switch
    {
        // Custom domain exceptions: always trust the exception's own status and title.
        AppException appEx => (appEx.StatusCode, appEx.Title),

        // ── Client errors (4xx) ──────────────────────────────────────────────
        // Syntactically malformed or missing required input.
        ArgumentNullException or ArgumentException
            => (StatusCodes.Status400BadRequest, "Invalid argument"),

        // The caller does not have a valid identity.
        UnauthorizedAccessException
            => (StatusCodes.Status401Unauthorized, "Unauthorized"),

        // A named resource could not be located.
        KeyNotFoundException
            => (StatusCodes.Status404NotFound, "Resource not found"),

        // The server timed out waiting for the request.
        TimeoutException
            => (StatusCodes.Status408RequestTimeout, "Request timed out"),

        // Client closed the connection before the response was sent — not a server fault.
        OperationCanceledException
            => (StatusCodes.Status499ClientClosedRequest, "Request cancelled by client"),

        // ── Server errors (5xx) ──────────────────────────────────────────────
        // A feature has been declared but not yet built.
        NotImplementedException
            => (StatusCodes.Status501NotImplemented, "Not implemented"),

        // Catch-all: unknown or unanticipated server errors.
        _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
    };
}
