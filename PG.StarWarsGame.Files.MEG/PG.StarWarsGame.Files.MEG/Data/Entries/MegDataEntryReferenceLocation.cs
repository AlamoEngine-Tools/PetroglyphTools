// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Data.Entries;

/// <summary>
/// Location reference of an existing MEG data entry and its owning .MEG file.
/// </summary>
public sealed class MegDataEntryReferenceLocation : IEquatable<MegDataEntryReferenceLocation>
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
    /// Initializes a new instance of the <see cref="MegDataEntryReferenceLocation"/>.
    /// </summary>
    /// <param name="megFile">The owning .MEG file</param>
    /// <param name="dataEntry">The referenced <see cref="MegDataEntry"/>.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="megFile"/> or <paramref name="dataEntry"/> is <see langword="null"/>.</exception>
    /// <exception cref="FileNotInMegException">The <paramref name="dataEntry"/> does not exist in the <paramref name="megFile"/>'s archive.</exception>
    public MegDataEntryReferenceLocation(IMegFile megFile, MegDataEntry dataEntry)
    {
        if (megFile == null) 
            throw new ArgumentNullException(nameof(megFile));
        if (dataEntry == null) 
            throw new ArgumentNullException(nameof(dataEntry));
        if (megFile.Content.Contains(dataEntry))
            throw new FileNotInMegException(megFile.FilePath, dataEntry.FilePath);
        
        MegFile = megFile;
        DataEntry = dataEntry;
    }

    /// <inheritdoc />
    public bool Equals(MegDataEntryReferenceLocation? other)
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
        return ReferenceEquals(this, obj) || obj is MegDataEntryReferenceLocation other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(MegFile, DataEntry);
    }
}