// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Test;
using PG.StarWarsGame.Files.MTD.Binary.File.Type.Definition;

namespace PG.StarWarsGame.Files.MTD.Test.Binary.File.Type.Definition
{
    [TestClass]
    public class MtdImageTableTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Ctor_Test__ThrowsArgumentException()
        {
            MtdImageTable _ = new MtdImageTable(new MtdImageTableRecord[] { });
        }

        [TestMethod]
        public void ToBytes_Test__AreBinaryEquivalent()
        {
            MtdImageTable mtdImageTable = new MtdImageTable(new[]
            {
                new MtdImageTableRecord(TestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_NAME,
                    TestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_X_POSITION,
                    TestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_Y_POSITION,
                    TestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_X_EXTEND,
                    TestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_Y_EXTEND,
                    TestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_ALPHA),
                new MtdImageTableRecord(TestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_NAME,
                    TestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_X_POSITION,
                    TestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_Y_POSITION,
                    TestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_X_EXTEND,
                    TestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_Y_EXTEND,
                    TestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_ALPHA)
            });
            byte[] actual = mtdImageTable.ToBytes();
            Assert.AreEqual(actual.Length, 2 * TestConstants.MtdImageTableRecordTestConstants.OBJECT_SIZE_IN_BYTE);
            TestUtility.AssertAreBinaryEquivalent(TestConstants.MtdImageTableTestConstants.DEFAULT_TO_BYTES, actual);
        }

        [TestMethod]
        public void Size_Test__AsExpected()
        {
            MtdImageTable mtdImageTable = new MtdImageTable(new[]
            {
                new MtdImageTableRecord(TestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_NAME,
                    TestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_X_POSITION,
                    TestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_Y_POSITION,
                    TestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_X_EXTEND,
                    TestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_Y_EXTEND,
                    TestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_ALPHA),
                new MtdImageTableRecord(TestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_NAME,
                    TestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_X_POSITION,
                    TestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_Y_POSITION,
                    TestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_X_EXTEND,
                    TestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_Y_EXTEND,
                    TestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_ALPHA)
            });
            Assert.AreEqual(mtdImageTable.Size, 2 * TestConstants.MtdImageTableRecordTestConstants.OBJECT_SIZE_IN_BYTE);

        }
    }
}
