// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Data.EntryLocations;

/// <summary>
/// Location reference of an existing MEG data entry and its owning .MEG file.
/// </summary>
public sealed class MegDataEntryLocationReference : IDataEntryLocation, IEquatable<MegDataEntryLocationReference>
{
    /// <summary>
    /// Gets the owning .MEG file of <see cref="DataEntry"/>.
    /// </summary>
    public IMegFile MegFile { get; }

    /// <summary>
    /// Gets the referenced MEG data entry.
    /// </summary>
    public MegDataEntry DataEntry { get; }

    /// <summary>
    /// Gets a value indicating whether the data exists in the meg file referenced in this instance.
    /// </summary>
    public bool Exists => MegFile.Archive.Contains(DataEntry);

    /// <summary>
    /// Initializes a new instance of the <see cref="MegDataEntryLocationReference"/>.
    /// </summary>
    /// <param name="megFile">The owning .MEG file</param>
    /// <param name="dataEntry">The referenced <see cref="MegDataEntry"/>.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="megFile"/> or <paramref name="dataEntry"/> is <see langword="null"/>.</exception>
    public MegDataEntryLocationReference(IMegFile megFile, MegDataEntry dataEntry)
    {
        MegFile = megFile ?? throw new ArgumentNullException(nameof(megFile));
        DataEntry = dataEntry ?? throw new ArgumentNullException(nameof(dataEntry));
    }

    /// <inheritdoc />
    public bool Equals(MegDataEntryLocationReference? other)
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
        return ReferenceEquals(this, obj) || obj is MegDataEntryLocationReference other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(MegFile, DataEntry);
    }
}