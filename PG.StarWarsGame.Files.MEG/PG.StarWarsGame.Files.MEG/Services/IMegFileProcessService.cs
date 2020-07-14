using System.Collections.Generic;
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
        /// Packs a list of files as *.MEG archive.
        /// </summary>
        /// <param name="megArchiveName">The desired name of the archive.</param>
        /// <param name="absoluteFilePaths">A list of files to be packed.</param>
        /// <param name="targetDirectory">The target directory to which the *.MEG archive will be written.</param>
        void PackFilesAsMegArchive(string megArchiveName, List<string> absoluteFilePaths, string targetDirectory);

        /// <summary>
        /// Unpacks a given *.MEG file into a given directory.
        /// The file structure within the *.MEG file will be preserved relative to the target directory.
        /// </summary>
        /// <param name="filePath">Path to the *.MEG file to unpack.</param>
        /// <param name="targetDirectory">Directory to unpack the files into.</param>
        void UnpackMegFile(string filePath, string targetDirectory);

        /// <summary>
        /// Same as <see cref="UnpackMegFile(string,string)"/>, but with a previously loaded
        /// <see cref="MegFileHolder"/> instead of a file path to a meg file.  
        /// </summary>
        /// <param name="holder">The previously loaded *.MEG file.</param>
        /// <param name="targetDirectory">Directory to unpack the files into.</param>
        void UnpackMegFile(MegFileHolder holder, string targetDirectory);
        
        /// <summary>
        /// Unpacks a single file from a given *.MEG file, provided the file is stored in the archive.
        /// </summary>
        /// <param name="holder">The previously loaded *.MEG file.</param>
        /// <param name="targetDirectory">Directory to unpack the files into.</param>
        /// <param name="fileName">The name of the file to unpack.</param>
        void UnpackMegFile(MegFileHolder holder, string targetDirectory, string fileName);
        
        /// <summary>
        /// Loads a *.MEG file's metadata into a <see cref="MegFileHolder"/>. This holder can be used for targeted unpacking
        /// of single files or checks for existence of a given file.. 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>The specified *.MEG file's metadata.</returns>
        MegFileHolder Load(string filePath);
    }
}