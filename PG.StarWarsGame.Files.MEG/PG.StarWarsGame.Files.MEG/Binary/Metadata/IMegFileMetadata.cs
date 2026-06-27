// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.Binary.File;

namespace PG.StarWarsGame.Files.MEG.Binary.Metadata;

internal interface IMegFileMetadata : IBinaryFile
{
    /// <summary>
    /// Gets the header of the .MEG archive.
    /// </summary>
    IMegHeader Header { get; }

    /// <summary>
    /// Gets the table that holds the file names of the .MEG archive.
    /// </summary>
    BinaryTable<MegFileNameTableRecord> FileNameTable { get; }

    /// <summary>
    /// Gets the table that holds the file descriptors of the .MEG archive.
    /// </summary>
    IMegFileTable FileTable { get; }
}