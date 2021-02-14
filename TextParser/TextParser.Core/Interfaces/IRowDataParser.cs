using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TextParser.Core.Enums;

namespace TextParser.Core.Interfaces {
    public interface IRowDataParser {
        public Task<RowState> ParseRowData(int rowIndex, string rowText, StringBuilder registrationStringBuilder, StringBuilder propDescriptionStringBuilder,
            StringBuilder dateOfLeaseStringBuilder, StringBuilder lesseesStringBuilder, StringBuilder noteStringBuilder, RowState lastRowState);
    }
}
