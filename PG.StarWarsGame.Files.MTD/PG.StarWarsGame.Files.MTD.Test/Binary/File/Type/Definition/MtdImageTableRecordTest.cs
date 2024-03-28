// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.MTD.Binary.File.Type.Definition;
using PG.StarWarsGame.Files.MTD.Commons.Exceptions;
using PG.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MTD.Test.Binary.File.Type.Definition;

public class MtdImageTableRecordTest
{
    private static MtdImageTableRecord s_defaultRecord;

    public MtdImageTableRecordTest()
    {
        s_defaultRecord = new MtdImageTableRecord(MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_NAME,
            MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_X_POSITION,
            MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_Y_POSITION, MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_X_EXTEND,
            MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_Y_EXTEND,
            MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_ALPHA);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData("  \r\n  ")]
    [InlineData("  \t  \r\n  ")]
    public void Ctor_Test__ThrowsArgumentException(string inputName)
    {
        Assert.Throws<ArgumentException>(() => new MtdImageTableRecord(inputName, 0, 0, 0, 0, false));
    }

    [Fact]
    public void Ctor_Test__ThrowsInvalidIconNameException()
    {
        Assert.Throws<InvalidIconNameException>(() => new MtdImageTableRecord(TestUtility.GetRandomStringOfLength(128), 0, 0, 0, 0, false));
    }

    [Fact]
    public void Ctor_Test__IsBinaryEquivalentToExpected()
    {
        var actual = new MtdImageTableRecord(MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_NAME,
            MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_X_POSITION,
            MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_Y_POSITION, MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_X_EXTEND,
            MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_Y_EXTEND,
            MtdTestConstants.MtdImageTableRecordTestConstants.DEFAULT_TEST_RECORD_ALPHA).ToBytes();
        Assert.Equal(MtdTestConstants.MtdImageTableRecordTestConstants.OBJECT_SIZE_IN_BYTE, actual.Length);
        TestUtility.AssertAreBinaryEquivalent(MtdTestConstants.MtdImageTableRecordTestConstants.EXPECTED_MTD_IMAGE_TABLE_RECORD_AS_BYTES, actual);
    }

    [Fact]
    public void Size_Test__AsExpected()
    {
        Assert.Equal(MtdTestConstants.MtdImageTableRecordTestConstants.OBJECT_SIZE_IN_BYTE, s_defaultRecord.Size);
    }
}
