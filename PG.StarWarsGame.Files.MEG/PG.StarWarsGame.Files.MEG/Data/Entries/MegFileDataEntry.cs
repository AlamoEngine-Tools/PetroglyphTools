// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Services;

namespace PG.StarWarsGame.Files.MEG.Data.Entries;

public sealed class MegFileDataEntry : MegDataEntryBase<MegFileDataEntryLocation>, IEquatable<MegFileDataEntry>
{
    public override string FilePath => Location.DataEntry.FilePath;
    public override Crc32 FileNameCrc32 => Location.DataEntry.FileNameCrc32;

    public MegFileDataEntry(MegFileDataEntryLocation location) : base(location)
    {
    }

    public bool Equals(MegFileDataEntry other)
    {
        return base.Equals(other);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}