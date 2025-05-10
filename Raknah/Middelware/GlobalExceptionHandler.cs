using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Raknah.Middelware;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Somthing Want Wrong {exception}", exception);

        ProblemDetails problemDetails = new ProblemDetails()
        {
            Title = "Internal Server Error",
            Detail = exception.Message,
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
            Extensions =
            {
                ["traceId"] = httpContext.TraceIdentifier,
            }

        };

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);
        return new ValueTask<bool>(true);
    }



}
