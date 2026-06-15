// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Data.EntryLocations;

/// <summary>
/// Location reference of an existing MEG data entry and its owning MEG.
/// </summary>
public sealed class MegDataEntryLocationReference : IDataEntryLocation, IEquatable<MegDataEntryLocationReference>
{
    /// <summary>
    /// Gets the MEG that owns <see cref="DataEntry"/>.
    /// </summary>
    public IMegDataSource Source { get; }

    /// <summary>
    /// Gets the referenced MEG data entry.
    /// </summary>
    public MegDataEntry DataEntry { get; }

    /// <summary>
    /// Gets a value indicating whether the data exists in the MEG referenced in this instance.
    /// </summary>
    public bool Exists => Source.Archive.Contains(DataEntry);
    
    /// <summary>
    /// Initializes a new instance of the <see cref="MegDataEntryLocationReference"/>.
    /// </summary>
    /// <param name="source">The MEG that owns the entry.</param>
    /// <param name="dataEntry">The referenced <see cref="MegDataEntry"/>.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="source"/> or <paramref name="dataEntry"/> is <see langword="null"/>.</exception>
    public MegDataEntryLocationReference(IMegDataSource source, MegDataEntry dataEntry)
    {
        Source = source ?? throw new ArgumentNullException(nameof(source));
        DataEntry = dataEntry ?? throw new ArgumentNullException(nameof(dataEntry));
    }

    /// <inheritdoc />
    public bool Equals(MegDataEntryLocationReference? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return Source.Equals(other.Source) && DataEntry.Equals(other.DataEntry);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is MegDataEntryLocationReference other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Source, DataEntry);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{DescribeSource(Source)}::{DataEntry.Path}";
    }

    internal static string DescribeSource(IMegDataSource source)
    {
        return source switch
        {
            IMegFile megFile => megFile.FilePath,
            InMemoryMeg inMemoryMeg => inMemoryMeg.Name,
            _ => "<unknown MEG source>"
        };
    }
}
