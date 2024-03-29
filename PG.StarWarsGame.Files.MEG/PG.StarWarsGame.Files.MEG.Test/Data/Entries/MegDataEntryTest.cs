// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Data.Entries;


public class MegDataEntryTest : MegDataEntryBaseTest<MegDataEntryLocation>
{
    [Fact]
    public void Test_Ctor()
    {
        var entry = new MegDataEntry("path ab", new Crc32(123), new MegDataEntryLocation(1, 2), true, "path\u00A0ab");

        Assert.Equal(new Crc32(123), entry.Crc32);
        Assert.Equal("path ab", entry.FilePath);
        Assert.Equal("path\u00A0ab", entry.OriginalFilePath);
        Assert.Equal(1u, entry.Location.Offset);
        Assert.Equal(2u, entry.Location.Size);
        Assert.True(entry.Encrypted);
    }

    [Fact]
    public void Test_Ctor_DefaultLocations()
    {
        var entry = new MegDataEntry("path", new Crc32(123), default, true, "path");

        Assert.Equal(new Crc32(123), entry.Crc32);
        Assert.Equal("path", entry.FilePath);
        Assert.Equal("path", entry.OriginalFilePath);
        Assert.Equal(0u, entry.Location.Offset);
        Assert.Equal(0u, entry.Location.Size);
        Assert.True(entry.Encrypted);
    }

    [Fact]
    public void Test_Ctor_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new MegDataEntry(null!, new Crc32(0), default, false, null!));
        Assert.Throws<ArgumentException>(() =>
            new MegDataEntry("  ", new Crc32(0), default, false, null!));
        Assert.Throws<ArgumentNullException>(() =>
            new MegDataEntry("test", new Crc32(0), default, false, null!));
        Assert.Throws<ArgumentException>(() =>
            new MegDataEntry("test", new Crc32(0), default, false, ""));

        Assert.Throws<ArgumentException>(() =>
            new MegDataEntry("testÄ", new Crc32(0), default, false, ""));
    }


    [Fact]
    public void Test_Equals_HashCode()
    {
        var t = CreateLocation(1);
        var entry1 = CreateEntry("path", new Crc32(123), t, false, null);
        var entry8 = CreateEntry("path", new Crc32(123), t, true, "ABC");

        Assert.False(entry1.Equals(entry8));
        Assert.False(entry1.Equals(null));
        Assert.NotEqual(entry1.GetHashCode(), entry8.GetHashCode());
    }

    protected override MegDataEntryBase<MegDataEntryLocation> CreateEntry(string path, Crc32 crc, MegDataEntryLocation location)
    {
        return CreateEntry(path, crc, location, false, null);
    }

    protected override MegDataEntryLocation CreateLocation(int seed)
    {
        unchecked
        {
            return new MegDataEntryLocation((uint)seed, (uint)seed);
        }
    }

    public static MegDataEntry CreateEntry(string path, Crc32 crc, MegDataEntryLocation location, bool encrypted, string? originalPath)
    {
        return new MegDataEntry(path, crc, location, encrypted, originalPath ?? path);
    }

    public static MegDataEntry CreateEntry(string path, Crc32 crc = default, uint offset = 0, uint size = 0, bool encrypted = false, string? originalPath = null)
    {
        return CreateEntry(path, crc, new MegDataEntryLocation(offset, size), encrypted, originalPath);
    }
}