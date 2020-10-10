using CommandLine;

namespace PG.StarWarsGame.Files.MEG.CLI
{
    internal abstract class OptionsBase : IOptions
    {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }
    }
}