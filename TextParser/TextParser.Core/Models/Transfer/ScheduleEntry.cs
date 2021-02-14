using System;
using System.Collections.Generic;
using System.Text;

namespace TextParser.Core.Models.Transfer {
    public class ScheduleEntry {
        public int EntryNumber { get; set; }
        public DateTime? EntryDate { get; set; }
        public string EntryType { get; set; }
        public IEnumerable<string> EntryText { get; set; }
    }
}
