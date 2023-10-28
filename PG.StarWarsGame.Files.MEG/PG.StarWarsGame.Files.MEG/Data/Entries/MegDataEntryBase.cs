// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Services;

namespace PG.StarWarsGame.Files.MEG.Data.Entries;

public abstract class MegDataEntryBase : IMegDataEntry
{
    public abstract string FilePath { get; }

    public abstract Crc32 FileNameCrc32 { get; }

    // In contrast to equality, comparison is only based on the <see cref="FileNameCrc32"/> checksum.
    public int CompareTo(IMegDataEntry other)
    {
        // IMPORTANT: Changing the logic here also requires to update the binary models!
        if (ReferenceEquals(this, other))
            return 0;
        if (ReferenceEquals(null, other))
            return 1;
        return FileNameCrc32.CompareTo(other.FileNameCrc32);
    }
}