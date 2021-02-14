using System.Collections.Generic;
using System.Threading.Tasks;

namespace TextParser.Core.Interfaces {
    public interface IEntryTextParserService<TInputType, TReturnType> {
        /// <summary>
        /// Parses Entry Text and maps it into the specified return type.
        /// </summary>
        /// <param name="rawTextCollection"></param>
        /// <returns></returns>
        public Task<IEnumerable<TReturnType>> ParseEntryTextCollection(IEnumerable<TInputType> rawTextCollection);

        public Task<TReturnType> ParseEntryText(TInputType item);
    }
}
