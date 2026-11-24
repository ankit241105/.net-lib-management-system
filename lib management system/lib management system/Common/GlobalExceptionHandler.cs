using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace lib_management_system.Common
{
    // Catches ANY unhandled exception that escapes the controllers/services/repositories,
    // logs the real details for us (via Serilog), and returns a clean, safe JSON response
    // to the client instead of a raw 500 + stack trace.
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
            => _logger = logger;

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            // The id ASP.NET Core assigns to this request. We log it AND return it,
            // so a user who hit an error can give it to us and we grep the logs for it.
            var traceId = httpContext.TraceIdentifier;

            // Full exception (type, message, stack trace) goes to OUR logs only.
            _logger.LogError(
                exception,
                "Unhandled exception. TraceId: {TraceId}, Path: {Path}",
                traceId,
                httpContext.Request.Path);

            // The client gets a generic, standards-based (RFC 7807) error body -
            // no stack trace, no internal details leaked.
            var problem = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred.",
                Detail = "The server encountered an error while processing your request.",
                Instance = httpContext.Request.Path
            };
            problem.Extensions["traceId"] = traceId;

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

            // true = "I handled it, don't fall through to the default 500 page."
            return true;
        }
    }
}
