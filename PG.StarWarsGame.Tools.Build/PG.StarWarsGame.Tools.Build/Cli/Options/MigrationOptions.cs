using System;
using CommandLine;
using JetBrains.Annotations;

namespace PG.StarWarsGame.Tools.Build.Cli.Options
{
    /// <summary>
    /// The command reflected by this options class migrates a yvaw-build project to an eaw-build project.
    /// <example>migrate -m|--mod [old-config]</example>
    /// <example>migrate -t|--text [config-file]</example>
    /// </summary>
    [Verb("migrate", HelpText = "Migrate a yvaw-build project to an eaw-build project.")]
    public class MigrationOptions : IOptions
    {

        private readonly bool m_modProjectMigration;
        private readonly bool m_translationMigration;
        [NotNull] private readonly string m_configurationFilePath;
        private readonly bool m_verbose;
        
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="modProjectMigration"></param>
        /// <param name="translationMigration"></param>
        /// <param name="configurationFilePath"></param>
        /// <param name="verbose"></param>
        public MigrationOptions(bool modProjectMigration, bool translationMigration,
            [NotNull] string configurationFilePath, bool verbose)
        {
            m_modProjectMigration = modProjectMigration;
            m_translationMigration = translationMigration;
            m_configurationFilePath =
                configurationFilePath ?? throw new ArgumentNullException(nameof(configurationFilePath));
            m_verbose = verbose;
        }

        /// <summary>
        /// Migrate a yvaw-build project to an eaw-build project.
        /// --mod and --text are mutually exclusive options.
        /// </summary>
        [Option('m', "mod", Required = true, Default = false, SetName = nameof(ModProjectMigration),
            HelpText =
                "Migrate a yvaw-build project to an eaw-build project. --mod and --text are mutually exclusive options.")]
        public bool ModProjectMigration => m_modProjectMigration;

        /// <summary>
        /// "Migrate a translationmanifest.xml to a localisation project.
        /// --mod and --text are mutually exclusive options.
        /// </summary>
        [Option('t', "text", Required = true, Default = false, SetName = nameof(TranslationMigration),
            HelpText =
                "Migrate a translationmanifest.xml to a localisation project. --mod and --text are mutually exclusive options.")]
        public bool TranslationMigration => m_translationMigration;

        /// <summary>
        /// The path to the configuration file to migrate. If the path contains spaces, wrap it in quotation marks (").
        /// </summary>
        [Value(0, Required = true, MetaName = "<CONFIGURATION-FILE>",
            HelpText =
                "The path to the configuration file to migrate. If the path contains spaces, wrap it in quotation marks (\").")]
        public string ConfigurationFilePath => m_configurationFilePath;
        
        /// <inheritdoc/>
        [Option('v', "verbose", Required = false, Default = false, HelpText = "If set, the logging is set to the considerably more verbose logging level INFO rather than the default WARNING level.")]
        public bool Verbose => m_verbose;
    }
}