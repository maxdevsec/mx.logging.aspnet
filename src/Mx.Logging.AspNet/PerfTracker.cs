using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Mx.Library.Logging;


namespace Mx.Logging.AspNet
{
    /// <summary>
    /// PerfTracker - logging class used for tracking application performance
    /// </summary>
    public class PerfTracker
    {
        private readonly Stopwatch _stopwatch;
        private readonly LogDetail _logDetail;

        public PerfTracker(string name, string userId, string location, string application, string layer)
        {
            _stopwatch = Stopwatch.StartNew();
            _logDetail = new LogDetail
            {
                Message = name,
                UserId = userId,
                Application = application,
                Layer = layer,
                Location = location,
                Hostname = Environment.MachineName
            };

            var beginTime = DateTime.Now;
            _logDetail.AdditionalInfo = new Dictionary<string, object>()
            {
                {"Started", beginTime.ToString(CultureInfo.CurrentCulture)}   
            };
        }

        public PerfTracker(string name, string userId, string location, string application, string layer,
            Dictionary<string, object> perfParameters) : this(name, userId, location, application, layer)
        {
            foreach (var perfParameter in perfParameters)
            {
                _logDetail.AdditionalInfo.Add($"input-{perfParameter.Key}", perfParameter.Value);
            }
        }

        /// <summary>
        /// Stop the performance stopwatch and write a log entry 
        /// </summary>
        public void Stop()
        {
            _stopwatch.Stop();
            _logDetail.ElapsedMilliseconds = _stopwatch.ElapsedMilliseconds;
            Logger.WritePerf(_logDetail);

        }
    }
}