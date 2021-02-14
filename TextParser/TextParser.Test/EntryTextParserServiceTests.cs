using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TextParser.Core.Interfaces;
using TextParser.Core.Models.Transfer;
using TextParser.Core.Services;
using TextParser.DAL.Models;
using Xunit;

namespace TextParser.Test {
    public class EntryTextParserServiceTests : IClassFixture<EntryTextParserServiceTestData> {
        private EntryTextParserService entryTextParserService;
        private Mock<ICustomLogger> customLogger;
        private EntryTextParserServiceTestData fixture;

        public EntryTextParserServiceTests(EntryTextParserServiceTestData fixture) {
            this.fixture = fixture;
        }


        public EntryTextParserServiceTests() {
            customLogger = new Mock<ICustomLogger>();
            entryTextParserService = new EntryTextParserService(customLogger.Object, new RowDataParserService(customLogger.Object));
        }

        [Fact]
        public async Task TextParserParsesHealthyModel() {
            var result = await entryTextParserService.ParseEntryText(fixture.HeathlyrawEntryTextOutput);
        }

        [Fact]
        public async Task TextParserParsesLossyModel() {
            var result = await entryTextParserService.ParseEntryText(fixture.LossyrawEntryTextOutput);
        }

        [Fact]
        public async Task TextParserParsesCollection() {
            var result = await entryTextParserService.ParseEntryTextCollection(fixture.EntryTextOutputCollection);
        }      

    }
    public class EntryTextParserServiceTestData {
        public EntryTextParserServiceTestData() {

        }

        public RawEntryTextOutput HeathlyrawEntryTextOutput { get; set; }
        public RawEntryTextOutput LossyrawEntryTextOutput { get; set; }
        public IEnumerable<RawEntryTextOutput> EntryTextOutputCollection { get; set; }

    }

}
