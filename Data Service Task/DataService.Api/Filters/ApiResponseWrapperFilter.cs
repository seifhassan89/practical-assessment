using DataService.Api.Middleware;
using DataService.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DataService.Api.Filters;

/// <summary>
/// A global result filter that automatically wraps every successful (2xx) <see cref="ObjectResult"/>
/// in a <see cref="SuccessApiResponse{T}"/> envelope before the response is written to the wire.
/// <para>
/// Controllers return their domain objects directly; the wrapping happens here, once, for
/// the entire application. To opt a specific action out of wrapping, decorate it with
/// <see cref="SkipResponseWrapperAttribute"/>.
/// </para>
/// </summary>
public sealed class ApiResponseWrapperFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        bool skip = context.ActionDescriptor.EndpointMetadata
            .OfType<SkipResponseWrapperAttribute>()
            .Any();

        ObjectResult? objectResult = context.Result as ObjectResult;
        int statusCode = objectResult is not null
            ? objectResult.StatusCode ?? StatusCodes.Status200OK
            : StatusCodes.Status200OK;

        if (!skip
            && objectResult is not null
            && statusCode is >= StatusCodes.Status200OK and < StatusCodes.Status300MultipleChoices
            && objectResult.Value is not null
            && objectResult.Value is not ApiResponse)   // guard: never double-wrap
        {
            string correlationId = context.HttpContext.Items[CorrelationIdMiddleware.ItemsKey] as string
                ?? context.HttpContext.TraceIdentifier;

            objectResult.Value = ApiResponse.Ok(objectResult.Value, correlationId);
        }

        await next();
    }
}

/// <summary>
/// Decorating an action with this attribute prevents <see cref="ApiResponseWrapperFilter"/>
/// from wrapping its response. Useful for endpoints that must return a raw payload
/// (e.g. file downloads, health checks, WebSocket upgrades).
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed class SkipResponseWrapperAttribute : Attribute;
