// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Services;
using System;

namespace PG.StarWarsGame.Files.MEG.Data.Entries;

internal sealed class MegFileDataEntryReference : MegDataEntryBase<MegDataEntryOriginInfo>, IEquatable<MegFileDataEntryReference>
{
    public MegDataEntry DataEntry { get; }

    /// <inheritdoc />
    public override string FilePath => DataEntry.FilePath;

    /// <inheritdoc />
    public override Crc32 FileNameCrc32 => DataEntry.FileNameCrc32;

    public MegFileDataEntryReference(MegDataEntry dataEntry, MegDataEntryOriginInfo originInfo) : base(originInfo)
    {
        DataEntry = dataEntry ?? throw new ArgumentNullException(nameof(dataEntry));
    }

    /// <inheritdoc />
    public bool Equals(MegFileDataEntryReference? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return DataEntry.Equals(other.DataEntry) && base.Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), DataEntry);
    }
}