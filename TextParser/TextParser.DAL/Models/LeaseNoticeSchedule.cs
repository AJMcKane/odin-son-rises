using System;

namespace TextParser.DAL.Models {
    public class LeaseNoticeSchedule {
        public Guid Id { get; set; }
        public string RegistrationDateAndPlan { get; set; }
        public string PropertyDescription { get; set; }
        public string DateOfLeaseAndTerm { get; set; }
        public string LesseesTitle { get; set; }
        public string Notes { get; set; }

        public override string ToString() {
            return $"Id: {Id}  \r\n" +
                   $"|  {RegistrationDateAndPlan}\r\n   " +
                   $"|  {PropertyDescription}\r\n   " +
                   $"| Date of Lease and Term: {DateOfLeaseAndTerm}\r\n    " +
                   $"| Lessee’s title: {LesseesTitle}\r\n  " +
                   $"| Notes: {Notes} \r\n\r\n";
        }
    }
}
