using System.Collections.Generic;
using CommandLine;

namespace PG.StarWarsGame.Files.MEG.CLI
{
    [Verb("pack", false, HelpText = "Packs given input files into a .meg archive and stores it on disk.")]
    internal class PackOptions : OptionsBase
    {
        [Option('i', "inputFiles", Required = false, HelpText = "A list of file locations, which shall get packed.")]
        public IEnumerable<string> InputFiles { get; set; }

        [Option('o', "output", Required = true, HelpText = "The name of the .meg which will get created." + 
                                                           "The parameter also may be an relative or absolute path.")]
        public string Output { get; set; }
    }
}
