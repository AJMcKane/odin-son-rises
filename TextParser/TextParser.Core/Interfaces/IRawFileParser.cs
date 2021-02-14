using System.Collections.Generic;
using System.Threading.Tasks;
using TextParser.Core.Models.Transfer;

namespace TextParser.Core.Interfaces {
    public interface IRawFileParserService<TOutputType> {
        public Task<IEnumerable<TOutputType>> ParseFile(string filePath);
    }
}
