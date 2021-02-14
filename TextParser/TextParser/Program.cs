using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TextParser.Core;
using TextParser.Core.Interfaces;
using TextParser.Core.Models.Transfer;
using TextParser.Core.Services;
using TextParser.Core.Utilities;
using TextParser.DAL.Services;
using TextParser.DAL.Models;
using TextParser.DAL.Interfaces;

namespace TextParser {
    public class Program {
        public static async Task Main(string[] args) {
            Console.WriteLine("Beginning Application Setup");
            using IHost host = CreateHostBuilder(args).Build();
            var serviceProvider = host.Services.CreateScope().ServiceProvider;
            var logger = serviceProvider.GetRequiredService<ICustomLogger>();
            try {
                //TODO: Before calling GetRequiredService we would take in an arg like FileName, inspect our extension and modify our service provider accordingly
                //We could also flip the app to iterate a directory, instantiate different instances with of ILeaseTextParser with different IRawFileParsers per fileType.
                var textParsingService = serviceProvider.GetRequiredService<ILeaseTextParser>();
                var success = await textParsingService.ParseLeaseText("./InputFiles/InputJson.json");
                if (success) {
                    logger.Log(LogLevel.Information, "Parsing Complete, please check output database for results.");
                } else {
                    logger.Log(LogLevel.Error, "Unable to parse file, please check logs for more information");
                }
            }
            catch(Exception ex) {
                logger.LogException(ex, "Application crashed");
            }

            logger.Log(LogLevel.Information, "Press Enter Key to close...");
            Console.ReadLine();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) {
            return Host.CreateDefaultBuilder(args).ConfigureServices(SetupDependencies);
        }

        private static void SetupDependencies(HostBuilderContext context, IServiceCollection services) {
            services.AddScoped<ILeaseTextParser, LeaseNoticeScheduleTextParser>()
                .AddScoped<IEntryTextParserService<RawEntryTextOutput, LeaseNoticeSchedule>, EntryTextParserService>()
                .AddScoped<IRawFileParserService<RawEntryTextOutput>, JsonRawParserService>()
                .AddScoped<ITextParserDataService<LeaseNoticeSchedule>,TextParserDataService>()
                .AddScoped<IRegexHandlerService, RegexHandlerService>()
                .AddSingleton<IConsoleWriter, ConsoleWriter>()
                .AddSingleton<ICustomLogger, ConsoleLogger>((srv) => new ConsoleLogger(LogLevel.Debug, srv.GetRequiredService<IConsoleWriter>()));
        }
    }
}
