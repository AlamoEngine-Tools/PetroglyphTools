// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using PG.Commons.Data.Files;

[assembly: InternalsVisibleTo("PG.StarWarsGame.Files.DAT.Test")]

namespace PG.StarWarsGame.Files.DAT.Files
{
    /// <summary>
    /// Base implementation of <see cref="IAlamoFileType"/> for the <code>*.DAT</code> files used for localisation.
    /// The Alamo Engine knows sorted and unsorted versions. The sorted file type <see cref="SortedDatAlamoFileType"/>
    /// does not allow duplicate keys and its contents are sorted ascending by the key's custom CRC32 <see cref="PG.Commons.Util.ChecksumUtility"/>.
    /// It is used for all localisation purposes.
    /// The unsorted file type <see cref="UnsortedDatAlamoFileType"/> is used as a credits file (yes, like those movie
    /// credits). It allows duplicate keys which are used as formatting instructions. 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class ADatAlamoFileType : IAlamoFileType
    {
        private const FileType FILE_TYPE = FileType.Binary;
        private const string FILE_EXTENSION = "dat";

        /// <summary>
        /// The discerning flag between <see cref="SortedDatAlamoFileType"/> and <see cref="UnsortedDatAlamoFileType"/>.
        /// </summary>
        public abstract bool IsSorted { get; }

        public FileType Type => FILE_TYPE;
        public string FileExtension => FILE_EXTENSION;

        /// <summary>
        /// The Alamo Dat File base name convention for the file type.
        /// </summary>
        public abstract string FileBaseName { get; }
        

        /// <summary>
        /// The Alamo Dat File separator convention for the file type.
        /// </summary>
        public abstract string FileBaseNameSeparator { get; }
    }
}
