using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;

namespace PG.StarWarsGame.Files.MEG.CLI
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<UnpackOptions, PackOptions>(args)
                .WithParsed<UnpackOptions>(ExecInternal)
                .WithParsed<PackOptions>(ExecInternal)
                .WithNotParsed(HandleParseErrorsInternal);
        }

        private static void ExecInternal(IOptions options)
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection, options.Verbose);

            IApplicationTask task = options switch
            {
                PackOptions packOptions => new Packer(packOptions),
                UnpackOptions unpackOptions => new Unpacker(unpackOptions),
                _ => throw new ArgumentOutOfRangeException(nameof(options))
            };
            task.Run();
        }

        private static void ConfigureServices(IServiceCollection serviceCollection, bool verbose)
        {
            serviceCollection.AddLogging(config =>
                {
#if DEBUG
                    config.AddDebug();
#endif
                    config.AddConsole();
                })
                .Configure<LoggerFilterOptions>(options =>
                {
#if DEBUG
                    options.AddFilter<DebugLoggerProvider>(null, LogLevel.Trace);
#endif
                    options.AddFilter<ConsoleLoggerProvider>(null, verbose ? LogLevel.Trace : LogLevel.Warning);
                });
        }

        private static void HandleParseErrorsInternal(IEnumerable<Error> errs)
        {
            IEnumerable<Error> errors = errs as Error[] ?? errs.ToArray();
            if (errors.OfType<HelpVerbRequestedError>().Any() || errors.OfType<HelpRequestedError>().Any())
            {
                Environment.ExitCode = 0;
                return;
            }
            Environment.ExitCode = 64;
        }
    }
}
