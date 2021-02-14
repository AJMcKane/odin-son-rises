using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TextParser.Core.Enums;
using TextParser.Core.Extensions;
using TextParser.Core.Interfaces;
using TextParser.Core.Models.Transfer;
using TextParser.DAL.Models;

namespace TextParser.Core.Services {
    public class EntryTextParserService : IEntryTextParserService<RawEntryTextOutput, LeaseNoticeSchedule> {
        private const int MAX_CONCURRENT_TASK_COUNT = 10;
        private ICustomLogger logger;
        private IRowDataParserService rowDataParser;

        public EntryTextParserService(ICustomLogger logger, IRowDataParserService rowDataParser) {
            this.rowDataParser = rowDataParser;
            this.logger = logger;
        }
        
        public async Task<IEnumerable<LeaseNoticeSchedule>> ParseEntryTextCollection(IEnumerable<RawEntryTextOutput> rawTextCollection) {
            var structuredData = await rawTextCollection.FunctionCollectionWithMaxConcurrency(ParseEntryText, MAX_CONCURRENT_TASK_COUNT);
            return structuredData;
        }

        public async Task<LeaseNoticeSchedule> ParseEntryText(RawEntryTextOutput item) {
            if (item.EntryText.Count() > 0) {
                StringBuilder registrationStringBuilder = new StringBuilder(), propDescriptionStringBuilder = new StringBuilder(),
                dateOfLeaseStringBuilder = new StringBuilder(), lesseesStringBuilder = new StringBuilder(), noteStringBuilder = new StringBuilder();
                var entryTextArray = item.EntryText.Where(p => !string.IsNullOrWhiteSpace(p))
                                                   .ToArray();
                var lastRowState = RowState.Standard;
                for (int i = 0; i < entryTextArray.Length; i++) {
                    lastRowState = await rowDataParser.ParseRowData(i, 
                                                                    entryTextArray[i],
                                                                    registrationStringBuilder,
                                                                    propDescriptionStringBuilder,
                                                                    dateOfLeaseStringBuilder,
                                                                    lesseesStringBuilder,
                                                                    noteStringBuilder,
                                                                    lastRowState);                 
                }

                return new LeaseNoticeSchedule() {
                    Id = Guid.NewGuid(),
                    RegistrationDateAndPlan = registrationStringBuilder.ToString().Trim(),
                    PropertyDescription = propDescriptionStringBuilder.ToString().Trim(),
                    DateOfLeaseAndTerm = dateOfLeaseStringBuilder.ToString().Trim(),
                    LesseesTitle = lesseesStringBuilder.ToString().Trim(),
                    Notes = noteStringBuilder.ToString().Trim(),
                };
            } else {
                //We would build on this and give a diagnostic Row ID + details success metrics for troubleshooting
                logger.Log(LogLevel.Warning, "Invalid RawEntryTextOutput Item passed for parsing, Entry Text contained no rows");
            }

            return new LeaseNoticeSchedule();
        }        
    }
}
