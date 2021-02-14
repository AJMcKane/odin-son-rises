using System;
using System.Collections.Generic;
using System.Text;

namespace TextParser.Core.Models.Transfer {
    public class RawEntryTextOutput {
        /// <summary>
        /// Collection of all Entry Text elements parsed from any input source.
        /// Each element should represent a cross-coloumn partial table row.
        /// </summary>
        public IEnumerable<string> EntryText { get; set; }
    }
}
