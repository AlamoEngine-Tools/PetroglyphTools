// // Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Binary.Shared.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.V1.Metadata;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.File.Type.Definition.V1;

[TestClass]
public class MegMetadataTest
{
    [TestMethod]
    public void Ctor_Test__ThrowsArgumentNullException()
    {
        var fileTable = new MegFileTable(new List<MegFileContentTableRecord>
            { new(default, 0, 0, 0, 0) });
        Assert.ThrowsException<ArgumentNullException>(() => new MegMetadata(default, null!, fileTable));


        var fileNameTable = new MegFileNameTable(new List<MegFileNameTableRecord>
            { new("123", Encoding.ASCII) });
        Assert.ThrowsException<ArgumentNullException>(() =>
            new MegMetadata(default, fileNameTable, null!));
    }

    [TestMethod]
    public void Ctor_Test__Correct()
    {
        new MegMetadata(
            new MegHeader(1, 1),
            new MegFileNameTable(new List<MegFileNameTableRecord> { new("123", Encoding.ASCII) }),
            new MegFileTable(new List<MegFileContentTableRecord> { default }));

        Assert.IsTrue(true);
    }
}