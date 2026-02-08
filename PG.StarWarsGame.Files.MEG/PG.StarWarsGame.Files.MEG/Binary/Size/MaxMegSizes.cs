// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.StarWarsGame.Files.MEG.Binary.Size;

internal readonly ref struct MaxMegSizes
{
    public required uint MaxFileSize { get; init; }
    public required uint MaxEntrySize { get; init; }
}