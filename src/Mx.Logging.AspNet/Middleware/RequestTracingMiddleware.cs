
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Mx.Library.Logging;


namespace Mx.Logging.AspNet.Middleware
{
    public class RequestTracingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestTracingMiddleware> _logger;
        private readonly string _application;



        public RequestTracingMiddleware(string application, RequestDelegate next, ILoggerFactory loggerFactory)
        {

            _application = application;
            _next = next;
            _logger = loggerFactory.CreateLogger<RequestTracingMiddleware>();

        }

        public async Task Invoke(HttpContext context)
        {

           var correlationId = Guid.NewGuid();
           LogMessage($"Request made to :{context.Request.Path}", correlationId);
            
           await _next(context).ConfigureAwait(false);

           LogMessage($"Request to :{context.Request.Path} completed", correlationId);
        }

        private void LogMessage(string message, Guid correlationId)
        {
            var logDetail = new LogDetail
            {

                Application = _application,
                CorrelationId = correlationId.ToString(),
               // Exception = null,
                Message = message,
                Hostname = Environment.MachineName

            };

            _logger.LogInformation(logDetail.Message + " with {@LogDetail}", logDetail);
        }
    }
}