// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Metadata;

[TestClass]
public class MegFileNameTableRecordTest
{
    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    public void Ctor_Test__ThrowsArgumentException(string fileName)
    {
        Assert.ThrowsException<ArgumentException>(() => new MegFileNameTableRecord(fileName));
    }

    [TestMethod]
    public void Ctor_Test__ThrowsOverflowException()
    {
        var fn = new string('a', ushort.MaxValue + 1);
        Assert.ThrowsException<OverflowException>(() => new MegFileNameTableRecord(fn));
    }

    [TestMethod]
    [DataRow("abc", 2 + 3)]
    [DataRow("abc123", 2 + 6)]
    [DataRow("üöä", 2 + 3)]
    [DataRow("©", 2 + 1)]
    [DataRow("🍔", 2 + 2)] // Long byte emojii
    [DataRow("❓", 2 + 1)] // Short byte emojii
    public void Ctor_Test_Size(string fileName, int expectedSize)
    {
        var record = new MegFileNameTableRecord(fileName);
        Assert.AreEqual(expectedSize, record.Size);
    }

    [TestMethod]
    [DataRow("abc", "abc")]
    [DataRow("abc_def", "abc_def")]
    [DataRow("abc123", "abc123")]
    [DataRow("üöä", "???")]
    [DataRow("©", "?")]
    [DataRow("🍔", "??")] // Long byte emojii
    [DataRow("❓", "?")] // Short byte emojii
    [DataRow("a\u0160", "a?")] // ALT+0160
    public void Test_FileNameGetEncoded(string fileName, string expectedName)
    {
        var record = new MegFileNameTableRecord(fileName);
        Assert.AreEqual(expectedName, record.FileName);
        Assert.AreEqual(fileName, record.OriginalFilePath);
    }

    [TestMethod]
    [DataRow("a", new byte[] { 0x1, 0x0, 0x61 })]
    [DataRow("ab", new byte[] { 0x2, 0x0, 0x61, 0x62 })]
    [DataRow("aü", new byte[] { 0x2, 0x0, 0x61, 0x3F })]
    [DataRow("aß", new byte[] { 0x2, 0x0, 0x61, 0x3F })]
    public void Test_GetBytes(string fileName, byte[] expectedBytes)
    {
        var record = new MegFileNameTableRecord(fileName);
        CollectionAssert.AreEqual(expectedBytes, record.Bytes);
    }
}