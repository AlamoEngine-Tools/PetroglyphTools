// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using JetBrains.Annotations;
using PG.Commons.Data.Holder;
using PG.Commons.Util;
using PG.StarWarsGame.Files.MEG.Files.V1;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PG.StarWarsGame.Files.MEG.Holder.V1
{
    /// <summary>
    ///     The provided holder for Petroglyph's
    ///     <a href="https://modtools.petrolution.net/docs/MegFileFormat">v1 *.MEG files</a>.
    ///     *.MEG or Mega files are a proprietary archive type bundling files together in a RAM friendly way.
    ///     <remarks>
    ///         This file holder does not hold all files that are packaged in a *.MEG file,
    ///         but all necessary meta-information to extract a given file on-demand.
    ///     </remarks>
    /// </summary>
    public sealed class MegFileHolder : IFileHolder<IList<MegFileDataEntry>, MegAlamoFileType>
    {
        /// <summary>
        ///     .ctor
        /// </summary>
        /// <param name="filePath">The path to the directory that holds the file on disc.</param>
        /// <param name="fileName">The desired file name without the file extension.</param>
        public MegFileHolder([NotNull] string filePath, [NotNull] string fileName)
        {
            FilePath = StringUtility.HasText(filePath)
                ? filePath
                : throw new ArgumentNullException(nameof(filePath), $"Argument {nameof(filePath)} may not be null.");
            FileName = StringUtility.HasText(fileName)
                ? fileName
                : throw new ArgumentNullException(nameof(fileName), $"Argument {nameof(fileName)} may not be null.");
        }

        /// <inheritdoc />
        public string FilePath { get; }

        /// <inheritdoc />
        public string FileName { get; }

        /// <inheritdoc />
        public MegAlamoFileType FileType { get; } = new MegAlamoFileType();

        /// <inheritdoc />
        public IList<MegFileDataEntry> Content { get; set; } = new List<MegFileDataEntry>();

        /// <inheritdoc />
        public string FullyQualifiedName => $"{FileName}.{FileType.FileExtension}";

        /// <summary>
        ///     Tries to get a <see cref="MegFileDataEntry" /> by matching the provided filename.
        ///     If multiple matches are found it will always return the first match.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="megFileDataEntry"></param>
        /// <returns></returns>
        public bool TryGetFirstMegFileDataEntryWithMatchingName([NotNull] string fileName,
            [CanBeNull] out MegFileDataEntry megFileDataEntry)
        {
            if (StringUtility.HasText(fileName))
            {
                foreach (MegFileDataEntry dataEntry in Content.Where(dataEntry =>
                    dataEntry.RelativeFilePath.Contains(fileName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    megFileDataEntry = dataEntry;
                    return true;
                }
            }

            megFileDataEntry = null;
            return false;
        }

        /// <summary>
        ///     Tries to get a list of <see cref="MegFileDataEntry" /> by matching the provided filename.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="megFileDataEntries"></param>
        /// <returns></returns>
        public bool TryGetAllMegFileDataEntriesWithMatchingName([NotNull] string fileName,
            [NotNull] out IList<MegFileDataEntry> megFileDataEntries)
        {
            if (!StringUtility.HasText(fileName))
            {
                megFileDataEntries = new List<MegFileDataEntry>();
                return false;
            }

            megFileDataEntries = Content.Where(dataEntry =>
                dataEntry.RelativeFilePath.Contains(fileName, StringComparison.InvariantCultureIgnoreCase)).ToList();
            return megFileDataEntries.Any();
        }
    }
}
