using System;
using CommandLine;
using JetBrains.Annotations;
using PG.Commons.Cli.Options;

namespace PG.StarWarsGame.Tools.Build.Cli.Options
{
    /// <summary>
    /// The command reflected by this options class builds (generates the `*.DAT` files and commandbar files) a mod.
    /// The build operation is defined by the config file supplied as argument, it defaults to <see cref="Defaults.CONFIGURATION_FILE_NAME"/>
    /// <example>build [config-file]</example>
    /// </summary>
    [Verb("build", HelpText =
        "Builds (generates the `*.DAT` files and commandbar files) a mod. The build operation is defined by the config file supplied as argument.")]
    public class BuildOptions : IOptions
    {
        [NotNull] private readonly string m_configurationFile;
        private readonly bool m_verbose;

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="configurationFile"></param>
        /// <param name="verbose"></param>
        public BuildOptions([NotNull] string configurationFile, bool verbose)
        {
            m_configurationFile = configurationFile ?? throw new ArgumentNullException(nameof(configurationFile));
            m_verbose = verbose;
        }

        /// <summary>
        /// The path to the mod's configuration file.
        /// </summary>
        [Value(0, Required = false, MetaName = "<CONFIGURATION-FILE>",
            HelpText = "The path to the mod's configuration file.", Default = Defaults.CONFIGURATION_FILE_NAME)]
        public string ConfigurationFile => m_configurationFile;
        /// <inheritdoc />
        public bool Verbose => m_verbose;
    }
}