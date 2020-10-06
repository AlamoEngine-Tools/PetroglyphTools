using System;
using CommandLine;
using JetBrains.Annotations;
using PG.Commons.Cli.Options;

namespace PG.StarWarsGame.Tools.Build.Cli.Options
{
    /// <summary>
    /// The command reflected by this options class releases a mod: Builds the mod, cooks the mod, and copies the built mod to a target directory.
    /// Optionally it also increments the version number. A config file needs to be supplied as argument.
    /// <example>release [config-file] [target-directory] (--major|--minor|--patch)</example>
    /// </summary>
    [Verb("release", HelpText = "Release a mod.")]
    public class ReleaseOptions : IOptions
    {
        [NotNull] private readonly string m_configurationFile;
        private readonly bool m_increaseMajor;
        private readonly bool m_increaseMinor;
        private readonly bool m_increasePatch;
        private readonly bool m_verbose;

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="configurationFile"></param>
        /// <param name="increaseMajor"></param>
        /// <param name="increaseMinor"></param>
        /// <param name="increasePatch"></param>
        /// <param name="verbose"></param>
        public ReleaseOptions([NotNull] string configurationFile, bool increaseMajor, bool increaseMinor,
            bool increasePatch, bool verbose)
        {
            m_configurationFile = configurationFile ?? throw new ArgumentNullException(nameof(configurationFile));
            m_increaseMajor = increaseMajor;
            m_increaseMinor = increaseMinor;
            m_increasePatch = increasePatch;
            m_verbose = verbose;
        }

        /// <summary>
        /// The path to the mod's configuration file.
        /// </summary>
        [Value(0, Required = false, MetaName = "<CONFIGURATION-FILE>",
            HelpText = "The path to the mod's configuration file.", Default = Defaults.CONFIGURATION_FILE_NAME)]
        public string ConfigurationFile => m_configurationFile;

        /// <summary>
        /// Create a new major release, e.g. 1.0.0, 3.0.0, ...
        /// </summary>
        [Option("major", SetName = "version-major", HelpText = "Create a new major release, e.g. 1.0.0, 3.0.0, ...",
            Default = false, Required = false)]
        public bool IncreaseMajor => m_increaseMajor;

        /// <summary>
        /// Create a new minor release, e.g. 1.1.0, 1.2.0, ...
        /// </summary>
        [Option("minor", SetName = "version-minor", HelpText = "Create a new minor release, e.g. 1.1.0, 1.2.0, ...",
            Default = false, Required = false)]
        public bool IncreaseMinor => m_increaseMinor;

        /// <summary>
        /// Create a new patch release, e.g. 1.1.1, 1.1.2, ...
        /// </summary>
        [Option("patch", SetName = "version-patch", HelpText = "Create a new patch release, e.g. 1.1.1, 1.1.2, ...",
            Default = false, Required = false)]
        public bool IncreasePatch => m_increasePatch;

        ///<inheritdoc/>
        public bool Verbose => m_verbose;
    }
}