using System;
using Mx.Library.Logging;
using Serilog;
using Serilog.Events;


namespace Mx.Logging.AspNet
{
    public static class Logger
    {
        private static readonly ILogger PerfLogger;
        private static readonly ILogger UsageLogger;
        private static readonly ILogger ErrorLogger;
        private static readonly ILogger DiagnosticLogger;

        static Logger()
        {
            PerfLogger = new LoggerConfiguration()
                .WriteTo.File(path: "perf.txt")
                .CreateLogger();

            UsageLogger = new LoggerConfiguration()
                .WriteTo.File("usage.txt")
                .CreateLogger();

            ErrorLogger = new LoggerConfiguration()
                .WriteTo.File(path: "error.txt")
                .CreateLogger();

            DiagnosticLogger = new LoggerConfiguration()
                .WriteTo.File(path: "diagnostic.txt")
                .CreateLogger();
        }

        public static void WritePerf(LogDetail logDetail)
        {
            PerfLogger.Write(LogEventLevel.Information, "{@LogDetail}", logDetail);
        }

        public static void WriteUsage(LogDetail logDetail)
        {
            UsageLogger.Write(LogEventLevel.Information, "{@LogDetail}", logDetail);
        }

        public static void WriteError(LogDetail logDetail)
        {
            ErrorLogger.Write(LogEventLevel.Information, "{@LogDetail}", logDetail);
        }

        public static void WriteDiagnostic(LogDetail logDetail)
        {
            DiagnosticLogger.Write(LogEventLevel.Information, "{@LogDetail}", logDetail);
        }

        private static string GetMessageFromException(Exception ex)
        {
            while (true)
            {
                if (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    continue;
                }

                return ex.Message;
            }
        }
    }
}