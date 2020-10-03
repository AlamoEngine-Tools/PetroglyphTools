using CommandLine;

namespace PG.StarWarsGame.Files.MEG.CLI
{
    [Verb("unpack", false, HelpText = "Unpacks a given .meg file to a specified location.")]
    internal class UnpackOptions : OptionsBase
    {
        [Option('i', "input", Required = true, HelpText = "The file name of the to-be unpacked .meg file." +
                                                          "The parameter also may be an relative or absolute path.")]
        public string Input { get; set; }

        [Option('o', "output", Required = false, HelpText = "Location where files will get extracted to.")]
        public string Output { get; set; }
    }
}