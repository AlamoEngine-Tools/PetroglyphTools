// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;

namespace PG.StarWarsGame.Files.MEG.Data.Entries;

/// <summary>
/// Intermediate MEG data entry necessary to construct new .MEG files which data entries from various locations.
/// <br/>
/// This data entry type hold the following information:
/// <list type="bullet">
/// <item>The <see cref="MegDataEntry"/> which represent and locates this entry within it to be built .MEG file. </item>
/// <item>The location of the actual file. This is either an archived data entry or a file on the file system.</item>
/// </list>
/// </summary>
/// <remarks>
/// <b>Note:</b> <see cref="DataEntry"/>'s location already is dependent to the binary representation of the to be built .MEG file.
/// </remarks>
internal sealed class VirtualMegDataEntryReference : MegDataEntryBase<MegDataEntryOriginInfo>, IEquatable<VirtualMegDataEntryReference>
{
    public MegDataEntry DataEntry { get; }

    /// <inheritdoc />
    public override string FilePath => DataEntry.FilePath;

    /// <inheritdoc />
    public override Crc32 Crc32 => DataEntry.Crc32;

    public VirtualMegDataEntryReference(MegDataEntry dataEntry, MegDataEntryOriginInfo originInfo) : base(originInfo)
    {
        DataEntry = dataEntry ?? throw new ArgumentNullException(nameof(dataEntry));
    }

    /// <inheritdoc />
    public bool Equals(VirtualMegDataEntryReference? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return DataEntry.Equals(other.DataEntry) && base.Equals(other);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj is not VirtualMegDataEntryReference other)
            return false;
        return Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), DataEntry);
    }
}