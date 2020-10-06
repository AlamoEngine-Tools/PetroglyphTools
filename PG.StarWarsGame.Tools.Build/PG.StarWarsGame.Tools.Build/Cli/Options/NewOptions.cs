using System;
using CommandLine;
using JetBrains.Annotations;
using PG.Commons.Cli.Options;

namespace PG.StarWarsGame.Tools.Build.Cli.Options
{
    /// <summary>
    /// The command reflected by this options class generates the scaffolding for a new mod with a default (non-functional) eaw-build configuration.
    /// <example>new [mod-name] [path] -e|--eaw [eaw-path] -f|--foc [foc-path]</example>
    /// </summary>
    [Verb("new", HelpText = "Initialises a new EaW mod project.")]
    public class NewOptions : IOptions
    {   
        [NotNull] private readonly string m_modName;
        [NotNull] private readonly string m_path;
        [NotNull] private readonly string m_eawPath;
        [NotNull] private readonly string m_focPath;
        private readonly bool m_createGitRepository;
        private readonly bool m_verbose;

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="modName"></param>
        /// <param name="path"></param>
        /// <param name="eawPath"></param>
        /// <param name="focPath"></param>
        /// <param name="createGitRepository"></param>
        /// <param name="verbose"></param>
        public NewOptions([NotNull] string modName, [NotNull] string path, [NotNull] string eawPath,
            [NotNull] string focPath, bool createGitRepository, bool verbose)
        {
            m_modName = modName ?? throw new ArgumentNullException(nameof(modName));
            m_path = path ?? throw new ArgumentNullException(nameof(path));
            m_eawPath = eawPath ?? throw new ArgumentNullException(nameof(eawPath));
            m_focPath = focPath ?? throw new ArgumentNullException(nameof(focPath));
            m_createGitRepository = createGitRepository;
            m_verbose = verbose;
        }

        /// <summary>
        /// The mod's name. Required first argument.
        /// </summary>
        [Value(0, Required = true, MetaName = "<MOD-NAME>", HelpText = "The mod's name.")]
        public string ModName => m_modName;

        /// <summary>
        /// The path to the directory to initialise the mod in. Required second argument.
        /// </summary>
        [Value(1, Required = true, MetaName = "<MOD-PATH>",
            HelpText = "The path to the directory to initialise the mod in.")]
        public string Path => m_path;

        /// <summary>
        /// The path to the Empire at War data directory.
        /// </summary>
        [Option('e', "eaw", Required = true, HelpText = "The path to the Empire at War data directory.")]
        public string EawPath => m_eawPath;

        /// <summary>
        /// The path to the Forces of Corruption data directory.
        /// </summary>
        [Option('f', "foc", Required = true, HelpText = "The path to the Forces of Corruption data directory.")]
        public string FocPath => m_focPath;

        /// <summary>
        /// Initialises a git repository.
        /// <remarks>Currently not implemented.</remarks>
        /// </summary>
        [Option('r', "repository", Required = false, Default = false, HelpText = "Initialises a git repository.",
            Hidden = true)] // [gruenwaldlu]: Hidden as it's currently an unimplemented feature.
        public bool CreateGitRepository => m_createGitRepository;
        
        /// <inheritdoc/>
        [Option('v', "verbose", Required = false, Default = false, HelpText = "If set, the logging is set to the considerably more verbose logging level INFO rather than the default WARNING level.")]
        public bool Verbose => m_verbose;
    }
}