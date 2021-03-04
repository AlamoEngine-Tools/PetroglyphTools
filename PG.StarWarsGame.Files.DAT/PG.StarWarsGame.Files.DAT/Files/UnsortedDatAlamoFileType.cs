// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("PG.StarWarsGame.Files.DAT.Test")]

namespace PG.StarWarsGame.Files.DAT.Files
{
    /// <summary>
    /// The unsorted file type implementation of <see cref="PG.Commons.Data.Files.IAlamoFileType"/> is used as a credits
    /// file (yes, like those movie credits). It allows duplicate keys which are used as formatting instructions and the
    /// given order of keys is retained.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class UnsortedDatAlamoFileType : ADatAlamoFileType
    {
        private const bool IS_SORTED = false;
        private const string FILE_BASE_NAME = "creditstext";
        private const string FILE_BASE_SEPARATOR = "_";

        public override bool IsSorted => IS_SORTED;
        public override string FileBaseName => FILE_BASE_NAME;
        public override string FileBaseNameSeparator => FILE_BASE_SEPARATOR;
    }
}
