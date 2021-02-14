using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
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
            customLogger = new Mock<ICustomLogger>();
            this.fixture = fixture;
            entryTextParserService = new EntryTextParserService(customLogger.Object, new RowDataParserService(customLogger.Object));
        }

        [Fact]
        public async Task TextParserParsesHealthyModel() {
            var expectedNotes = "NOTE 1: The lease comprises also other land." +
                " NOTE 2: During the subsistance of this lease, the leases dated 30 April 1981 and 21 July 1988 referred to above take effect as underleases." +
                " NOTE 3: The lease dated referred to above has been determined as to 104 Shakespear Road (First and Second Floor Maisonette) and Unit G11 (Ground Floor).";

            var result = await entryTextParserService.ParseEntryText(fixture.HeathlyrawEntryTextOutput);
            Assert.Equal("15.04.2010 Edged and numbered 4 in blue (part of)", result.RegistrationDateAndPlan);
            Assert.Equal("Flat 705 Landmark West Tower (seventh floor flat)", result.PropertyDescription);
            Assert.Equal("11.02.2010 999 years from 1.1.2009", result.DateOfLeaseAndTerm);
            Assert.Equal("EGL570340", result.LesseesTitle);
            Assert.Equal(expectedNotes, result.Notes);
        }

        [Fact]
        public async Task TextParserParsesLossyModel() {
            var result = await entryTextParserService.ParseEntryText(fixture.LossyrawEntryTextOutput);
            Assert.Equal("17.05.2011", result.RegistrationDateAndPlan);
            Assert.Equal("3 Market Place (Basement Unit)", result.PropertyDescription);
            Assert.Equal("18.03.2011 9 years from 18/03/2011", result.DateOfLeaseAndTerm);
            Assert.Equal("AGL232764", result.LesseesTitle);
        }

        [Fact]
        public async Task TextParserParsesCollection() {
            var result = await entryTextParserService.ParseEntryTextCollection(fixture.EntryTextOutputCollection);
            Assert.Equal(2, result.Count());
        }      

    }
    public class EntryTextParserServiceTestData {
        public EntryTextParserServiceTestData() {
            HeathlyrawEntryTextOutput = new RawEntryTextOutput() {
                EntryText = new List<string>() {
                    "15.04.2010      Flat 705 Landmark West        11.02.2010      EGL570340  ",
                    "Edged and       Tower (seventh floor flat)    999 years from             ",
                    "numbered 4 in                                 1.1.2009                   ",
                    "blue (part of)",
                    "NOTE 1: The lease comprises also other land.",
                    "NOTE 2: During the subsistance of this lease, the leases dated 30 April 1981 and 21 July 1988 referred to above take effect as underleases.",
                    "NOTE 3: The lease dated referred to above has been determined as to 104 Shakespear Road (First and Second Floor Maisonette) and Unit G11 (Ground Floor)."
                }
            };

            LossyrawEntryTextOutput = new RawEntryTextOutput() {
                EntryText = new List<string>() {
                    "17.05.2011      3 Market Place (Basement      18.03.2011      AGL232764  ",
                    "Unit)                         9 years from               ",
                    "18/03/2011"
                }
            };

            EntryTextOutputCollection = new List<RawEntryTextOutput>() {
                HeathlyrawEntryTextOutput,
                LossyrawEntryTextOutput
            };
        }

        public RawEntryTextOutput HeathlyrawEntryTextOutput { get; set; }
        public RawEntryTextOutput LossyrawEntryTextOutput { get; set; }
        public IEnumerable<RawEntryTextOutput> EntryTextOutputCollection { get; set; }

    }

}
