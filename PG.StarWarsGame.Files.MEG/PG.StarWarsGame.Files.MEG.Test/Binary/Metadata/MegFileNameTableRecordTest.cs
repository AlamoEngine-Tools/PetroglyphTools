// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.Testing;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Metadata;

[TestClass]
public class MegFileNameTableRecordTest
{
    [TestMethod]
    [DataRow("", "org")]
    [DataRow("   ", "org")]
    [DataRow("path", "")]
    public void Ctor_Test__ThrowsArgumentException(string fileName, string originalFileName)
    {
        Assert.ThrowsException<ArgumentException>(() => new MegFileNameTableRecord(fileName, originalFileName));
    }

    [TestMethod]
    public void Ctor_Test__ThrowsArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new MegFileNameTableRecord(null!, "org"));
        Assert.ThrowsException<ArgumentNullException>(() => new MegFileNameTableRecord("path", null!));
    }

    [TestMethod]
    public void Ctor_Test__ThrowsArgumentException()
    {
        var fn = new string('a', ushort.MaxValue + 1);
        Assert.ThrowsException<ArgumentException>(() => new MegFileNameTableRecord(fn, "org"));
    }

    [TestMethod]
    public void Ctor_Test_OriginalPath()
    {
        const string expectedOrgPath = "someUnusualStringÜöä😅";
        var record = ExceptionUtilities.AssertDoesNotThrowException(() => new MegFileNameTableRecord("path", expectedOrgPath));
        Assert.AreEqual("path", record.FileName);
        Assert.AreEqual(expectedOrgPath, record.OriginalFilePath);
    }

    [TestMethod]
    [DataRow("abc", 2 + 3)]
    [DataRow("abc123", 2 + 6)]
    public void Ctor_Test_Size(string fileName, int expectedSize)
    {
        var record = new MegFileNameTableRecord(fileName, "org");
        Assert.AreEqual(expectedSize, record.Size);
    }

    [TestMethod]
    [DataRow("üöä")]
    [DataRow("©")]
    [DataRow("🍔")] // Long byte emojii
    [DataRow("❓")] // Short byte emojii
    [DataRow("a\u00A0")] 
    public void Test_NonAsciiPath_Throws(string fileName)
    {
        Assert.ThrowsException<ArgumentException>(() => new MegFileNameTableRecord(fileName, "org"));
    }

    [TestMethod]
    [DataRow("a", new byte[] { 0x1, 0x0, 0x61 })]
    [DataRow("ab", new byte[] { 0x2, 0x0, 0x61, 0x62 })]
    public void Test_GetBytes(string fileName, byte[] expectedBytes)
    {
        var record = new MegFileNameTableRecord(fileName, "org");
        CollectionAssert.AreEqual(expectedBytes, record.Bytes);
    }

    internal static MegFileNameTableRecord CreateNameRecord(string path, string? orgPath = null)
    {
        return new MegFileNameTableRecord(path, orgPath ?? path);
    }
}