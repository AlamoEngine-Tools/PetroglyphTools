namespace PG.StarWarsGame.Tools.Build.Cli.Options
{
    /// <summary>
    /// Basic interface for all valid command-line-options and their shared elements.
    /// Each *Options class that inherits the interface acts as a new word for the command-line
    /// </summary>
    public interface IOptions
    {
        /// <summary>
        /// If set, the logging is set to the considerably more verbose logging level INFO rather than the default WARNING level.
        /// </summary>
        bool Verbose { get; }
    }
}