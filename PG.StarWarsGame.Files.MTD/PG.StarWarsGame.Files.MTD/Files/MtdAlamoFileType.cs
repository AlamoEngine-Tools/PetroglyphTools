// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Diagnostics.CodeAnalysis;
using PG.Commons.Data.Files;

namespace PG.StarWarsGame.Files.MTD.Files
{
    /// <summary>
    ///     Minimal file info for the
    ///     <a href="https://modtools.petrolution.net/docs/MtdFileFormat">*.MTD Mega Texture Directory file type</a>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class MtdAlamoFileType : IAlamoFileType
    {
        private const FileType FILE_TYPE = FileType.TextureAtlasWithMetaInformation;
        private const string FILE_EXTENSION = "mtd";

        public FileType Type => FILE_TYPE;
        public string FileExtension => FILE_EXTENSION;
    }
}
