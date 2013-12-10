using System;

namespace TeamCityBackupTask
{
    public static class ExceptionExtensions
    {
        public static string AllMessagesWithStackTraces(this Exception exception)
        {
            while (exception.InnerException != null)
                return ExceptionMessageWithStackTrace(exception) + 
                       Environment.NewLine + 
                       Environment.NewLine + 
                       exception.InnerException.AllMessagesWithStackTraces();

            return ExceptionMessageWithStackTrace(exception);
        }

        public static string ExceptionMessageWithStackTrace(Exception exception)
        {
            return exception.Message + Environment.NewLine + (exception.StackTrace ?? "");
        }
    }
}
