using System;
using System.Globalization;
using System.IO;

namespace OpenGSGLibrary.Tools
{
    public enum LogLevel
    {
        Info,
        Warning,
        Err,
        Fatal
    }

    public class Logger
    {
        private StreamWriter? logFile_ = null;

        /// <summary>
        /// Constructor for logger.
        /// Checks if log folder exists and takes care of archiving of old log files if necessary,
        /// so that we are good to go.
        /// </summary>
        /// <param name="logName">Name of log file</param>
        /// <param name="logPath">Path for main log file, old versions are archived in subfolder "LogArchive"</param>
        public Logger(string logName, string logPath)
        {
            if (!Directory.Exists(logPath))
                throw new DirectoryNotFoundException("Log directory not found: " + logPath);

            var fullLogFilePath = Path.Combine(logPath, logName);
            if (File.Exists(fullLogFilePath))
            {
                ArchiveLogFile(fullLogFilePath);
            }

            // open stream for append
            logFile_ = new StreamWriter(fullLogFilePath, append: true);
        }

        /// <summary>
        /// Immediately writes one line of output into log file.
        /// System time in milliseconds and log level will be written in front of message.
        /// </summary>
        /// <param name="logLevel">LogLevel from LogLevel enumeration, will be noted as I|W|E|F.</param>
        /// <param name="message">Message to write into log.</param>
        public void WriteLine(LogLevel logLevel, string message)
        {
            var outputLine = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture) + "\t";

            switch (logLevel)
            {
                case LogLevel.Info:
                    outputLine = outputLine + "I" + "\t";
                    break;
                case LogLevel.Warning:
                    outputLine = outputLine + "W " + "\t";
                    break;
                case LogLevel.Err:
                    outputLine = outputLine + "E " + "\t";
                    break;
                case LogLevel.Fatal:
                    outputLine = outputLine + "F " + "\t";
                    break;
            }

            outputLine = outputLine + message;

            if (logFile_ is null)
                throw new InvalidOperationException("Logger not initialized");

            logFile_.WriteLine(outputLine);
            logFile_.Flush();
        }

        private void ArchiveLogFile(string fullFilePath)
        {
            var logFileName = Path.GetFileName(fullFilePath);
            var archivePath = Path.Combine(Path.GetDirectoryName(fullFilePath) ?? string.Empty, "LogArchive");
            if (!Directory.Exists(archivePath))
                Directory.CreateDirectory(archivePath);

            var archiveFilePath = GetFreeLogFilePath(archivePath, logFileName);
            File.Move(fullFilePath, archiveFilePath);
        }

        private string GetFreeLogFilePath(string archivePath, string logFileName)
        {
            var coreName = Path.GetFileNameWithoutExtension(logFileName) + "_" + DateTime.Today.ToString("yyyyMMdd");
            var proposedName = coreName + ".log";
            var i = 0;
            while (File.Exists(Path.Combine(archivePath, proposedName)))
            {
                i += 1;
                proposedName = coreName + "_" + i.ToString() + ".log";
            }

            return Path.Combine(archivePath, proposedName);
        }
    }
}
