// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Test;
using PG.StarWarsGame.Files.MTD.Binary.File.Type.Definition;
using System;

namespace PG.StarWarsGame.Files.MTD.Test.Binary.File.Type.Definition
{
    [TestClass]
    public class MtdFileTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Ctor_Test__ThrowsArgumentException()
        {
            MtdFile _ = new MtdFile(new MtdImageTableRecord[] { });
        }

        [TestMethod]
        public void ToBytes_Test__AreBinaryEquivalent()
        {
            MtdFile mtdFile = new MtdFile(new[]
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
            byte[] actual = mtdFile.ToBytes();
            TestUtility.AssertAreBinaryEquivalent(TestConstants.MtdFileTestConstants.METADATA_TO_BYTES, actual);
        }
    }
}
