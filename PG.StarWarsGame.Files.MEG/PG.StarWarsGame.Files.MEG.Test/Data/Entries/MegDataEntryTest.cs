// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;

namespace PG.StarWarsGame.Files.MEG.Test.Data.Entries;

[TestClass]
public class MegDataEntryTest : MegDataEntryBaseTest<MegDataEntryLocation>
{
    [TestMethod]
    public void Test_Ctor()
    {
        var entry = new MegDataEntry("path ab", new Crc32(123), new MegDataEntryLocation(1, 2), true, "path\u00A0ab");

        Assert.AreEqual(new Crc32(123), entry.Crc32);
        Assert.AreEqual("path ab", entry.FilePath);
        Assert.AreEqual("path\u00A0ab", entry.OriginalFilePath);
        Assert.AreEqual(1u, entry.Location.Offset);
        Assert.AreEqual(2u, entry.Location.Size);
        Assert.IsTrue(entry.Encrypted);
    }

    [TestMethod]
    public void Test_Ctor_DefaultLocations()
    {
        var entry = new MegDataEntry("path", new Crc32(123), default, true, "path");

        Assert.AreEqual(new Crc32(123), entry.Crc32);
        Assert.AreEqual("path", entry.FilePath);
        Assert.AreEqual("path", entry.OriginalFilePath);
        Assert.AreEqual(0u, entry.Location.Offset);
        Assert.AreEqual(0u, entry.Location.Size);
        Assert.IsTrue(entry.Encrypted);
    }

    [TestMethod]
    public void Test_Ctor_Throws()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
            new MegDataEntry(null!, new Crc32(0), default, false, null!));
        Assert.ThrowsException<ArgumentException>(() =>
            new MegDataEntry("  ", new Crc32(0), default, false, null!));
        Assert.ThrowsException<ArgumentNullException>(() =>
            new MegDataEntry("test", new Crc32(0), default, false, null!));
        Assert.ThrowsException<ArgumentException>(() =>
            new MegDataEntry("test", new Crc32(0), default, false, ""));

        Assert.ThrowsException<ArgumentException>(() =>
            new MegDataEntry("testÄ", new Crc32(0), default, false, ""));
    }


    [TestMethod]
    public void Test_Equals_HashCode()
    {
        var t = CreateLocation(1);
        var entry1 = CreateEntry("path", new Crc32(123), t, false, null);
        var entry8 = CreateEntry("path", new Crc32(123), t, true, "ABC");

        Assert.IsFalse(entry1.Equals(entry8));
        Assert.IsFalse(entry1.Equals(null));
        Assert.AreNotEqual(entry1.GetHashCode(), entry8.GetHashCode());
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