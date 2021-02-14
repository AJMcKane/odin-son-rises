using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextParser.DAL.Interfaces;
using TextParser.DAL.Models;

namespace TextParser.DAL.Services {
    public class TextParserDataService : ITextParserDataService<LeaseNoticeSchedule> {
        List<LeaseNoticeSchedule> leaseNoticeSchedules;

        public Task Save(LeaseNoticeSchedule item) {
            throw new NotImplementedException();
        }
    }
}
