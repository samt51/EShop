using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Order.Application.Middleware.Exceptions
{
    public class ExceptionMiddleware : IMiddleware
    {
        private static readonly JsonSerializerOptions JsonOpt = new(JsonSerializerDefaults.Web);

        private readonly ILogger<ExceptionMiddleware> _logger;
        public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger) => _logger = logger;

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                var userId = context.User?.FindFirst("Id")?.Value ?? "-";
                using (_logger.BeginScope(new Dictionary<string, object> { ["UserId"] = userId }))
                    _logger.LogError(ex, "Unhandled exception");

                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            var statusCode = exception is ValidationException
                ? StatusCodes.Status422UnprocessableEntity
                : StatusCodes.Status500InternalServerError;

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/json; charset=utf-8";

            if (exception is ValidationException vex)
            {
                var payload = new
                {
                    statusCode = StatusCodes.Status400BadRequest,
                    isSuccess  = false,
                    errors     = vex.Errors.Select(e => e.ErrorMessage).ToList()
                };
                var json = JsonSerializer.Serialize(payload, JsonOpt);
                return httpContext.Response.WriteAsync(json);
            }

            var generic = new
            {
                statusCode,
                isSuccess = false,
                errors    = new[] { "Internal Server Error" }
            };
            var genericJson = JsonSerializer.Serialize(generic, JsonOpt);
            return httpContext.Response.WriteAsync(genericJson);
        }
    }
}
