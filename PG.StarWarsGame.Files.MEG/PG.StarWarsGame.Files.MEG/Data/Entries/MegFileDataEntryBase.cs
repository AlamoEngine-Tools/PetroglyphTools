// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Services;

namespace PG.StarWarsGame.Files.MEG.Data.Entries;

public abstract class MegFileEntryDataBase<T> : MegDataEntryBase<T>, IEquatable<MegFileEntryDataBase<T>> where T : notnull
{
    public MegFileDataEntry FileEntry { get; }

    public sealed override string FilePath => FileEntry.FilePath;

    public sealed override Crc32 FileNameCrc32 => FileEntry.FileNameCrc32;

    protected MegFileEntryDataBase(MegFileDataEntry fileDataEntry, T location) : base(location)
    {
        FileEntry = fileDataEntry ?? throw new ArgumentNullException(nameof(fileDataEntry));
    }

    public bool Equals(MegFileEntryDataBase<T>? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return FileEntry.Equals(other.FileEntry) && base.Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), FileEntry);
    }
}