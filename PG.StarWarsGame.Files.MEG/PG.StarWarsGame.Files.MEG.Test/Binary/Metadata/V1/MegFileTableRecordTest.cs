﻿// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.Metadata.V1;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Metadata.V1;

[TestClass]
public class MegFileTableRecordTest
{
    [TestMethod]
    public void Ctor_Test__ThrowsArgumentOutOfRangeException()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new MegFileTableRecord(
                new Crc32(0),
                int.MaxValue + 1u,
                0,
                0,
                0));

        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new MegFileTableRecord(
            new Crc32(0),
            0,
            0,
            0,
            int.MaxValue + 1u));
    }

    [TestMethod]
    public void Ctor_Test__Correct()
    {
        var record = new MegFileTableRecord(new Crc32(0), 1, 2, 3, 4);
        Assert.AreEqual(0, (int)record.Crc32);
        Assert.AreEqual(1, (int)record.FileTableRecordIndex);
        Assert.AreEqual(2, (int)record.FileSize);
        Assert.AreEqual(3, (int)record.FileOffset);
        Assert.AreEqual(4, (int)record.FileNameIndex);
    }

    [TestMethod]
    public void Ctor_Test__Compare()
    {
        var r0 = new MegFileTableRecord(new Crc32(0), 1, 1, 1, 1);
        var r1 = new MegFileTableRecord(new Crc32(1), 0, 0, 0, 0);

        Assert.IsTrue(r0 < r1);
        Assert.IsFalse(r0 > r1);
        Assert.IsTrue(r1 > r0);
        Assert.IsFalse(r1 < r0);

        Assert.IsTrue(r1 <= r1);
        Assert.IsTrue(r1 >= r1);

        Assert.AreEqual(0, r0.CompareTo(r0));
        Assert.AreEqual(-1, r0.CompareTo(r1));
        Assert.AreEqual(1, r1.CompareTo(r0));

        Assert.AreEqual(1, ((IComparable<IMegFileDescriptor>)r1).CompareTo(null!));
        Assert.AreEqual(0, ((IComparable<IMegFileDescriptor>)r1).CompareTo(r1));
        Assert.AreEqual(1, ((IComparable<IMegFileDescriptor>)r1).CompareTo(r0));
        Assert.AreEqual(-1, ((IComparable<IMegFileDescriptor>)r0).CompareTo(r1));
    }

    [TestMethod]
    public void Ctor_Test__Bytes()
    {
        var record = new MegFileTableRecord(new Crc32(1), 2, 3, 4, 5);

        var bytes = new byte[]
        {
            0x1, 0x0, 0x0, 0x0,
            0x2, 0x0, 0x0, 0x0,
            0x3, 0x0, 0x0, 0x0,
            0x4, 0x0, 0x0, 0x0,
            0x5, 0x0, 0x0, 0x0,
        };
        CollectionAssert.AreEqual(bytes, record.Bytes);
    }
}