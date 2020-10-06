using System;
using CommandLine;
using JetBrains.Annotations;
using PG.Commons.Cli.Options;

namespace PG.StarWarsGame.Tools.Build.Cli.Options
{
    /// <summary>
    /// The command reflected by this options class cooks (packages files into `*.MEG` files and moves files to a target directory) a mod.
    /// The cook operation is defined by the config file supplied as argument, it defaults to <see cref="Defaults.CONFIGURATION_FILE_NAME"/>
    /// <example>cook [config-file]</example>
    /// </summary>
    [Verb("cook", HelpText =
        "Cooks (packages files into `*.MEG` files and moves files to a target directory) a mod. The cook operation is defined by the config file supplied as argument.")]
    public class CookOptions : IOptions
    {
        [NotNull] private readonly string m_configurationFile;
        private readonly bool m_verbose;

        public CookOptions([NotNull] string configurationFile, bool verbose)
        {
            m_configurationFile = configurationFile ?? throw new ArgumentNullException(nameof(configurationFile));
            m_verbose = verbose;
        }

        [Value(0, Required = false, MetaName = "<CONFIGURATION-FILE>",
            HelpText = "The path to the mod's configuration file.", Default = Defaults.CONFIGURATION_FILE_NAME)]
        public string ConfigurationFile => m_configurationFile;

        /// <inheritdoc/>
        public bool Verbose => m_verbose;
    }
}