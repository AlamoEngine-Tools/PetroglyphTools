// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.MTD.Binary.File.Type.Definition;
using PG.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MTD.Test.Binary.File.Type.Definition;

public class MtdHeaderTest
{
    private const int EXPECTED_SIZE = sizeof(uint);

    [Theory(Skip = "Yes")]
    [InlineData(0u)]
    [InlineData(12345u)]
    [InlineData(uint.MaxValue)]
    public void ToBytes_Test__AreBinaryEquivalent(uint input)
    {
        var header = new MtdHeader(input);
        var actual = header.ToBytes();
        TestUtility.AssertAreBinaryEquivalent(BitConverter.GetBytes(input), actual);
    }

    [Fact]
    public void Size_Test_AsExpected()
    {
        Assert.Equal(EXPECTED_SIZE, new MtdHeader().Size);
    }
}
