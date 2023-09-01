using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Mx.Logging.AspNet
{
    public class TrackUsageAttribute : ActionFilterAttribute
    {
        private readonly string _application;
        private readonly string _layer;
        private readonly string _activityName;

        public TrackUsageAttribute(string application, string layer, string activityName)
        {
            _application = application;
            _layer = layer;
            _activityName = activityName;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var routeDictionary = new Dictionary<string, object>();
            if (context.RouteData.Values?.Keys != null)
            {
                foreach (var key in context.RouteData.Values?.Keys)
                {
                    routeDictionary.Add($"RouteData-{key}", (string)context.RouteData.Values[key]);
                }
                WebLogger.LogWebUsage(_application, _layer, _activityName, context.HttpContext, routeDictionary);
            }
        }
    }
}