﻿using System;
using PG.StarWarsGame.Files.DAT.Binary.Metadata;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Binary.Metadata;

public class DatHeaderTest
{
    [Theory]
    [InlineData(0u)]
    [InlineData(1u)]
    [InlineData(100u)]
    public void Ctor(uint number)
    {
        var header = new DatHeader(number);
        Assert.Equal(number, header.RecordCount);
        Assert.Equal(sizeof(uint), header.Size);
        Assert.Equal(BitConverter.GetBytes(header.RecordCount), header.Bytes);

        var buffer = new byte[header.Size];
        buffer.AsSpan().Fill(1);

        header.GetBytes(buffer);

        Assert.Equal(BitConverter.GetBytes(header.RecordCount), buffer);
    }

    [Fact]
    public void Ctor_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new DatHeader((uint)int.MaxValue + 1));
    }
}