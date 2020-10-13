using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using PG.Commons.Data.Holder;
using PG.Commons.Util;
using PG.StarWarsGame.Files.MEG.Files.V1;

[assembly: InternalsVisibleTo("PG.StarWarsGame.Files.MEG.Test")]

namespace PG.StarWarsGame.Files.MEG.Holder.V1
{
    /// <summary>
    /// The provided holder for Petroglyph's <a href="https://modtools.petrolution.net/docs/MegFileFormat">v1 *.MEG files</a>.
    /// *.MEG or Mega files are a proprietary archive type bundling files together in a RAM friendly way. 
    /// <remarks>This file holder does not hold all files that are packaged in a *.MEG file,
    /// but all necessary meta-information to extract a given file on-demand.</remarks>
    /// </summary>
    public sealed class MegFileHolder : IFileHolder<List<MegFileDataEntry>, MegAlamoFileType>
    {
        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="filePath">The path to the directory that holds the file on disc.</param>
        /// <param name="fileName">The desired file name without the file extension.</param>
        public MegFileHolder([NotNull] string filePath, [NotNull] string fileName)
        {
            FilePath = filePath ?? throw new ArgumentNullException($"Argument {nameof(filePath)} may not be null.");
            FileName = fileName ?? throw new ArgumentNullException($"Argument {nameof(fileName)} may not be null.");
        }

        /// <inheritdoc />
        public string FilePath { get; }

        /// <inheritdoc />
        public string FileName { get; }

        /// <inheritdoc />
        public MegAlamoFileType FileType { get; } = new MegAlamoFileType();

        /// <inheritdoc />
        public List<MegFileDataEntry> Content { get; set; } = new List<MegFileDataEntry>();

        /// <summary>
        /// Tries to get a <see cref="MegFileDataEntry"/> by the provided filename.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="megFileDataEntry"></param>
        /// <returns></returns>
        public bool TryGetMegFileDataEntry(string fileName, out MegFileDataEntry megFileDataEntry)
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
    }
}