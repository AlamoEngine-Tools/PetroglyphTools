// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Data.Entries;

public sealed class MegFileDataEntryReference : MegFileDataEntryBase, IEquatable<MegFileDataEntryReference>
{
    public IMegFile MegFile { get; }

    public MegFileDataEntryReference(IMegFile megFile, MegFileDataEntry fileEntry) : base(fileEntry)
    {
        MegFile = megFile ?? throw new ArgumentNullException(nameof(megFile));
    }

    public bool Equals(MegFileDataEntryReference? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return base.Equals(other) && MegFile.Equals(other.MegFile);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is MegFileDataEntryReference other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), MegFile);
    }
}