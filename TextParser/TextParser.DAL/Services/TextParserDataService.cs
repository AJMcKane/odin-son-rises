using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextParser.DAL.Interfaces;
using TextParser.DAL.Models;

namespace TextParser.DAL.Services {
    /// <summary>
    /// This would be our EFCore IRepo wrapper class
    /// </summary>
    public class TextParserDataService : ITextParserDataService<LeaseNoticeSchedule> {
        List<LeaseNoticeSchedule> leaseNoticeSchedules;

        public TextParserDataService() {
            leaseNoticeSchedules = new List<LeaseNoticeSchedule>();
        }

        public Task Save(LeaseNoticeSchedule item) {
            leaseNoticeSchedules.Add(item);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<LeaseNoticeSchedule>> GetAll() {
            return Task.FromResult(leaseNoticeSchedules.AsEnumerable());
        }
    }
}
