// // Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for details.

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
    }

    [TestMethod]
    public void Ctor_Test__ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() => new MegHeader(1, 2));
    }

    [TestMethod]
    public void Ctor_Test__Correct()
    {
        var _ = new MegHeader(1, 1);
        var __ = new MegHeader(int.MaxValue, int.MaxValue);
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void Test_Size()
    {
        Assert.AreEqual(8, default(MegHeader).Size);
    }
}