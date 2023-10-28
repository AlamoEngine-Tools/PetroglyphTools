// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using PG.Commons.Services;

namespace PG.StarWarsGame.Files.MEG.Data.Entries;

public abstract class MegDataEntryBase<T> : IMegDataEntry<T>, IEquatable<MegDataEntryBase<T>> where T : notnull
{
    public abstract string FilePath { get; }

    public abstract Crc32 FileNameCrc32 { get; }

    public T Location { get; }

    protected MegDataEntryBase(T location)
    {
        Location = location ?? throw new ArgumentNullException(nameof(location));
    }

    // In contrast to equality, comparison is only based on the <see cref="FileNameCrc32"/> checksum.
    public int CompareTo(IMegDataEntry other)
    {
        // IMPORTANT: Changing the logic here also requires to update the binary models!
        if (ReferenceEquals(this, other))
            return 0;
        if (ReferenceEquals(null, other))
            return 1;
        return FileNameCrc32.CompareTo(other.FileNameCrc32);
    }

    public virtual bool Equals(MegDataEntryBase<T>? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return FilePath == other.FilePath && FileNameCrc32.Equals(other.FileNameCrc32) && EqualityComparer<T>.Default.Equals(Location, other.Location);
    }

    public sealed override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) 
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != GetType())
            return false;
        return Equals((MegDataEntryBase<T>)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(FilePath, FileNameCrc32, Location);
    }
}