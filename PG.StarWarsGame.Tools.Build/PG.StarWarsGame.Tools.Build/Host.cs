using System.Collections.Generic;
using System.Linq;
using CommandLine;
using PG.Commons.Cli.Options;
using PG.Commons.Environment;
using PG.StarWarsGame.Tools.Build.Cli.Options;

namespace PG.StarWarsGame.Tools.Build
{
    internal static class Host
    {
        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<NewOptions, MigrationOptions, BuildOptions, CookOptions, ReleaseOptions>(args)
                .WithParsed<NewOptions>(ExecInternal)
                .WithParsed<MigrationOptions>(ExecInternal)
                .WithParsed<BuildOptions>(ExecInternal)
                .WithParsed<CookOptions>(ExecInternal)
                .WithParsed<ReleaseOptions>(ExecInternal)
                .WithNotParsed(HandleParseErrorsInternal);
        }

        private static void ExecInternal(IOptions opts)
        {
        }

        private static void HandleParseErrorsInternal(IEnumerable<Error> errs)
        {
            IEnumerable<Error> errors = errs as Error[] ?? errs.ToArray();
            if (errors.OfType<HelpVerbRequestedError>().Any() || errors.OfType<HelpRequestedError>().Any())
            {
                System.Environment.ExitCode = (int) ExitCode.Success;
                return;
            }

            System.Environment.ExitCode = (int) ExitCode.ExUsage;
        }
    }
}