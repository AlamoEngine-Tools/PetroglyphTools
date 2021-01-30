// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using PG.Commons.Binary.File;

[assembly: InternalsVisibleTo("PG.StarWarsGame.Files.MEG.Test")]

namespace PG.StarWarsGame.Files.MEG.Binary.File.Type.Definition.V1
{
    internal class MegFile : IBinaryFile
    {
        public MegFile(MegHeader header, MegFileNameTable fileNameTable, MegFileContentTable fileContentTable)
        {
            Header = header ?? throw new ArgumentNullException(nameof(header));
            FileNameTable = fileNameTable ?? throw new ArgumentNullException(nameof(fileNameTable));
            FileContentTable = fileContentTable ?? throw new ArgumentNullException(nameof(fileContentTable));
        }

        [NotNull] internal MegHeader Header { get; }

        [NotNull] internal MegFileNameTable FileNameTable { get; }

        [NotNull] public MegFileContentTable FileContentTable { get; }

        public byte[] ToBytes()
        {
            List<byte> b = new List<byte>();
            b.AddRange(Header.ToBytes());
            b.AddRange(FileNameTable.ToBytes());
            b.AddRange(FileContentTable.ToBytes());
            return b.ToArray();
        }
    }
}
