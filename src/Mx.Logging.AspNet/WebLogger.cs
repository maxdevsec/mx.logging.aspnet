using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Mx.Library.Logging;
using Serilog;
using Serilog.Events;


namespace Mx.Logging.AspNet
{
    public static class WebLogger
    {
        public static void LogWebUsage(string application, string layer, string activityName, HttpContext context,
            Dictionary<string, object> additionalInfo = null)
        {
            var details = GetWebLogDetail(application, layer, activityName, context, additionalInfo);
            Logger.WriteUsage(details);
        }

        public static void LogWebDiagnostic(string application, string layer, string message, HttpContext context,
            Dictionary<string, object> diagnosticInfo = null)
        {
            var details = GetWebLogDetail(application, layer, message, context, diagnosticInfo);
            Logger.WriteDiagnostic(details);
        }

        public static void LogWebError(string application, string layer, Exception ex, HttpContext context)
        {
            var details = GetWebLogDetail(application, layer, null, context, null);
            //details.Exception = ex;
            Logger.WriteError(details);
        }

        private static LogDetail GetWebLogDetail(string application, string layer, string activityName,
            HttpContext context, Dictionary<string, object> additionalInfo = null)
        {
            var detail = new LogDetail
            {
                Application = application,
                Layer = layer,
                Message = activityName,
                Hostname = Environment.MachineName,
                CorrelationId = Activity.Current?.Id ?? context.TraceIdentifier,
                AdditionalInfo = additionalInfo ?? new Dictionary<string, object>()
            };

            SetUserData(detail,context);
            SetRequestData(detail, context);

            return detail;
        }

        private static void SetRequestData(LogDetail logDetail, HttpContext context)
        {
            var request = context.Request;
            if (request != null)
            {
                logDetail.Location = request.Path;
                logDetail.AdditionalInfo.Add("UserAgent", request.Headers["User-Agent"]);
                logDetail.AdditionalInfo.Add("Languages", request.Headers["Accept-Language"]);

                var queryParameters = QueryHelpers.ParseQuery(request.QueryString.ToString());

                foreach (var key in queryParameters.Keys)
                {
                    logDetail.AdditionalInfo.Add($"QueryString-{key}", queryParameters[key]);
                }
            }
            
        }

        private static void SetUserData(LogDetail logDetail, HttpContext context)
        {
            var user = context.User;
            if (user != null)
            {
                var i = 1;
                foreach (var claim in user.Claims)
                {
                    if (claim.Type == ClaimTypes.NameIdentifier)
                    {
                    } else if (claim.Type != "name") // Do not include user name in log entries
                    {
                        logDetail.AdditionalInfo.Add($"UserClaim-{i++}{claim.Type}", claim.Value);
                    }
                }
            }
            
        }
    }
}