using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Mx.Logging.AspNet.Middleware
{
    public static class CustomExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder, string application, string errorHandlingPath)
        {
            return builder.UseMiddleware<CustomExceptionMiddleware>(application, Options.Create(
                new ExceptionHandlerOptions
                {
                    ExceptionHandlingPath = new PathString(errorHandlingPath)
                }));

        }
    }
}