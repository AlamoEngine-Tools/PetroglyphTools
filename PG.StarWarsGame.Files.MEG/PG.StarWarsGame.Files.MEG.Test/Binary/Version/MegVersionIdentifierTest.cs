using System;
using System.IO;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Files;
using PG.Testing;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Version;

public class MegVersionIdentifierTest
{
    private readonly IServiceProvider _serviceProvider;

    public MegVersionIdentifierTest()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(new MockFileSystem());
        _serviceProvider = sc.BuildServiceProvider();
    }

    [Fact]
    public void GetMegFileVersion_ThrowsArgNull()
    {
        Assert.Throws<ArgumentNullException>(() => new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(null!, out _));
    }

    [Fact]
    public void GetMegFileVersion_ThrowsArg()
    {
        Assert.Throws<ArgumentException>(() => new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(new NonSeekableStream(), out _));
    }

    [Fact]
    public void GetMegFileVersion_EmptyStream()
    {
        Assert.Throws<ArgumentException>(() => new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(new MemoryStream([]), out _));
    }

    [Fact]
    public void GetMegFileVersion_InvalidFlags()
    {
        var data = new byte[]
        {
            0x77, 0x77, 0x77, 0x77,
            0xa4, 0x70, 0x7d, 0x3f
        };
        Assert.Throws<BinaryCorruptedException>(() => new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(new MemoryStream(data), out _));
    }

    [Fact]
    public void GetMegFileVersion_InvalidId()
    {
        var data = new byte[]
        {
            0xff, 0xff, 0xff, 0xff,
            0xaa, 0x77, 0x77, 0x33
        };
        Assert.Throws<BinaryCorruptedException>(() => new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(new MemoryStream(data), out _));
    }

    /*
     * V1 Tests
     */

    [Fact]
    public void GetMegFileVersion_V1_EmptyFile()
    {
        var data = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
        var version = new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(new MemoryStream(data), out var encrypted);

        Assert.Equal(MegFileVersion.V1, version);
        Assert.False(encrypted);
    }

    [Fact]
    public void GetMegFileVersion_V1_SomeFileWithJunk()
    {
        var data = new byte[]
        {
            1, 0, 0, 0, 1, 0, 0, 0,
            1, 2, 3, 4, 5 // Junk
        };
        var version = new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(new MemoryStream(data), out var encrypted);

        Assert.Equal(MegFileVersion.V1, version);
        Assert.False(encrypted);
    }

    [Fact]
    public void GetMegFileVersion_V1_NumFilesMatchesId()
    {
        var data = new byte[]
        {
            0xa4, 0x70, 0x7d, 0x3f, 0xa4, 0x70, 0x7d, 0x3f,
            1, 2, 3, 4, 5 // Junk
        };
        var version = new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(new MemoryStream(data), out var encrypted);

        Assert.Equal(MegFileVersion.V1, version);
        Assert.False(encrypted);
    }

    [Fact]
    public void GetMegFileVersion_V1_NumFilesMatchesFlags()
    {
        var data = new byte[]
        {
            0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
            1, 2, 3, 4, 5 // Junk
        };
        var version = new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(new MemoryStream(data), out var encrypted);

        Assert.Equal(MegFileVersion.V1, version);
        Assert.False(encrypted);
    }

    [Fact]
    public void GetMegFileVersion_V1_NumFilesMatchesEncryptedFlag()
    {
        var data = new byte[]
        {
            0xff, 0xff, 0xff, 0x8f, 0xff, 0xff, 0xff, 0x8f,
            1, 2, 3, 4, 5 // Junk
        };
        var version = new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(new MemoryStream(data), out var encrypted);

        Assert.Equal(MegFileVersion.V1, version);
        Assert.False(encrypted);
    }

    /*
     * V2 Tests
     */

    [Fact]
    public void GetMegFileVersion_V2_ThrowsIncompleteData()
    {
        var data = new byte[]
        {
            0xff, 0xff, 0xff, 0xff,
            0xa4, 0x70, 0x7d, 0x3f
        };
        Assert.Throws<BinaryCorruptedException>(() => new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(new MemoryStream(data), out _));
    }

    [Fact]
    public void GetMegFileVersion_V2_Empty()
    {
        var data = new byte[]
        {
            0xff, 0xff, 0xff, 0xff,
            0xa4, 0x70, 0x7d, 0x3f,
            0,0,0,0,
            0,0,0,0,
            0,0,0,0,
        };
        var version = new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(new MemoryStream(data), out var encrypted);

        Assert.Equal(MegFileVersion.V2, version);
        Assert.False(encrypted);
    }

    [Fact]
    public void GetMegFileVersion_V2_WithJunk()
    {
        var data = new byte[]
        {
            0xff, 0xff, 0xff, 0xff,
            0xa4, 0x70, 0x7d, 0x3f,
            0,0,0,0,
            0,0,0,0,
            0,0,0,0,
            1,2,3,4 // Some junk 
        };
        var version = new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(new MemoryStream(data), out var encrypted);

        Assert.Equal(MegFileVersion.V2, version);
        Assert.False(encrypted);
    }

    [Fact]
    public void GetMegFileVersion_V2_1File_ButCorrupted()
    {
        var data = new byte[]
        {
            0xff, 0xff, 0xff, 0xff,
            0xa4, 0x70, 0x7d, 0x3f,
            0x2c, 00, 00, 00,
            1, 0, 0, 0,
            1, 0, 0, 0
        };
        Assert.Throws<BinaryCorruptedException>(() => new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(new MemoryStream(data), out _));
    }

    [Fact]
    public void GetMegFileVersion_V2_1FileEmpty()
    {
        var data = new byte[]
        {
            0xff, 0xff, 0xff, 0xff,
            0xa4, 0x70, 0x7d, 0x3f,
            0x2b, 00, 00, 00,
            1, 0, 0, 0,
            1, 0, 0, 0,
            1, 0,
            45,
            0x92, 0x5A, 0xB4, 0xD4,
            0,0,0,0,
            0,0,0,0,
            0x2b,0,0,0,
            0,0,0,0,
        };
        var version = new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(new MemoryStream(data), out var encrypted);

        Assert.Equal(MegFileVersion.V2, version);
        Assert.False(encrypted);
    }

    [Fact]
    public void GetMegFileVersion_V2_2Files()
    {
        var data = TestUtility.GetEmbeddedResource(typeof(MegVersionIdentifierTest), "Files.v2_2_files_data.meg");
        var version = new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(data, out var encrypted);

        Assert.Equal(MegFileVersion.V2, version);
        Assert.False(encrypted);
    }


    /*
     * V3 Tests (Unencrypted)
     */

    [Fact]
    public void GetMegFileVersion_V3_EmptyFile()
    {
        var data = new byte[]
        {
            0xff, 0xff, 0xff, 0xff,
            0xa4, 0x70, 0x7d, 0x3f,
            0,0,0,0,
            0,0,0,0,
            0,0,0,0,
            0,0,0,0,
        };
        var version = new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(new MemoryStream(data), out var encrypted);

        Assert.Equal(MegFileVersion.V3, version);
        Assert.False(encrypted);
    }

    [Fact]
    public void GetMegFileVersion_V3_EmptyFile_Junk()
    {
        var data = new byte[]
        {
            0xff, 0xff, 0xff, 0xff,
            0xa4, 0x70, 0x7d, 0x3f,
            0,0,0,0,
            0,0,0,0,
            0,0,0,0,
            0,0,0,0,
            1,2,3,4
        };
        var version = new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(new MemoryStream(data), out var encrypted);

        Assert.Equal(MegFileVersion.V3, version);
        Assert.False(encrypted);
    }

    [Fact]
    public void GetMegFileVersion_V3_1FileEmpty()
    {
        var data = new byte[]
        {
            0xff, 0xff, 0xff, 0xff,
            0xa4, 0x70, 0x7d, 0x3f,
            0x2f, 00, 00, 00,
            1, 0, 0, 0,
            1, 0, 0, 0,
            3, 0, 0, 0,
            1, 0,
            45,
            0, 0,
            0x92, 0x5A, 0xB4, 0xD4,
            0,0,0,0,
            0,0,0,0,
            0x2f,0,0,0,
            0,0,
        };
        var version = new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(new MemoryStream(data), out var encrypted);

        Assert.Equal(MegFileVersion.V3, version);
        Assert.False(encrypted);
    }

    [Fact]
    public void GetMegFileVersion_V3_CorruptedCauseEncryptFlagSet()
    {
        var data = new byte[]
        {
            0xff, 0xff, 0xff, 0xff,
            0xa4, 0x70, 0x7d, 0x3f,
            0x2f, 00, 00, 00,
            1, 0, 0, 0,
            1, 0, 0, 0,
            3, 0, 0, 0,
            1, 0,
            45,
            1, 0,
            0x92, 0x5A, 0xB4, 0xD4,
            0,0,0,0,
            0,0,0,0,
            0x2f,0,0,0,
            0,0,
        };
        Assert.Throws<BinaryCorruptedException>(() => new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(new MemoryStream(data), out _));
    }

    [Fact]
    public void GetMegFileVersion_V3_1File_ButCorrupted()
    {
        var data = new byte[]
        {
            0xff, 0xff, 0xff, 0xff,
            0xa4, 0x70, 0x7d, 0x3f,
            0x2f, 00, 00, 00,
            1, 0, 0, 0,
            1, 0, 0, 0,
            3, 0, 0, 0
        };
        Assert.Throws<BinaryCorruptedException>(() => new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(new MemoryStream(data), out _));
    }

    [Fact]
    public void GetMegFileVersion_V3_2Files()
    {
        var data = TestUtility.GetEmbeddedResource(typeof(MegVersionIdentifierTest), "Files.v3n_2_files_data.meg");
        var version = new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(data, out var encrypted);

        Assert.Equal(MegFileVersion.V3, version);
        Assert.False(encrypted);
    }


    /*
     * V3 Tests (Encrypted)
     */

    [Fact]
    public void GetMegFileVersion_V3_Enc_Empty()
    {
        var data = new byte[]
        {
            0xff, 0xff, 0xff, 0x8f,
            0xa4, 0x70, 0x7d, 0x3f,
            0,0,0,0,
            0,0,0,0,
            0,0,0,0,
            0,0,0,0,
        };
        var version = new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(new MemoryStream(data), out var encrypted);

        Assert.Equal(MegFileVersion.V3, version);
        Assert.True(encrypted);
    }

    [Fact]
    public void GetMegFileVersion_V3_Enc_WithJunk()
    {
        var data = new byte[]
        {
            0xff, 0xff, 0xff, 0x8f,
            0xa4, 0x70, 0x7d, 0x3f,
            0xFF,0,0,0,
            2,0,0,0,
            2,0,0,0,
            0xAA,0,0,0,
            1,2,3,4, // Junk
        };
        var version = new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(new MemoryStream(data), out var encrypted);

        Assert.Equal(MegFileVersion.V3, version);
        Assert.True(encrypted);
    }


    private class NonSeekableStream : Stream
    {
        public override void Flush() => throw new NotImplementedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();
        public override void SetLength(long value) => throw new NotImplementedException();
        public override int Read(byte[] buffer, int offset, int count) => throw new NotImplementedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => 1;
        public override long Position { get; set; }
    }
}