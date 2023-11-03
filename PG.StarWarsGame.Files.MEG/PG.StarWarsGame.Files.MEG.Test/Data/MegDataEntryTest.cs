// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Services;
using PG.StarWarsGame.Files.MEG.Data.Entries;

namespace PG.StarWarsGame.Files.MEG.Test.Data;

[TestClass]
public class MegDataEntryTest
{
    //[TestMethod]
    //public void Test_Ctor()
    //{
    //    var entry = new MegDataEntryIdentity(new Crc32(123), "path", 456, 789);

    //    Assert.AreEqual(new Crc32(123), entry.FileNameCrc32);
    //    Assert.AreEqual("path", entry.FilePath);
    //    Assert.AreEqual(456u, entry.Offset);
    //    Assert.AreEqual(789u, entry.Size);
    //}


    //[TestMethod]
    //public void Test_Equals_HashCode()
    //{
    //    var entry1 = new MegDataEntryIdentity(new Crc32(123), "path", 456, 789);
    //    var entry2 = new MegDataEntryIdentity(new Crc32(123), "path", 456, 789);

    //    var entry3 = new MegDataEntryIdentity(new Crc32(456), "path", 456, 789);
    //    var entry4 = new MegDataEntryIdentity(new Crc32(123), "test", 456, 789);
    //    var entry5 = new MegDataEntryIdentity(new Crc32(123), "path", 123, 789);
    //    var entry6 = new MegDataEntryIdentity(new Crc32(123), "path", 456, 123);
    //    var entry7 = new MegDataEntryIdentity(new Crc32(123), "PATH", 456, 789);

    //    Assert.AreEqual(entry1, entry2);
    //    Assert.AreEqual(entry1, (object)entry2);
    //    Assert.AreEqual(entry1, entry1);
    //    Assert.AreEqual(entry1.GetHashCode(), entry2.GetHashCode());

    //    Assert.AreNotEqual(entry1, (object?)null);
    //    Assert.AreNotEqual(entry1, (MegDataEntryIdentity?)null);
    //    Assert.AreNotEqual(entry1, entry3);
    //    Assert.AreNotEqual(entry1, entry4);
    //    Assert.AreNotEqual(entry1, entry5);
    //    Assert.AreNotEqual(entry1, entry6);
    //    Assert.AreNotEqual(entry1, entry7);

    //    Assert.AreNotEqual(entry1.GetHashCode(), entry3.GetHashCode());
    //    Assert.AreNotEqual(entry1.GetHashCode(), entry4.GetHashCode());
    //    Assert.AreNotEqual(entry1.GetHashCode(), entry5.GetHashCode());
    //    Assert.AreNotEqual(entry1.GetHashCode(), entry6.GetHashCode());
    //    Assert.AreNotEqual(entry1.GetHashCode(), entry7.GetHashCode());
    //}


    //[TestMethod]
    //public void Test_CompareTo()
    //{
    //    var entry1 = new MegDataEntryIdentity(new Crc32(123), "path", 456, 789);
    //    var entry2 = new MegDataEntryIdentity(new Crc32(123), "path", 456, 789);


    //    Assert.AreEqual(0, entry1.CompareTo(entry1));
    //    Assert.AreEqual(0, entry1.CompareTo(entry2));

    //    Assert.AreEqual(1, entry1.CompareTo(null));


    //    var entry3 = new MegDataEntryIdentity(new Crc32(0), "path", 456, 789);
    //    var entry4 = new MegDataEntryIdentity(new Crc32(789), "path", 456, 789);

    //    Assert.AreEqual(-1, entry1.CompareTo(entry4));
    //    Assert.AreEqual(1, entry1.CompareTo(entry3));
    //}

    public static MegDataEntry CreateEntry(string path, Crc32 crc = default, uint offset = 0, uint size = 0, bool encrypted = false)
    {
        return new MegDataEntry(path, new Crc32(0), new MegDataEntryLocation(offset, size), encrypted);
    }
}