using System.Collections.Generic;

namespace TextParser.Core.Models.Transfer {
    public class LeaseSchedule {
        public string ScheduleType { get; set; }
        public IEnumerable<ScheduleEntry> ScheduleEntry {get; set;}
    }
}
