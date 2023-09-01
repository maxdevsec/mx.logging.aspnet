using Microsoft.AspNetCore.Builder;

namespace Mx.Logging.AspNet.Middleware
{
    public static class CustomApiExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomApiExceptionHandler(this IApplicationBuilder builder, string application, bool useStructureLogging)
        {
            return builder.UseMiddleware<CustomApiExceptionMiddleware>(application, useStructureLogging);
        }
    }
}
