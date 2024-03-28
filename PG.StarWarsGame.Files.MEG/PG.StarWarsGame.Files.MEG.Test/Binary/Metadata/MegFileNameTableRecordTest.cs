// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Metadata;

public class MegFileNameTableRecordTest
{
    [Theory]
    [InlineData("", "org")]
    [InlineData("   ", "org")]
    [InlineData("path", "")]
    public void Ctor_Test_ThrowsArgumentException(string fileName, string originalFileName)
    {
        Assert.Throws<ArgumentException>(() => new MegFileNameTableRecord(fileName, originalFileName));
    }

    [Fact]
    public void Ctor_Test_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new MegFileNameTableRecord(null!, "org"));
        Assert.Throws<ArgumentNullException>(() => new MegFileNameTableRecord("path", null!));
    }

    [Fact]
    public void Ctor_Test_StringTooLong_ThrowsArgumentException()
    {
        var fn = new string('a', ushort.MaxValue + 1);
        Assert.Throws<ArgumentException>(() => new MegFileNameTableRecord(fn, "org"));
    }

    [Fact]
    public void Ctor_Test_OriginalPath()
    {
        const string expectedOrgPath = "someUnusualStringÜöä😅";
        var record = ExceptionUtilities.AssertDoesNotThrowException(() => new MegFileNameTableRecord("path", expectedOrgPath));
        Assert.Equal("path", record.FileName);
        Assert.Equal(expectedOrgPath, record.OriginalFilePath);
    }

    [Theory]
    [InlineData("abc", 2 + 3)]
    [InlineData("abc123", 2 + 6)]
    public void Ctor_Test_Size(string fileName, int expectedSize)
    {
        var record = new MegFileNameTableRecord(fileName, "org");
        Assert.Equal(expectedSize, record.Size);
    }

    [Theory]
    [InlineData("üöä")]
    [InlineData("©")]
    [InlineData("🍔")] // Long byte emojii
    [InlineData("❓")] // Short byte emojii
    [InlineData("a\u00A0")] 
    public void Test_NonAsciiPath_Throws(string fileName)
    {
        Assert.Throws<ArgumentException>(() => new MegFileNameTableRecord(fileName, "org"));
    }

    [Theory]
    [InlineData("a", new byte[] { 0x1, 0x0, 0x61 })]
    [InlineData("ab", new byte[] { 0x2, 0x0, 0x61, 0x62 })]
    public void Test_GetBytes(string fileName, byte[] expectedBytes)
    {
        var record = new MegFileNameTableRecord(fileName, "org");
        Assert.Equal(expectedBytes, record.Bytes);
    }

    internal static MegFileNameTableRecord CreateNameRecord(string path, string? orgPath = null)
    {
        return new MegFileNameTableRecord(path, orgPath ?? path);
    }
}