// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Services;

namespace PG.StarWarsGame.Files.MEG.Data.Entries;

public abstract class MegFileDataEntryBase : MegDataEntryBase, IEquatable<MegFileDataEntryBase>
{
    public MegFileDataEntry FileEntry { get; }

    public sealed override string FilePath => FileEntry.FilePath;

    public sealed override Crc32 FileNameCrc32 => FileEntry.FileNameCrc32;

    protected MegFileDataEntryBase(MegFileDataEntry fileDataEntry)
    {
        FileEntry = fileDataEntry ?? throw new ArgumentNullException(nameof(fileDataEntry));
    }

    bool IEquatable<MegFileDataEntryBase>.Equals(MegFileDataEntryBase? other)
    {
        if (other is null) 
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return FileEntry.Equals(other.FileEntry);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) 
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != GetType())
            return false;
        return Equals((MegFileDataEntryBase)obj);
    }

    public override int GetHashCode()
    {
        return FileEntry.GetHashCode();
    }
}