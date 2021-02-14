using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TextParser.Core.Enums;
using TextParser.Core.Interfaces;

namespace TextParser.Core.Services {
    public class RowDataParserService : IRowDataParserService {
        private const string NOTE_PREFIX = "Note";
        private const int DOUBLE_COL_SPACE = 28;
        private ICustomLogger logger;
        public RowDataParserService(ICustomLogger logger) {
            this.logger = logger;
        }

        public async Task<RowState> ParseRowData(int rowIndex, string rowText, StringBuilder registrationStringBuilder, StringBuilder propDescriptionStringBuilder,
            StringBuilder dateOfLeaseStringBuilder, StringBuilder lesseesStringBuilder, StringBuilder noteStringBuilder, RowState lastRowState) {
            try {
                var columnSpaceMatches = Regex.Matches(rowText, @"\s\s+");
                var columnText = rowText.Split("   ", StringSplitOptions.RemoveEmptyEntries)
                                         .Where(p => !string.IsNullOrWhiteSpace(p))
                                         .ToArray();
                var rowTextEndsWithSpacing = Regex.IsMatch(rowText, @"\s\s+$");

                if (rowIndex == 0) {
                    ParseFirstRow(columnText, registrationStringBuilder, propDescriptionStringBuilder,
                     dateOfLeaseStringBuilder, lesseesStringBuilder);
                    return RowState.Standard;
                } else {
                    return ParseIntermediateRow(columnSpaceMatches, columnText, registrationStringBuilder, propDescriptionStringBuilder,
                     dateOfLeaseStringBuilder,lesseesStringBuilder, noteStringBuilder, lastRowState, rowTextEndsWithSpacing);
                }
            } catch (Exception ex) {
                logger.LogException(ex, "Failure whilst parsing Entry Text Items, potential unsupported string format");
                throw ex;
            }
        }

        private RowState ParseIntermediateRow(MatchCollection columnSpaceMatches, string[] columnText, StringBuilder registrationStringBuilder, StringBuilder propDescriptionStringBuilder, 
            StringBuilder dateOfLeaseStringBuilder, StringBuilder lesseesStringBuilder,  StringBuilder noteStringBuilder, RowState lastRowState, bool rowTextEndsInSpacing) {
            var firstColumnSpaceMatch = columnSpaceMatches.FirstOrDefault();
            var anySpacing = firstColumnSpaceMatch != null;
            var doubleColSpacing =firstColumnSpaceMatch?.Length >= DOUBLE_COL_SPACE;
            var thisRowState = RowState.Standard;
            switch (columnText.Length) {
                case 1:
                    thisRowState = ParseOneColumn(columnText, registrationStringBuilder, dateOfLeaseStringBuilder, 
                        noteStringBuilder, lastRowState, rowTextEndsInSpacing, doubleColSpacing, anySpacing);
                    //var text = columnText[0];
                    //if (text.Contains(NOTE_PREFIX, StringComparison.InvariantCultureIgnoreCase)) {
                    //    TrimAndAppendTrailingSpace(noteStringBuilder, text);
                    //    thisRowState = RowState.NoteElement;
                    //    return thisRowState;
                    //}

                    //switch (lastRowState) {
                    //    case RowState.NoteElement:
                    //        TrimAndAppendTrailingSpace(noteStringBuilder, text);
                    //        thisRowState = lastRowState;
                    //        break;
                    //    case RowState.OneElementDateLease:
                    //        TrimAndAppendTrailingSpace(dateOfLeaseStringBuilder, text);
                    //        thisRowState = lastRowState;
                    //        break;

                    //    case RowState.OneElementRegistration:
                    //        TrimAndAppendTrailingSpace(registrationStringBuilder, text);
                    //        thisRowState = lastRowState;
                    //        break;
                    //    case RowState.Standard:
                    //        if (rowTextEndsInSpacing || TextIsDate(text)) {
                    //            TrimAndAppendTrailingSpace(dateOfLeaseStringBuilder, text);
                    //            thisRowState = RowState.OneElementDateLease;
                    //        } else {
                    //            TrimAndAppendTrailingSpace(registrationStringBuilder, text);
                    //            thisRowState = RowState.OneElementRegistration;
                    //        }
                    //        break;
                    //    case RowState.TwoElementsRegistrationAndDate:
                    //        if(doubleColSpacing) {
                    //            TrimAndAppendTrailingSpace(registrationStringBuilder, text);
                    //            thisRowState = RowState.OneElementRegistration;
                    //        } else {
                    //            TrimAndAppendTrailingSpace(dateOfLeaseStringBuilder, text);
                    //            thisRowState = RowState.OneElementRegistration;
                    //        }
                    //        break;
                    //    case RowState.TwoElementsPropertyAndDate:
                    //        TrimAndAppendTrailingSpace(dateOfLeaseStringBuilder, text);
                    //        thisRowState = RowState.OneElementDateLease;
                    //        break;
                    //}
                    break;
                case 2:
                    thisRowState = ParseTwoColumns(columnText, registrationStringBuilder, propDescriptionStringBuilder, dateOfLeaseStringBuilder, lastRowState, doubleColSpacing);
                    //We need to check our spacing here to determine which two coloumns we have                        
                    //if(doubleColSpacing || lastRowState == RowState.TwoElementsRegistrationAndDate) {
                    //    TrimAndAppendTrailingSpace(registrationStringBuilder, columnText[0]);
                    //    TrimAndAppendTrailingSpace(dateOfLeaseStringBuilder, columnText[1]);
                    //    thisRowState = RowState.TwoElementsRegistrationAndDate;
                    //}
                    //else {
                    //    TrimAndAppendTrailingSpace(propDescriptionStringBuilder, columnText[0]);
                    //    TrimAndAppendTrailingSpace(dateOfLeaseStringBuilder, columnText[1]);
                    //    thisRowState = RowState.TwoElementsPropertyAndDate;
                    //}
                    break;
                case 3:
                //    TrimAndAppendTrailingSpace(registrationStringBuilder, columnText[0]);
                //    TrimAndAppendTrailingSpace(propDescriptionStringBuilder, columnText[1]);
                //    TrimAndAppendTrailingSpace(dateOfLeaseStringBuilder, columnText[2]);
                    thisRowState = ParseThreeColumns(columnText, registrationStringBuilder, propDescriptionStringBuilder, dateOfLeaseStringBuilder);
                    break;
                case 4:
                    thisRowState = ParseFourColumns(columnText, registrationStringBuilder, propDescriptionStringBuilder, dateOfLeaseStringBuilder, lesseesStringBuilder);
                    break;
                default:
                    break;
            }

            return thisRowState;
        }

        private void ParseFirstRow(string[] columnText, StringBuilder registrationStringBuilder, StringBuilder propDescriptionStringBuilder,
            StringBuilder dateOfLeaseStringBuilder, StringBuilder lesseesStringBuilder) {
            switch (columnText.Length) {
                case 1:
                    if (TextIsDate(columnText[0])) {
                        TrimAndAppendTrailingSpace(registrationStringBuilder, columnText[0]);
                    }

                    if (TextIsTitle(columnText[0])) {
                        TrimAndAppendTrailingSpace(lesseesStringBuilder, columnText[0]);
                    } else {
                        TrimAndAppendTrailingSpace(registrationStringBuilder, columnText[0]);
                    }
                    break;
                case 2:
                    if (TextIsDate(columnText[0])) {
                        TrimAndAppendTrailingSpace(registrationStringBuilder, columnText[0]);
                    } else {
                        TrimAndAppendTrailingSpace(propDescriptionStringBuilder, columnText[0]);
                    }

                    if (TextIsDate(columnText[1])) {
                        TrimAndAppendTrailingSpace(dateOfLeaseStringBuilder, columnText[1]);
                    } else if (TextIsTitle(columnText[1])) {
                        TrimAndAppendTrailingSpace(lesseesStringBuilder, columnText[1]);
                    } else {
                        TrimAndAppendTrailingSpace(propDescriptionStringBuilder, columnText[1]);
                    }
                    break;
                case 3:
                    if (TextIsDate(columnText[0])) {
                        TrimAndAppendTrailingSpace(registrationStringBuilder, columnText[0]);
                        TrimAndAppendTrailingSpace(propDescriptionStringBuilder, columnText[1]);
                        if (TextIsTitle(columnText[2])) {
                            TrimAndAppendTrailingSpace(lesseesStringBuilder, columnText[2]);
                        } else {
                            TrimAndAppendTrailingSpace(dateOfLeaseStringBuilder, columnText[2]);
                        }
                    } else {
                        TrimAndAppendTrailingSpace(propDescriptionStringBuilder, columnText[0]);
                        TrimAndAppendTrailingSpace(dateOfLeaseStringBuilder, columnText[1]);
                        TrimAndAppendTrailingSpace(lesseesStringBuilder, columnText[2]);
                    }
                    break;
                case 4:
                    //TrimAndAppendTrailingSpace(registrationStringBuilder, columnText[0]);
                    //TrimAndAppendTrailingSpace(propDescriptionStringBuilder, columnText[1]);
                    //TrimAndAppendTrailingSpace(dateOfLeaseStringBuilder, columnText[2]);
                    //TrimAndAppendTrailingSpace(lesseesStringBuilder, columnText[3]);
                    ParseFourColumns(columnText, registrationStringBuilder, propDescriptionStringBuilder, dateOfLeaseStringBuilder, lesseesStringBuilder);
                    break;
                default:
                    break;

            }
        }

        private RowState ParseOneColumn(string[] columnText, StringBuilder registrationStringBuilder,
            StringBuilder dateOfLeaseStringBuilder, StringBuilder noteStringBuilder, 
            RowState lastRowState, bool rowTextEndsInSpacing, 
            bool doubleColSpacing, bool anySpacing) {
            var text = columnText[0];
            var thisRowState = RowState.Standard;
            if (text.Contains(NOTE_PREFIX, StringComparison.InvariantCultureIgnoreCase)) {
                TrimAndAppendTrailingSpace(noteStringBuilder, text);
                return RowState.NoteElement;
            }

            switch (lastRowState) {
                case RowState.NoteElement:
                    TrimAndAppendTrailingSpace(noteStringBuilder, text);
                    thisRowState = lastRowState;
                    break;
                case RowState.OneElementDateLease:
                    TrimAndAppendTrailingSpace(dateOfLeaseStringBuilder, text);
                    thisRowState = lastRowState;
                    break;

                case RowState.OneElementRegistration:
                    TrimAndAppendTrailingSpace(registrationStringBuilder, text);
                    thisRowState = lastRowState;
                    break;
                case RowState.Standard:
                    if (rowTextEndsInSpacing || TextIsDate(text)) {
                        TrimAndAppendTrailingSpace(dateOfLeaseStringBuilder, text);
                        thisRowState = RowState.OneElementDateLease;
                    } else {
                        TrimAndAppendTrailingSpace(registrationStringBuilder, text);
                        thisRowState = RowState.OneElementRegistration;
                    }
                    break;
                case RowState.TwoElementsRegistrationAndDate:
                    if (doubleColSpacing || !anySpacing) {
                        TrimAndAppendTrailingSpace(registrationStringBuilder, text);
                        thisRowState = RowState.OneElementRegistration;
                    } else {
                        TrimAndAppendTrailingSpace(dateOfLeaseStringBuilder, text);
                        thisRowState = RowState.OneElementRegistration;
                    }
                    break;
                case RowState.TwoElementsPropertyAndDate:
                    TrimAndAppendTrailingSpace(dateOfLeaseStringBuilder, text);
                    thisRowState = RowState.OneElementDateLease;
                    break;
            }

            return thisRowState;
        }

        private RowState ParseTwoColumns(string[] columnText, StringBuilder registrationStringBuilder, StringBuilder propDescriptionStringBuilder,
            StringBuilder dateOfLeaseStringBuilder, RowState lastRowState, bool doubleColSpacing) {
            var thisRowState = RowState.Standard;
            if (doubleColSpacing || lastRowState == RowState.TwoElementsRegistrationAndDate) {
                TrimAndAppendTrailingSpace(registrationStringBuilder, columnText[0]);
                TrimAndAppendTrailingSpace(dateOfLeaseStringBuilder, columnText[1]);
                thisRowState = RowState.TwoElementsRegistrationAndDate;
            } else {
                TrimAndAppendTrailingSpace(propDescriptionStringBuilder, columnText[0]);
                TrimAndAppendTrailingSpace(dateOfLeaseStringBuilder, columnText[1]);
                thisRowState = RowState.TwoElementsPropertyAndDate;
            }

            return thisRowState;
        }

        private RowState ParseThreeColumns(string[] columnText, StringBuilder registrationStringBuilder, StringBuilder propDescriptionStringBuilder,
            StringBuilder dateOfLeaseStringBuilder) {
            TrimAndAppendTrailingSpace(registrationStringBuilder, columnText[0]);
            TrimAndAppendTrailingSpace(propDescriptionStringBuilder, columnText[1]);
            TrimAndAppendTrailingSpace(dateOfLeaseStringBuilder, columnText[2]);
            return RowState.Standard;
        }
        
        private RowState ParseFourColumns(string[] columnText, StringBuilder registrationStringBuilder, StringBuilder propDescriptionStringBuilder,
            StringBuilder dateOfLeaseStringBuilder, StringBuilder lesseesStringBuilder) {
            TrimAndAppendTrailingSpace(registrationStringBuilder, columnText[0]);
            TrimAndAppendTrailingSpace(propDescriptionStringBuilder, columnText[1]);
            TrimAndAppendTrailingSpace(dateOfLeaseStringBuilder, columnText[2]);
            TrimAndAppendTrailingSpace(lesseesStringBuilder, columnText[3]);
            return RowState.Standard;
        }

        private bool TextIsDate(string textToCheck) {
            DateTime parsedDate;
            return DateTime.TryParse(textToCheck, out parsedDate);
        }

        private bool TextIsTitle(string textToCheck) {
            bool isTitle = Regex.IsMatch(textToCheck, @"[A-Z]+(?=\d{2})");
            if(!isTitle) {
                //Check if we have an int for numeric titles
                long titleInt;
                isTitle = long.TryParse(textToCheck, out titleInt);
            }
            return isTitle;
        }

        private void TrimAndAppendTrailingSpace(StringBuilder stringBuilder, string stringToAppend) {
            stringBuilder.Append(stringToAppend.Trim()).Append(" ");
        }
    }
}
