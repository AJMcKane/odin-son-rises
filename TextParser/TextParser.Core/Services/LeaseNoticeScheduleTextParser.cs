using TextParser.Core.Interfaces;
using TextParser.Core.Models.Transfer;
using TextParser.DAL.Interfaces;
using TextParser.DAL.Models;

namespace TextParser.Core.Services {
    public class LeaseNoticeScheduleTextParser : BaseLeaseTextParser<LeaseNoticeSchedule, RawEntryTextOutput> {
        public LeaseNoticeScheduleTextParser(IRawFileParserService<RawEntryTextOutput> fileParser,
            IEntryTextParserService<RawEntryTextOutput, LeaseNoticeSchedule> entryTextParser,
            ITextParserDataService<LeaseNoticeSchedule> dataService,
            ICustomLogger logger) : base(fileParser, entryTextParser, dataService, logger) {
        }
    }
}
