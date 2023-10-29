// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Data.Entries;

public sealed class MegFileDataEntryLocation : IEquatable<MegFileDataEntryLocation>
{
    public IMegFile MegFile { get; }

    public MegDataEntry DataEntry { get; }

    /// <inheritdoc />
    public bool Equals(MegFileDataEntryLocation? other)
    {
        if (other is null) 
            return false;
        if (ReferenceEquals(this, other)) 
            return true;
        return MegFile.Equals(other.MegFile) && DataEntry.Equals(other.DataEntry);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is MegFileDataEntryLocation other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(MegFile, DataEntry);
    }
}