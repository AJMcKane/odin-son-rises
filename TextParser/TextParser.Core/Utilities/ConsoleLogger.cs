using Microsoft.Extensions.Logging;
using System;
using TextParser.Core.Interfaces;

namespace TextParser.Core.Utilities {
    public class ConsoleLogger : ICustomLogger {
        private LogLevel minimumLogLevel;
        private IConsoleWriter consoleWriter;

        public ConsoleLogger(LogLevel minimumLogLevel, IConsoleWriter consoleWriter) {
            this.minimumLogLevel = minimumLogLevel;
            this.consoleWriter = consoleWriter;
        }

        public void Log(LogLevel level, string message) {
            if (level >= minimumLogLevel) {
                consoleWriter.WriteLine($"{level.ToString().ToUpper()} | {message}");
            }
        }

        public void LogException(Exception ex, string message) {
            consoleWriter.WriteLine($"{LogLevel.Error.ToString().ToUpper()} | {message} \r\nException caught, see message for details: \r\n{ex.Message} \r\nStack Trace: {ex.StackTrace}");
        }
    }
}
