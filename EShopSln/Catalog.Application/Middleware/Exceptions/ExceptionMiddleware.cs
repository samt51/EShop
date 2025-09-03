using EShop.Shared.Dtos.BasesResponses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Context;
using ValidationException = FluentValidation.ValidationException;

namespace Catalog.Application.Middleware.Exceptions;

   public class ExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception ex)
            {
                var context = httpContext.User?.Identity?.IsAuthenticated != null || true ? httpContext.User.Identities.Select(x => x.FindFirst("Id"))?.FirstOrDefault() : null;
                if (context is not null)
                {
                    LogContext.PushProperty("UserId", context.Value.ToString());
                }
                _logger.Log(LogLevel.Error, ex.Message);
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            var code = exception is ValidationException
                ? StatusCodes.Status400BadRequest
                : StatusCodes.Status500InternalServerError;

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = code;

            var errors = exception is ValidationException vEx
                ? vEx.Errors.Select(e => e.ErrorMessage).ToList()
                : new List<string> { exception.Message };

            var response = new ResponseDto<object>()
                .Fail(null, errors, code);

            return httpContext.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
    }
