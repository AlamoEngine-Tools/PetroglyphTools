// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using JetBrains.Annotations;
using PG.Core.Attributes;
using PG.Core.Services.Attributes;
using PG.StarWarsGame.Files.MEG.Holder.V1;
using PG.StarWarsGame.Files.MEG.Services.V1;
using System.Collections.Generic;

namespace PG.StarWarsGame.Files.MEG.Services
{
    /// <summary>
    ///     MEF service interface definition for handling sorted *.DAT files.
    ///     A default implementation is provided in <see cref="MegFileProcessService" />.
    ///     When requesting the default implementation via an IoC Container or registering via injection, you may pass
    ///     a file system as argument implementing <see cref="System.IO.Abstractions.IFileSystem" /> and a logger factory
    ///     implementing <see cref="Microsoft.Extensions.Logging.ILoggerFactory" />
    /// </summary>
    [Order(OrderAttribute.DEFAULT_ORDER)]
    [DefaultServiceImplementation(typeof(MegFileProcessService))]
    public interface IMegFileProcessService
    {
        /// <summary>
        ///     Packs a list of files as *.MEG archive.
        /// </summary>
        /// <param name="megArchiveName">The desired name of the archive.</param>
        /// <param name="packedFileNameToAbsoluteFilePathsMap">A list of absolute file paths, identified by their name in the MEG file.</param>
        /// <param name="targetDirectory">The target directory to which the *.MEG archive will be written.</param>
        void PackFilesAsMegArchive([NotNull] string megArchiveName, [NotNull] IDictionary<string, string> packedFileNameToAbsoluteFilePathsMap, [NotNull] string targetDirectory);

        /// <summary>
        ///     Unpacks a given *.MEG file into a given directory.
        ///     The file structure within the *.MEG file will be preserved relative to the target directory.
        /// </summary>
        /// <param name="filePath">Path to the *.MEG file to unpack.</param>
        /// <param name="targetDirectory">Directory to unpack the files into.</param>
        void UnpackMegFile([NotNull] string filePath, [NotNull] string targetDirectory);

        /// <summary>
        ///     Same as <see cref="UnpackMegFile(string,string)" />, but with a previously loaded
        ///     <see cref="MegFileHolder" /> instead of a file path to a meg file.
        /// </summary>
        /// <param name="holder">The previously loaded *.MEG file.</param>
        /// <param name="targetDirectory">Directory to unpack the files into.</param>
        void UnpackMegFile([NotNull] MegFileHolder holder, [NotNull] string targetDirectory);

        /// <summary>
        ///     Unpacks a single file from a given *.MEG file, provided the file is stored in the archive.
        /// </summary>
        /// <param name="holder">The previously loaded *.MEG file.</param>
        /// <param name="targetDirectory">Directory to unpack the file into.</param>
        /// <param name="fileName">The name of the file to unpack.</param>
        /// <param name="preserveDirectoryHierarchy">
        ///     If set to false, any directory structure within the meg file will be
        ///     disregarded.
        /// </param>
        void UnpackSingleFileFromMegFile([NotNull] MegFileHolder holder, [NotNull] string targetDirectory, [NotNull] string fileName,
            bool preserveDirectoryHierarchy = true);

        /// <summary>
        ///     Loads a *.MEG file's metadata into a <see cref="MegFileHolder" />. This holder can be used for targeted unpacking
        ///     of single files or checks for existence of a given file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>The specified *.MEG file's metadata.</returns>
        MegFileHolder Load([NotNull] string filePath);
    }
}
