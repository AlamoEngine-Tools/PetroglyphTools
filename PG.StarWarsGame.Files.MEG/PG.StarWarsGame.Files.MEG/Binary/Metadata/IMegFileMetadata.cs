// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Binary.File;

namespace PG.StarWarsGame.Files.MEG.Binary.Metadata;

internal interface IMegFileMetadata : IBinaryFile
{
    IMegHeader Header { get; }

    IMegFileNameTable FileNameTable { get; }

    IMegFileTable FileTable { get; }
}