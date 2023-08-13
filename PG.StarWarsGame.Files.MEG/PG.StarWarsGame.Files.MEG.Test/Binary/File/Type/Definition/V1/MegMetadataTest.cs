// // Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Binary.V1.Metadata;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.File.Type.Definition.V1;

[TestClass]
public class MegMetadataTest
{
    [TestMethod]
    public void Ctor_Test__ThrowsArgumentNullException()
    { 
        Assert.ThrowsException<ArgumentNullException>(() => new MegMetadata(default, null!, new MegFileTable(new List<MegFileContentTableRecord>())));
        Assert.ThrowsException<ArgumentNullException>(() => new MegMetadata(default, new MegFileNameTable(new List<MegFileNameTableRecord>()), null!));
    }

    [TestMethod]
    public void Ctor_Test__ThrowsArgumentException()
    {
        Assert.ThrowsException<ArgumentException>(() => new MegMetadata(
            new MegHeader(1, 1),
            new MegFileNameTable(new List<MegFileNameTableRecord>()),
            new MegFileTable(new List<MegFileContentTableRecord>())));

        Assert.ThrowsException<ArgumentException>(() => new MegMetadata(
            new MegHeader(1, 1),
            new MegFileNameTable(new List<MegFileNameTableRecord> { new("123", Encoding.ASCII)}),
            new MegFileTable(new List<MegFileContentTableRecord>())));

        Assert.ThrowsException<ArgumentException>(() => new MegMetadata(
            new MegHeader(1, 1),
            new MegFileNameTable(new List<MegFileNameTableRecord> { new("123", Encoding.ASCII) }),
            new MegFileTable(new List<MegFileContentTableRecord> { default, default })));
    }

    [TestMethod]
    public void Ctor_Test__Correct()
    {
        var _ = new MegMetadata(
            new MegHeader(1, 1),
            new MegFileNameTable(new List<MegFileNameTableRecord> { new("123", Encoding.ASCII) }),
            new MegFileTable(new List<MegFileContentTableRecord> { default }));

        Assert.IsTrue(true);
    }
}