// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;

namespace PG.StarWarsGame.Files.MEG.Data.Entries;

/// <summary>
/// References an existing <see cref="MegDataEntry"/>.
/// </summary>
/// <remarks>
/// In contrast to <see cref="MegDataEntry"/> this class is aware its owning MEG file
/// and thus is capable referencing a data entry without further information.
/// </remarks>
public sealed class MegDataEntryReference : MegDataEntryBase<MegDataEntryLocationReference>, IEquatable<MegDataEntryReference>
{
    /// <inheritdoc />
    public override string FilePath => Location.DataEntry.FilePath;

    /// <inheritdoc />
    public override Crc32 Crc32 => Location.DataEntry.Crc32;

    /// <summary>
    /// Initializes a new instance of the <see cref="MegDataEntryReference"/>.
    /// </summary>
    /// <param name="location">The full location information of this data entry.</param>
    public MegDataEntryReference(MegDataEntryLocationReference location) : base(location)
    {
    }

    /// <inheritdoc />
    public bool Equals(MegDataEntryReference? other)
    {
        return base.Equals(other);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj is not MegDataEntryReference other)
            return false;
        return Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}