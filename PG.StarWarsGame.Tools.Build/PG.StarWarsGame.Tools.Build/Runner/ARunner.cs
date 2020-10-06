using System;
using System.IO.Abstractions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using PG.Commons.Cli.Options;
using PG.Commons.Environment;
using PG.StarWarsGame.Tools.Build.Cli.Options;

namespace PG.StarWarsGame.Tools.Build.Runner
{
    internal abstract class ARunner : IRunner
    {
        [NotNull] private readonly IFileSystem m_fileSystem;
        private readonly ILogger m_logger;
        [NotNull] private readonly IOptions m_options;

        protected ARunner(IFileSystem fileSystem, [NotNull] IOptions options, ILogger logger = null)
        {
            m_fileSystem = fileSystem ?? new FileSystem();
            m_options = options ?? throw new ArgumentNullException(nameof(options));
            m_logger = logger;
        }

        public abstract ExitCode Run();
    }
}