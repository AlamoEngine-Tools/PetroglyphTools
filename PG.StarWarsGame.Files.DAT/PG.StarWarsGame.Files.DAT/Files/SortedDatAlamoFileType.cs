// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Services;

namespace PG.StarWarsGame.Files.DAT.Files;

/// <summary>
/// The sorted file type implementation of <see cref="PG.Commons.Data.Files.IAlamoFileType"/>.
/// does not allow duplicate keys and its contents are sorted ascending by the key's custom CRC32 <see cref="ChecksumService"/>.
/// It is used for all localisation purposes.
/// </summary>
public sealed class SortedDatAlamoFileType : ADatAlamoFileType
{
    private const bool IS_SORTED = true;

    public override bool IsSorted => IS_SORTED;
}
