// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MTD.Binary.File.Type.Definition;
using PG.StarWarsGame.Files.MTD.Commons.Exceptions;

namespace PG.StarWarsGame.Files.MTD.Test.Binary.File.Type.Definition;

[TestClass]
public class MtdImageTableRecordTest
{
    private static MtdImageTableRecord s_defaultRecord;


    [TestInitialize]
    public void TestInitialize()
    {
        s_defaultRecord = new MtdImageTableRecord(MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_NAME,
            MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_X_POSITION,
            MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_Y_POSITION, MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_X_EXTEND,
            MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_Y_EXTEND,
            MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_ALPHA);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("    ")]
    [DataRow("  \r\n  ")]
    [DataRow("  \t  \r\n  ")]
    [ExpectedException(typeof(ArgumentException))]
    public void Ctor_Test__ThrowsArgumentException(string inputName)
    {
        var _ = new MtdImageTableRecord(inputName, 0, 0, 0, 0, false);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidIconNameException))]
    public void Ctor_Test__ThrowsInvalidIconNameException()
    {
        var _ = new MtdImageTableRecord(TestUtility.GetRandomStringOfLength(128), 0, 0, 0, 0, false);
    }

    [TestMethod]
    public void Ctor_Test__IsBinaryEquivalentToExpected()
    {
        var actual = new MtdImageTableRecord(MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_NAME,
            MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_X_POSITION,
            MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_Y_POSITION, MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_X_EXTEND,
            MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_Y_EXTEND,
            MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_ALPHA).ToBytes();
        Assert.AreEqual(MtdTestConstants.MtdImageTableRecordTestConstants.OBJECT_SIZE_IN_BYTE, actual.Length);
        TestUtility.AssertAreBinaryEquivalent(MtdTestConstants.MtdImageTableRecordTestConstants.EXPECTED_MTD_IMAGE_TABLE_RECORD_AS_BYTES, actual);
    }

    [TestMethod]
    public void Size_Test__AsExpected()
    {
        Assert.AreEqual(MtdTestConstants.MtdImageTableRecordTestConstants.OBJECT_SIZE_IN_BYTE, s_defaultRecord.Size);
    }
}
