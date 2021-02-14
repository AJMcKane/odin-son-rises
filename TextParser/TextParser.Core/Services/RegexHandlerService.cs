using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TextParser.Core.Interfaces;

namespace TextParser.Core.Services {
    public class RegexHandlerService : IRegexHandlerService {
        public string RemoveContentWithWhitespaceByPattern(string pattern, string textToProcess, RegexOptions options = RegexOptions.None) {
            var removalRegex = new Regex(pattern, options);
            return removalRegex.Replace(textToProcess, string.Empty);
        }

        public string[] SplitContentByPattern(string pattern, string textToProcess, RegexOptions options = RegexOptions.None) {
            var splitRegex = new Regex(pattern, options);
            return splitRegex.Split(textToProcess);
        }
    }
}
