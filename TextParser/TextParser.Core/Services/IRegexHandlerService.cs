using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TextParser.Core.Interfaces {
    public interface IRegexHandlerService {
        public string RemoveContentWithWhitespaceByPattern(string pattern, string textToProcess, RegexOptions options = RegexOptions.None);
        public string[] SplitContentByPattern(string pattern, string textToProcess, RegexOptions regexOptions = RegexOptions.None);
    }
}
