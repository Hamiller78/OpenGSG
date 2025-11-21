using System;

namespace Tools
{
    /// <summary>
    /// Singleton log class using the normal logger class.
    /// Can be used from anywhere in the program as a global logger.
    /// </summary>
    public class GlobalLogger
    {
        private const string DEFAULT_LOGNAME = "GSGLib.log";

        private static Logger? defLogger_ = null;

        private GlobalLogger() { }

        public static Logger GetInstance()
        {
            if (defLogger_ is null)
            {
                defLogger_ = new Logger(DEFAULT_LOGNAME, Environment.CurrentDirectory);
            }
            return defLogger_!;
        }

        public static void Init(string logFile, string logPath)
        {
            if (defLogger_ is not null)
                throw new ApplicationException("GlobalLogger class can only be initialized once!");

            defLogger_ = new Logger(logFile, logPath);
        }
    }
}
