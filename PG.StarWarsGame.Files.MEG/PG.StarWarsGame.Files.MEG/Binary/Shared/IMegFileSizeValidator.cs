// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Files.MEG.Binary.Metadata;

namespace PG.StarWarsGame.Files.MEG.Binary;

internal interface IMegFileSizeValidator
{
    bool Validate(long bytesRead, long archiveSize, IMegFileMetadata metadata);
}