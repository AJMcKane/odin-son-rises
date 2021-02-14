using Microsoft.Extensions.Logging;
using Moq;
using System;
using TextParser.Core.Interfaces;
using TextParser.Core.Utilities;
using Xunit;

namespace TextParser.Test {
    public class ConsoleLoggerTests {

        private ICustomLogger loggerSubject;
        private Mock<IConsoleWriter> consoleWriterMock;

        public ConsoleLoggerTests() {
            consoleWriterMock = new Mock<IConsoleWriter>();
        }

        [Fact]
        public void TestLoggerLogsLevelAndMessageAsString() {
            loggerSubject = new ConsoleLogger(LogLevel.Information, consoleWriterMock.Object);
            loggerSubject.Log(LogLevel.Information, "Test Log");
            consoleWriterMock.Verify(c => c.WriteLine(It.Is<string>(p => p.Equals("INFORMATION | Test Log"))));
        }

        [Fact]
        public void TestLoggerOnlyLogsAboveMinimumLevel() {
            var successString = "This will be logged";
            loggerSubject = new ConsoleLogger(LogLevel.Trace, consoleWriterMock.Object);
            loggerSubject.Log(LogLevel.Information, successString);
            loggerSubject = new ConsoleLogger(LogLevel.Error, consoleWriterMock.Object);
            loggerSubject.Log(LogLevel.Information, "This Won't Be Logged");
            consoleWriterMock.Verify(c => c.WriteLine(It.Is<string>(p => p.Equals($"INFORMATION | {successString}"))));
            consoleWriterMock.Verify(c => c.WriteLine(It.IsAny<string>()), Times.Once);
        }
    }
}
