// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.StarWarsGame.Files.MEG.Data.Entries;

internal sealed class ConstructingMegDataEntry : MegFileDataEntryBase, IEquatable<ConstructingMegDataEntry>
{
    public MegDataEntryOriginInfo OriginInfo { get; }

    public ConstructingMegDataEntry(MegFileDataEntry fileEntry, MegDataEntryOriginInfo originInfo) : base(fileEntry)
    {
        OriginInfo = originInfo ?? throw new ArgumentNullException(nameof(originInfo));
    }

    public bool Equals(ConstructingMegDataEntry? other)
    {
        if (ReferenceEquals(null, other)) 
            return false;
        if (ReferenceEquals(this, other)) 
            return true;
        return base.Equals(other) && OriginInfo.Equals(other.OriginInfo);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is ConstructingMegDataEntry other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), OriginInfo);
    }
}