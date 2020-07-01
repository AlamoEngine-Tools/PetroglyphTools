using PG.StarWarsGame.Files.MEG.Holder;

namespace PG.StarWarsGame.Files.MEG.Services
{
    /// <summary>
    /// MEF service interface definition for handling sorted *.DAT files.
    /// A default implementation is provided in <see cref=""/>.
    /// When requesting the default implementation via an IoC Container or registering via injection, you may pass
    /// a file system as argument implementing <see cref="System.IO.Abstractions.IFileSystem"/> and a logger
    /// implementing <see cref="Microsoft.Extensions.Logging.ILogger"/>
    /// </summary>
    public interface IMegFileProcessService
    {
        /// <summary>
        /// Unpacks a previously loaded *.MEG file.
        /// </summary>
        /// <param name="holder">The previously loaded <see cref="MegFileHolder"/></param>
        /// <param name="targetDirectory">The optional target directory. If not provided, null or empty,
        /// the *.MEG file content will be unpacked into the same directory the *.MEG file resides in.</param>
        void Unpack(MegFileHolder holder, string targetDirectory = null);
        /// <summary>
        /// Packs the files specified in a given <see cref="MegFileHolder"/> into a *.MEG file.
        /// The<see cref="MegFileHolder.FilePath"/> will act as the working directory.
        /// The <see cref="MegFileHolder.FileName"/> will be name of the produced output file.
        /// All files contained in <see cref="MegFileHolder.Content"/> will be packed .
        /// </summary>
        /// <param name="holder">The previously loaded/constructed <see cref="MegFileHolder"/></param>
        void Pack(MegFileHolder holder);
        /// <summary>
        /// Loads the contained metadata of a *.MEG file into a <see cref="MegFileHolder"/>.
        /// </summary>
        /// <param name="filePath">Path tot the *.MEG file to load.</param>
        /// <returns></returns>
        MegFileHolder Load(string filePath);
    }
}