using System;
using TextParser.Core.Interfaces;

namespace TextParser.Core.Utilities {
    public class ConsoleWriter : IConsoleWriter {
        public void WriteLine(string input) {
            Console.WriteLine(input);
        }
    }
}
