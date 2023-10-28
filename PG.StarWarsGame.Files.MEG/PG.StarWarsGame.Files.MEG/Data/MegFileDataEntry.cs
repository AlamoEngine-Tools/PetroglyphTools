// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Data;

/// <summary>
/// Container to identify the location of a MEG data entry of a MEG file.
/// </summary>
public sealed class MegFileDataEntry : IEquatable<MegFileDataEntry>
{
    /// <summary>
    /// Gets the data entry.
    /// </summary>
    public MegDataEntry DataEntry { get; }

    /// <summary>
    /// Gets the meg file
    /// </summary>
    public IMegFile MegFile { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MegFileDataEntry"/> class.
    /// </summary>
    /// <param name="megFile">The meg file.</param>
    /// <param name="dataEntry">The data entry.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="megFile"/> or <paramref name="dataEntry"/> is <see langword="null"/>.</exception>
    public MegFileDataEntry(IMegFile megFile, MegDataEntry dataEntry)
    {
        MegFile = megFile ?? throw new ArgumentNullException(nameof(megFile));
        DataEntry = dataEntry ?? throw new ArgumentNullException(nameof(dataEntry));
    }

    /// <inheritdoc/>
    public bool Equals(MegFileDataEntry? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other)) 
            return true;
        return DataEntry.Equals(other.DataEntry) && MegFile.Equals(other.MegFile);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is MegFileDataEntry other && Equals(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(DataEntry, MegFile);
    }
}