// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Data;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;

namespace PG.StarWarsGame.Files.MEG.Binary;

internal class MegDataEntryBinaryInformation(
    Crc32 crc32,
    string path,
    MegDataEntrySize sizes,
    bool encrypted,
    string originalPath,
    MegDataEntryOriginInfo origin) : IHasCrc32
{
    public string Path { get; } = path;

    public string OriginalPath { get; } = originalPath;

    public Crc32 Crc32 { get; } = crc32;

    public bool Encrypted { get; } = encrypted;

    public MegDataEntrySize Sizes { get; } = sizes;

    public MegDataEntryOriginInfo Origin { get; } = origin;
}