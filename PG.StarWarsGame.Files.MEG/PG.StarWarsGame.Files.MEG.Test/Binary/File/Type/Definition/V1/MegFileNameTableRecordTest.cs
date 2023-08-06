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
    [DataRow("    ")]
    [DataRow("  \n\t  ")]
    [DataRow("  \r\n  \n  ")]
    [ExpectedException(typeof(ArgumentException))]
    public void Ctor_Test__ThrowsArgumentException(string fileName)
    {
        var _ = new MegFileNameTableRecord(fileName, Encoding.ASCII);
    }

    [TestMethod]
    [ExpectedException(typeof(OverflowException))]
    public void Ctor_Test__ThrowsOverflowException()
    {
        var fn = TestUtility.GetRandomStringOfLength(ushort.MaxValue + 5);
        var _ = new MegFileNameTableRecord(fn, Encoding.ASCII);
    }
}
