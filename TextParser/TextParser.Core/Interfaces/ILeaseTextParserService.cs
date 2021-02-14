using System.Threading.Tasks;

namespace TextParser.Core.Interfaces {
    public interface ILeaseTextParserService {
        /// <summary>
        /// Parses Input File and stores to Database
        /// </summary>
        /// <param name="filePath">The Target File To Parse</param>
        /// <returns>Boolean to denote success</returns>
        public Task<bool> ParseLeaseText(string filePath);
    }
}
