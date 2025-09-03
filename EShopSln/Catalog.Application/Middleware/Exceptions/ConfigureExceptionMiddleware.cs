using Microsoft.AspNetCore.Builder;

namespace Catalog.Application.Middleware.Exceptions;

public static class ConfigureExceptionMiddleware
{
    public static void ConfigureExceptionHandlingMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
    }
}