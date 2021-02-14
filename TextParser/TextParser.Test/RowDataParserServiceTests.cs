using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TextParser.Core.Enums;
using TextParser.Core.Interfaces;
using TextParser.Core.Services;
using Xunit;

namespace TextParser.Test {
    public class RowDataParserServiceTests{

        Mock<ICustomLogger> customLogger;

        public RowDataParserServiceTests() {
            customLogger = new Mock<ICustomLogger>();
        }

        //With we'd move all this a Test Data Generator
        [Theory]
        [InlineData("08.01.2010      Parking Space 81 Landmark     12.11.2009      EGL565725  ", "08.01.2010 ", "Parking Space 81 Landmark ", "12.11.2009 ", "EGL565725 ")]
        [InlineData("08.01.2010      Parking Space 81 Landmark     12.11.2009      EGL565725", "08.01.2010 ", "Parking Space 81 Landmark ", "12.11.2009 ", "EGL565725 ")]
        [InlineData("Parking Space 81 Landmark     12.11.2009      EGL565725  ", "", "Parking Space 81 Landmark ", "12.11.2009 ", "EGL565725 ")]
        [InlineData("08.01.2010      Parking Space 81 Landmark     12.11.2009      ", "08.01.2010 ", "Parking Space 81 Landmark ", "12.11.2009 ", "")]
        [InlineData("08.01.2010      Parking Space 81 Landmark           EGL565725", "08.01.2010 ", "Parking Space 81 Landmark ", "", "EGL565725 ")]
        [InlineData("08.01.2010                                 SY565725", "08.01.2010 ", "", "", "SY565725 ")]
        [InlineData("Telephone Exchange            17.02.1955                ", "", "Telephone Exchange ", "17.02.1955 ", "")]
        [InlineData("An Electricity Sub - Station    26.08.1952", "", "An Electricity Sub - Station ", "26.08.1952 ", "")]
        [InlineData("CANCELLED", "CANCELLED ", "", "", "")]
        [InlineData("17.12.2014      Second Floor, Rear Wing,      28.11.2014      Not        ", "17.12.2014 ", "Second Floor, Rear Wing, ", "28.11.2014 ", "Not ")]
        [InlineData("17.12.2014      Second Floor, Rear Wing,      28.11.2014      12343245        ", "17.12.2014 ", "Second Floor, Rear Wing, ", "28.11.2014 ", "12343245 ")]
        public async Task TestFirstRowParsing(string rowText, string expectRegistration, string expectPropDetails, string expectLeasesDate, string expectTitle) {
            var registrationStringBuilder = new StringBuilder();
            var propDescriptionStringBuilder = new StringBuilder();
            var dateOfLeaseStringBuilder = new StringBuilder();
            var lesseesStringBuilder = new StringBuilder();
            var noteStringBuilder = new StringBuilder();

            var dataParser = new RowDataParserService(customLogger.Object);
            await dataParser.ParseRowData(0, rowText, registrationStringBuilder, propDescriptionStringBuilder, dateOfLeaseStringBuilder, lesseesStringBuilder, noteStringBuilder, RowState.Standard);
            Assert.Equal(expectRegistration, registrationStringBuilder.ToString());
            Assert.Equal(expectPropDetails, propDescriptionStringBuilder.ToString());
            Assert.Equal(expectLeasesDate, dateOfLeaseStringBuilder.ToString());
            Assert.Equal(expectTitle, lesseesStringBuilder.ToString());
        }


        [Theory]
        [InlineData("1               Ground floor flat)            999 years from             NOT", 1, "1 ", "Ground floor flat) ", "999 years from ", "", "NOT ", RowState.Standard)]
        [InlineData("1               Ground floor flat)            999 years from             LEASED", 2, "1 ", "Ground floor flat) ", "999 years from ", "", "LEASED ", RowState.Standard)]
        [InlineData("1               Ground floor flat)            999 years from             ", 1, "1 ", "Ground floor flat) ", "999 years from ", "",  "", RowState.Standard)]
        [InlineData("from 1 April               ", 2, "", "", "from 1 April ", "", "", RowState.Standard)]
        [InlineData("NOTE: This IS A TEST", 4, "", "", "", "NOTE: This IS A TEST ", "", RowState.Standard)]
        [InlineData("this is a follow on without the identifier", 5, "", "", "", "this is a follow on without the identifier ", "", RowState.NoteElement)]
        [InlineData("Street                        10 years from              ", 1, "", "Street ", "10 years from ", "", "", RowState.Standard)]
        [InlineData("Street                        10 years from              ", 1, "", "Street ", "10 years from ", "", "", RowState.TwoElementsPropertyAndDate)]
        [InlineData("Edged and                                     25 years from              ", 1, "Edged and ", "", "25 years from ", "", "", RowState.Standard)]
        [InlineData("Edged and                                     25 years from              ", 1, "Edged and ", "", "25 years from ", "", "", RowState.TwoElementsRegistrationAndDate)]   
        [InlineData("of)                                           including                  ", 3, "of) ", "", "including ", "", "", RowState.Standard)]
        [InlineData("of)                                           including                  ", 3, "of) ", "", "including ", "", "", RowState.TwoElementsRegistrationAndDate)]
        [InlineData("50 years (less             ", 3, "", "", "50 years (less ", "", "", RowState.Standard)]
        [InlineData("50 years (less             ", 3, "", "", "50 years (less ", "", "", RowState.TwoElementsRegistrationAndDate)]
        [InlineData("50 years (less             ", 3, "", "", "50 years (less ", "", "", RowState.TwoElementsPropertyAndDate)]
        [InlineData("from 1 April               ", 2, "", "", "from 1 April ", "", "", RowState.OneElementDateLease)]
        [InlineData("part of)                                            ", 3, "part of) ", "", "", "", "", RowState.TwoElementsRegistrationAndDate)]
        [InlineData("21.11.1984", 3, "", "", "21.11.1984 ", "", "", RowState.OneElementDateLease)]
        [InlineData("21.11.1984       ", 3, "", "", "21.11.1984 ", "", "", RowState.TwoElementsRegistrationAndDate)]
        [InlineData("21.11.1984       ", 3, "", "", "21.11.1984 ", "", "", RowState.TwoElementsPropertyAndDate)]
        public async Task TestIntermediateDataRowParsing(string rowText, int rowIndex, string expectRegistration,
                                                         string expectPropDetails, string expectLeasesDate,
                                                         string expectedNotes, string expectedLesees, RowState lastRowState) {
            var registrationStringBuilder = new StringBuilder();
            var propDescriptionStringBuilder = new StringBuilder();
            var dateOfLeaseStringBuilder = new StringBuilder();
            var lesseesStringBuilder = new StringBuilder();
            var noteStringBuilder = new StringBuilder();

            var dataParser = new RowDataParserService(customLogger.Object);
            await dataParser.ParseRowData(rowIndex, rowText, registrationStringBuilder, propDescriptionStringBuilder, dateOfLeaseStringBuilder, lesseesStringBuilder, noteStringBuilder, lastRowState);
            Assert.Equal(expectRegistration, registrationStringBuilder.ToString());
            Assert.Equal(expectPropDetails, propDescriptionStringBuilder.ToString());
            Assert.Equal(expectLeasesDate, dateOfLeaseStringBuilder.ToString());
            Assert.Equal(expectedNotes, noteStringBuilder.ToString());
            Assert.Equal(expectedLesees, lesseesStringBuilder.ToString());
        }


    }
}
