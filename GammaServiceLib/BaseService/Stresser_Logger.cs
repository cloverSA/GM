using System;
using System.Diagnostics;

namespace GammaStressAgent.BaseService
{
    public class Logger
    {
        public static void WriteAppLog(String source, String message, int code = 0)
        {
            EventLog.WriteEntry(source, message, EventLogEntryType.Information, code);
        }

        public static void WriteAppWarning(String source, String message, int code = 0)
        {
            EventLog.WriteEntry(source, message, EventLogEntryType.Warning, code);
        }

        public static void WriteAppError(String source, String message, int code = 0)
        {
            EventLog.WriteEntry(source, message, EventLogEntryType.Error, code);
        }
    }
}
