// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Binary.V1.Metadata;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.File.Type.Definition.V1;

[TestClass]
public class MegHeaderTest
{
    [TestMethod]
    public void Ctor_Test__ThrowsArgumentOORException()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new MegHeader((uint)int.MaxValue + 1, (uint)int.MaxValue + 1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => new MegHeader(0, 0));
    }

    [TestMethod]
    public void Ctor_Test__ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() => new MegHeader(1, 2));
    }

    [TestMethod]
    public void Ctor_Test__Correct()
    {
        new MegHeader(1, 1);
        new MegHeader(int.MaxValue, int.MaxValue);
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void Test_Size()
    {
        Assert.AreEqual(8, default(MegHeader).Size);
    }

    [TestMethod]
    public void Test_Bytes()
    {
        var header = new MegHeader(2, 2);
        var expectedBytes = new byte[]
        {
            0x2, 0x0, 0x0, 0x0,
            0x2, 0x0, 0x0, 0x0
        };
        CollectionAssert.AreEqual(expectedBytes, header.Bytes);
    }
}