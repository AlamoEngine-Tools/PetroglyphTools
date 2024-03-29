// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.MTD.Binary.File.Type.Definition;
using PG.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MTD.Test.Binary.File.Type.Definition;

public class MtdImageTableTest
{
    [Fact]
    public void Ctor_Test__ThrowsArgumentException()
    {
        var mtdImageTable = new MtdImageTable(new[]
        {
            new MtdImageTableRecord(MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_NAME,
                MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_X_POSITION,
                MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_Y_POSITION,
                MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_X_EXTEND,
                MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_Y_EXTEND,
                MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_ALPHA),
            new MtdImageTableRecord(MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_NAME,
                MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_X_POSITION,
                MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_Y_POSITION,
                MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_X_EXTEND,
                MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_Y_EXTEND,
                MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_ALPHA)
        });
        var actual = mtdImageTable.ToBytes();
        var _ = new MtdImageTable(Array.Empty<MtdImageTableRecord>());
        Assert.Equal(actual.Length, 2 * MtdTestConstants.MtdImageTableRecordTestConstants.OBJECT_SIZE_IN_BYTE);
        TestUtility.AssertAreBinaryEquivalent(MtdTestConstants.MtdImageTableTestConstants.DEFAULT_TO_BYTES, actual);
    }

    [Fact]
    public void ToBytes_Test__AreBinaryEquivalent()
    {
        var mtdFile = new MtdFile(new[]
        {
            new MtdImageTableRecord(MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_NAME,
                MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_X_POSITION,
                MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_Y_POSITION,
                MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_X_EXTEND,
                MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_Y_EXTEND,
                MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_ALPHA),
            new MtdImageTableRecord(MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_NAME,
                MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_X_POSITION,
                MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_Y_POSITION,
                MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_X_EXTEND,
                MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_Y_EXTEND,
                MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_ALPHA)
        });
        var actual = mtdFile.ToBytes();
        TestUtility.AssertAreBinaryEquivalent(MtdTestConstants.MtdFileTestConstants.METADATA_TO_BYTES, actual);
    }

    [Fact]
    public void Size_Test__AsExpected()
    {
        var mtdImageTable = new MtdImageTable(new[]
        {
            new MtdImageTableRecord(MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_NAME,
                MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_X_POSITION,
                MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_Y_POSITION,
                MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_X_EXTEND,
                MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_Y_EXTEND,
                MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_ALPHA),
            new MtdImageTableRecord(MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_NAME,
                MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_X_POSITION,
                MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_Y_POSITION,
                MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_X_EXTEND,
                MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_Y_EXTEND,
                MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_ALPHA)
        });
        Assert.Equal(mtdImageTable.Size, 2 * MtdTestConstants.MtdImageTableRecordTestConstants.OBJECT_SIZE_IN_BYTE);

    }
}
