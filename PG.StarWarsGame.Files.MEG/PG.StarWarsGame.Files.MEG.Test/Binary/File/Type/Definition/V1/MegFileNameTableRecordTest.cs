// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Binary.V1.Metadata;
using PG.Testing;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.File.Type.Definition.V1;

[TestClass]
public class MegFileNameTableRecordTest
{
    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [ExpectedException(typeof(ArgumentException))]
    public void Ctor_Test__ThrowsArgumentException(string fileName)
    {
        new MegFileNameTableRecord(fileName, Encoding.ASCII);
    }

    [TestMethod]
    [ExpectedException(typeof(OverflowException))]
    public void Ctor_Test__ThrowsOverflowException()
    {
        var fn = TestUtility.GetRandomStringOfLength(ushort.MaxValue + 1);
        new MegFileNameTableRecord(fn, Encoding.ASCII);
    }

    [TestMethod]
    public void Ctor_Test__ThrowsNotSupportedException_Encoding()
    {
        Assert.ThrowsException<NotSupportedException>(() => new MegFileNameTableRecord("someName", Encoding.Unicode));
        Assert.ThrowsException<NotSupportedException>(() => new MegFileNameTableRecord("someName", Encoding.UTF32));
        Assert.ThrowsException<NotSupportedException>(() => new MegFileNameTableRecord("someName", Encoding.UTF8));
    }

    [TestMethod]
    [DataRow("abc", 2 + 3)]
    [DataRow("abc123", 2 + 6)]
    [DataRow("üöä", 2 + 3)]
    [DataRow("©", 2 + 1)]
    [DataRow("🍔", 2 + 2)] // Long byte emojii
    [DataRow("❓", 2 + 1)] // Short byte emojii
    public void Ctor_Test__CorrectSize(string fileName, int expectedSize)
    {
        var record = new MegFileNameTableRecord(fileName, Encoding.ASCII);
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
    [DataRow("a ", "a?")] // ALT+0160
    public void Test__FileNameWithEncoding(string fileName, string expectedName)
    {
        var record = new MegFileNameTableRecord(fileName, Encoding.ASCII);
        Assert.AreEqual(expectedName, record.FileName);
    }
}