using IronPdf;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TextParser.Core.Extensions;
using TextParser.Core.Interfaces;
using TextParser.Core.Models.Transfer;

namespace TextParser.Core {
    public class IronPDFRawParserService : IRawFileParserService<RawEntryTextOutput> {
        private const string FILE_SCHEDULE_KEYWORD = "Schedule of notices of leases";
        private const int MAX_TASK_CONCURRENCY = 10;

        private ICustomLogger logger;
        private IRegexHandlerService regexHandlerService;

        public IronPDFRawParserService(ICustomLogger logger, IRegexHandlerService regexHandlerService) {
            this.logger = logger;
            this.regexHandlerService = regexHandlerService;
        }

        public async Task<IEnumerable<RawEntryTextOutput>> ParseFile(string filePath) {
            try {
                var inputPDF = PdfDocument.FromFile(filePath);

                var allpages = inputPDF.ExtractAllText();
                var trimmedPages = allpages.Remove(0, allpages.IndexOf(FILE_SCHEDULE_KEYWORD));

                //These patterns require more time and research in order to perfect as well as to make less flaky. but there's not enough time to get lost into Regex!
                var trimmedTable = regexHandlerService.RemoveContentWithWhitespaceByPattern(@"(Schedule of.*\n((?:.*\n){1,7}))|(Title number.*[\r\n].*[\r\n].*)", trimmedPages);
                //Case in point, we get an empty first element because there's nothing before the first match that we pass to the split.
                var unstructuredTableRows = regexHandlerService.SplitContentByPattern(@"^[0-9]+\s(?![a-zA-Z])", trimmedTable, RegexOptions.Multiline)
                                                               .Skip(1)
                                                               .AsEnumerable();
                logger.Log(LogLevel.Information, $"Found {unstructuredTableRows.Count()} rows of schedule content");
                var results = unstructuredTableRows.Select(p => new RawEntryTextOutput() {
                    EntryText = p.Split("\r\n")
                                    .Select(p => p.Replace("\n", " ")
                                                  .Replace("\r", " "))
                });
                //Out of time, but here we'd have to do some more complex parsing in order to normalise our parsed structure with our expected API response.  
                //The problem here is that the API response has clearer delimiters, however PDF parsers detect only one space, making column detection much harder. 


                return results;
            } catch (IOException exception) {
                logger.Log(LogLevel.Error, "IronPDF couldn't find or open the file specified, ensure the path is correct and it's not password protected");
                throw exception;
            } catch (Exception exception) {
                throw exception;
            }
        }
    }
}
