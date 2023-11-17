// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using PG.Commons.Services;

namespace PG.StarWarsGame.Files.MEG.Data.Entries;

/// <summary>
/// Base class to implement an <see cref="IMegDataEntry{T}"/>, handling data entry comparison and equality. 
/// </summary>
/// <inheritdoc cref="IMegDataEntry{T}"/>
public abstract class MegDataEntryBase<T> : IMegDataEntry<T>, IEquatable<MegDataEntryBase<T>> where T : notnull
{
    /// <inheritdoc />
    public abstract string FilePath { get; }

    /// <inheritdoc />
    public abstract Crc32 FileNameCrc32 { get; }

    /// <inheritdoc />
    public T Location { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MegDataEntryBase{T}"/> class with a given data entry location.
    /// </summary>
    /// <param name="location">The location information of this entry.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="location"/> is <see langword="null"/>.</exception>
    protected MegDataEntryBase(T location)
    {
        Location = location ?? throw new ArgumentNullException(nameof(location));
    }

    /// <inheritdoc/>
    public int CompareTo(IMegDataEntry? other)
    {
        // IMPORTANT: Changing the logic here also requires to update the binary models!
        if (ReferenceEquals(this, other))
            return 0;
        if (other is null)
            return 1;
        return FileNameCrc32.CompareTo(other.FileNameCrc32);
    }

    /// <inheritdoc />
    public bool Equals(MegDataEntryBase<T>? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return FilePath == other.FilePath && FileNameCrc32.Equals(other.FileNameCrc32) && EqualityComparer<T>.Default.Equals(Location, other.Location);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is null) 
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != GetType())
            return false;
        return Equals((MegDataEntryBase<T>)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(FilePath, FileNameCrc32, Location);
    }
}