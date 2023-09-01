using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mx.Library.ExceptionHandling;
using Mx.Library.Logging;
using Mx.Library.Serialization;


namespace Mx.Logging.AspNet.Middleware
{
    public class CustomApiExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILoggerFactory _loggerFactory;
        private readonly string _application;
        private readonly bool _useStructuredLogging;
        private const int ExceptionStatusCode = 500;
        private const string DefaultExceptionMessage = "An unexpected error occured";
        private const string DefaultExceptionTitle = "An exception occurred";

        public CustomApiExceptionMiddleware(string application, bool useStructuredLogging, RequestDelegate next, ILoggerFactory loggerFactory,
            IOptions<ExceptionHandlerOptions> options, DiagnosticSource diagnosticSource)
        {
            _application = application;
            _next = next;
            _useStructuredLogging = useStructuredLogging;
            var exceptionHandlerOptions = options.Value;
            _loggerFactory = loggerFactory;

            if (exceptionHandlerOptions.ExceptionHandler == null)
            {
                exceptionHandlerOptions.ExceptionHandler = _next;
            }

        }


        public async Task Invoke(HttpContext context)
        {

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = GetStatus(ex);
                context.Response.ContentType = "application/json";

                var logger = _loggerFactory.CreateLogger<CustomApiExceptionMiddleware>();

                var errorId = Activity.Current?.Id ?? context.TraceIdentifier;

                var userData = WebHelper.GetUserData(context);
                
                if (_useStructuredLogging)
                {
                    var logDetail = new LogDetail
                    {
                        Application = _application,
                        CorrelationId = errorId,
                        //Exception = ex,
                        Message = ex.Message,
                        Hostname = Environment.MachineName,
                        UserId = userData.UserId,
                        EventType = ex.GetType().Name
                    };

                    LogError(logDetail, logger, ex);
                }
                else
                {
                    logger.LogError(ex, GetErrorMessage(ex));
                }

                var errorResponse = new ProblemDetails(errorId, ex.GetType().Name, DefaultExceptionTitle, GetStatus(ex), GetErrorMessage(ex), GetRequestPath(context));

                await context.Response.WriteAsync(errorResponse.ToJson(), Encoding.UTF8);

            }
        }

        private void LogError(LogDetail logDetail, ILogger<CustomApiExceptionMiddleware> logger, Exception ex)
        {
            
            logger.LogError("An unhandled exception occurred "
                            + logDetail.Message + " with {timestamp} {message} {hostname} {application} {correlationId} {eventType} {@exception}",
                logDetail.TimeStamp, logDetail.Message, logDetail.Hostname, logDetail.Application, logDetail.CorrelationId, logDetail.EventType, ex);
        }
        private string GetRequestPath(HttpContext context)
        {
            if (context != null)
            {
                return context.Request.Path;
            }
            else
            {
                return string.Empty;
            }
        }

        private static string GetErrorMessage(Exception ex)
        {
            return ex is MxException ? ex.Message : DefaultExceptionMessage;
        }

        private static int GetStatus(Exception ex)
        {
            if (ex is MxException exception)
            {
                return exception.Status;
            }
            else
            {
                return 500;
            }
        }
    }
}
