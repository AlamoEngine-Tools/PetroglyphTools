// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Binary.File;

namespace PG.StarWarsGame.Files.DAT.Binary.Metadata;

internal interface IDatFileMetadata : IBinaryFile
{
    int RecordNumber { get; }
    IIndexTable IndexTable { get; }

    IKeyTable KeyTable { get; }
    IValueTable ValueTable { get; }
}