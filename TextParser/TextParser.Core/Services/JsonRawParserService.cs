using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TextParser.Core.Interfaces;
using TextParser.Core.Models.Transfer;

namespace TextParser.Core.Services {
    public class JsonRawParserService : IRawFileParserService<RawEntryTextOutput> {
        public async Task<IEnumerable<RawEntryTextOutput>> ParseFile(string filePath) {
            IEnumerable<RawEntryTextOutput> entryTextCollection;
            using (StreamReader file = File.OpenText(filePath))
            using (JsonTextReader reader = new JsonTextReader(file)) {
                JArray inputArray = (JArray) await JToken.ReadFromAsync(reader);
                var leaseSchedule = JsonConvert.DeserializeObject<IEnumerable<LeaseScheduleWrapper>>(inputArray.ToString());
                //We'd want to parse the response in batches, to ensure we don't have memory problems on smaller sized hosts.
                entryTextCollection = leaseSchedule.SelectMany(p => p.LeaseSchedule.ScheduleEntry.Select(d => d.EntryText))
                                                   .Select(p => new RawEntryTextOutput { EntryText = p })
                                                   .ToArray();
            }

            return entryTextCollection;
        }

        private class LeaseScheduleWrapper {
            public LeaseSchedule LeaseSchedule { get; set; }
        }
    }
}
