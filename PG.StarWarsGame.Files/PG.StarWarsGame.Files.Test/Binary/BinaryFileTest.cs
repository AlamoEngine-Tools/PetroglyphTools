using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PG.StarWarsGame.Files.Binary;
using Xunit;

namespace PG.StarWarsGame.Files.Test.Binary;

public class BinaryFileTest
{
    [Fact]
    public void WriteTo_NullArg_Throws()
    {
        var binary = new TestBinaryFile(new TestBinary([1, 2, 3, 4]));
        Assert.Throws<ArgumentNullException>(() => binary.WriteTo(null!));
    }

    [Fact]
    public void WriteTo()
    {
        var random = new Random();
        var numBins = random.Next(0, 10);

        var bins = new List<IBinary>();
        for (var i = 0; i < numBins; i++)
        {
            var bn = random.Next(1, 20);
            var bytes = new byte[bn];
            random.NextBytes(bytes);
            bins.Add(new TestBinary(bytes));
        }

        var binary = new TestBinaryFile(bins.ToArray());

        var ms = new MemoryStream();

        binary.WriteTo(ms);

        Assert.Equal(bins.SelectMany(x => x.Bytes), ms.ToArray());
    }
}