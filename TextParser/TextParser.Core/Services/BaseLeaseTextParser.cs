using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using TextParser.Core.Interfaces;
using TextParser.DAL.Interfaces;

namespace TextParser.Core.Services {
    public abstract class BaseLeaseTextParser<TLeaseScheduleType, TRawEntryTextType> : ILeaseTextParserService where TLeaseScheduleType : class where TRawEntryTextType: class {
        private IRawFileParserService<TRawEntryTextType> fileParser;
        private IEntryTextParserService<TRawEntryTextType, TLeaseScheduleType> entryTextParser;
        private ITextParserDataService<TLeaseScheduleType> dataService;
        private ICustomLogger logger;

        public BaseLeaseTextParser(IRawFileParserService<TRawEntryTextType> fileParser,
            IEntryTextParserService<TRawEntryTextType, TLeaseScheduleType> entryTextParser,
            ITextParserDataService<TLeaseScheduleType> dataService,
            ICustomLogger logger) {
            this.fileParser = fileParser;
            this.entryTextParser = entryTextParser;
            this.dataService = dataService;
            this.logger = logger;
        }

        public virtual async Task<bool> ParseLeaseText(string filePath) {
            logger.Log(LogLevel.Information, "Beginning Lease Parsing");
            var entryTextOutput = await fileParser.ParseFile(filePath);            
            var structuredOutputCollection = await entryTextParser.ParseEntryTextCollection(entryTextOutput);
            foreach (var result in structuredOutputCollection) {
                logger.Log(LogLevel.Information, result.ToString());
                await dataService.Save(result);
            }
            return structuredOutputCollection?.Count() > 0;
        }
    }
}
