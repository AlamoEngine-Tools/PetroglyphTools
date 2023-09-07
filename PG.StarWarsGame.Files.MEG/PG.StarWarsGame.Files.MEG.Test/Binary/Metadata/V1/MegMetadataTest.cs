// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.V1.Metadata;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Metadata.V1;

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
            { new("123") });
        Assert.ThrowsException<ArgumentNullException>(() =>
            new MegMetadata(default, fileNameTable, null!));
    }

    [TestMethod]
    public void Ctor_Test__ThrowsArgumentException()
    {
        var header1 = new MegHeader(1, 1);
        var header2 = new MegHeader(2, 2);
        var fileTable1 = new MegFileTable(new List<MegFileContentTableRecord>
            { new(default, 0, 0, 0, 0) });
        var fileNameTable1 = new MegFileNameTable(new List<MegFileNameTableRecord>
            { new("123") });
        var fileNameTable2 = new MegFileNameTable(new List<MegFileNameTableRecord>
           {
               new("123"),
               new("456")
           });

        Assert.ThrowsException<ArgumentException>(() => new MegMetadata(header2, fileNameTable1, fileTable1));
        Assert.ThrowsException<ArgumentException>(() => new MegMetadata(header1, fileNameTable2, fileTable1));
    }


    [TestMethod]
    public void Ctor_Test__Correct()
    {
        new MegMetadata(
            new MegHeader(1, 1),
            new MegFileNameTable(new List<MegFileNameTableRecord> { new("123") }),
            new MegFileTable(new List<MegFileContentTableRecord> { default }));

        Assert.IsTrue(true);
    }

    [TestMethod]
    public void Test_Size()
    {
        var header = new MegHeader(1, 1);
        var fileNameTable = new MegFileNameTable(new List<MegFileNameTableRecord> { new("123") });
        var fileTable = new MegFileTable(new List<MegFileContentTableRecord> { default });

        var metadata = new MegMetadata(header, fileNameTable, fileTable);

        Assert.AreEqual(header.Size + fileNameTable.Size + fileTable.Size, metadata.Size);
    }

    [TestMethod]
    public void Test_Bytes()
    {
        var header = new MegHeader(1, 1);
        var fileNameTable = new MegFileNameTable(new List<MegFileNameTableRecord> { new("123") });
        var fileTable = new MegFileTable(new List<MegFileContentTableRecord> { default });

        var metadata = new MegMetadata(header, fileNameTable, fileTable);

        var expectedBytes = header.Bytes
            .Concat(fileNameTable.Bytes)
            .Concat(fileTable.Bytes)
            .ToArray();

        CollectionAssert.AreEqual(expectedBytes, metadata.Bytes);
    }
}