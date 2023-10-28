// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Services;

namespace PG.StarWarsGame.Files.MEG.Data.Entries;

internal sealed class ConstructingMegDataEntry : IMegDataEntry, IEquatable<ConstructingMegDataEntry>
{
    /// <summary>
    /// 
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// 
    /// </summary>
    public Crc32 FileNameCrc32 { get; }

    /// <summary>
    /// 
    /// </summary>
    public MegDataEntryOriginInfo OriginInfo { get; }

    /// <summary>
    /// 
    /// </summary>
    public MegDataEntry DataEntry { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="dataEntry"></param>
    /// <param name="originInfo"></param>
    public ConstructingMegDataEntry(string fileName, MegDataEntry dataEntry, MegDataEntryOriginInfo originInfo)
    {
        FilePath = fileName;
        OriginInfo = originInfo;
        DataEntry = dataEntry;
    }

    /// <inheritdoc/>
    public bool Equals(ConstructingMegDataEntry? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return FilePath == other.FilePath && OriginInfo.Equals(other.OriginInfo);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is ConstructingMegDataEntry other && Equals(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(FilePath, OriginInfo);
    }
}