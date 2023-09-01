
using Microsoft.AspNetCore.Builder;

namespace Mx.Logging.AspNet.Middleware
{
    public static class RequestTracingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestTracing(this IApplicationBuilder builder, string application)
        {
            return builder.UseMiddleware<RequestTracingMiddleware>(application);

        }
    }
}
