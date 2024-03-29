// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.Metadata.V1;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Metadata.V1;

public class MegHeaderTest
{
    [Fact]
    public void Ctor_Test__ThrowsArgumentOORException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new MegHeader((uint)int.MaxValue + 1, (uint)int.MaxValue + 1));
    }

    [Fact]
    public void Ctor_Test__ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new MegHeader(1, 2));
    }

    [Fact]
    public void Ctor_Test__Correct()
    {
        new MegHeader(0, 0);
        new MegHeader(1, 1);
        new MegHeader(int.MaxValue, int.MaxValue);
        Assert.True(true);
    }

    [Fact]
    public void Ctor_Test__FileNumber()
    {
        IMegHeader header = new MegHeader(1, 1);
        Assert.Equal(1, header.FileNumber);
    }

    [Fact]
    public void Test_Size()
    {
        Assert.Equal(8, default(MegHeader).Size);
    }

    [Fact]
    public void Test_Bytes()
    {
        var header = new MegHeader(2, 2);
        var expectedBytes = new byte[]
        {
            0x2, 0x0, 0x0, 0x0,
            0x2, 0x0, 0x0, 0x0
        };
        Assert.Equal(expectedBytes, header.Bytes);
    }
}