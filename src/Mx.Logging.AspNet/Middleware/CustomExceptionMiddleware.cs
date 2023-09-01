using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Mx.Library.Logging;


namespace Mx.Logging.AspNet.Middleware
{
 public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ExceptionHandlerOptions _options;
        private readonly Func<object, Task> _clearCacheHeadersDelegate;
        private readonly ILoggerFactory _loggerFactory;
        private readonly string _application;



        public CustomExceptionMiddleware(string application, RequestDelegate next, ILoggerFactory loggerFactory,
            IOptions<ExceptionHandlerOptions> options, DiagnosticSource diagnosticSource)
        {

            _application = application;
            _next = next;
            _options = options.Value;
            _clearCacheHeadersDelegate = ClearCacheHeaders;
            _loggerFactory = loggerFactory;

            _options.ExceptionHandler = _next;
        }


        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                
                var errorId = Activity.Current?.Id ?? context.TraceIdentifier;

                var logDetail = new LogDetail
                {
                    Application = _application,
                    CorrelationId = errorId,
                    //Exception = ex,
                    Message = ex.Message,
                    Hostname = Environment.MachineName

                };

                LogError(logDetail);
                
                
                var originalPath = context.Request.Path;
                if (_options.ExceptionHandlingPath.HasValue)
                {
                    context.Request.Path = _options.ExceptionHandlingPath;
                }

                context.Response.Clear();
                var exceptionHandlerFeature = new ExceptionHandlerFeature()
                {
                    Error = ex,
                    Path = context.Request.Path,
                };

                context.Features.Set<IExceptionHandlerFeature>(exceptionHandlerFeature);
                context.Features.Set<IExceptionHandlerPathFeature>(exceptionHandlerFeature);
                context.Response.StatusCode = 200;
                context.Response.OnStarting(_clearCacheHeadersDelegate, context.Response);
                

                if (_options != null) await _options.ExceptionHandler?.Invoke(context);
            }
        }

        private void LogError(LogDetail logDetail)
        {
            var logger = _loggerFactory.CreateLogger<CustomExceptionMiddleware>();
            logger.LogError("An unhandled exception occured " 
                            + logDetail.Message + " with {timestamp} {message} {hostname} {application} {correlationId} {eventType}", 
                            logDetail.TimeStamp, logDetail.Message, logDetail.Hostname, logDetail.Application, logDetail.CorrelationId, logDetail.EventType);
        }

        private Task ClearCacheHeaders(object state)
        {
            var response = (HttpResponse)state;
            response.Headers[HeaderNames.CacheControl] = "no-cache";
            response.Headers[HeaderNames.Pragma] = "no-cache";
            response.Headers[HeaderNames.Expires] = "-1";
            response.Headers.Remove(HeaderNames.ETag);
            return Task.CompletedTask;
        }

    }
}