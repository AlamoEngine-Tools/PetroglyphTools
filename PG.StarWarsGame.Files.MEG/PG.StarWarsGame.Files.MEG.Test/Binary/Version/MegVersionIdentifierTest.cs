using System;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Binary;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Files;
using PG.Testing;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Version;

[TestClass]
public class MegVersionIdentifierTest
{
    private IServiceProvider _serviceProvider = null!;

    [TestInitialize]
    public void SetupServiceProvider()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(new MockFileSystem());
        _serviceProvider = sc.BuildServiceProvider();
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test__GetMegFileVersion_ThrowsArgNull()
    {
        new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(null!, out _);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Test__GetMegFileVersion_ThrowsArg()
    {
        new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(new NonSeekableStream(), out _);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Test__GetMegFileVersion_EmptyStream()
    {
        new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(new MemoryStream(Array.Empty<byte>()), out _);
    }

    [TestMethod]
    [ExpectedException(typeof(BinaryCorruptedException))]
    public void Test__GetMegFileVersion_InvalidFlags()
    {
        var data = new byte[]
        {
            0x77, 0x77, 0x77, 0x77,
            0xa4, 0x70, 0x7d, 0x3f
        };
        new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(new MemoryStream(data), out _);
    }

    [TestMethod]
    [ExpectedException(typeof(BinaryCorruptedException))]
    public void Test__GetMegFileVersion_InvalidId()
    {
        var data = new byte[]
        {
            0xff, 0xff, 0xff, 0xff,
            0xaa, 0x77, 0x77, 0x33
        };
        new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(new MemoryStream(data), out _);
    }

    /*
     * V1 Tests
     */

    [TestMethod]
    public void Test__GetMegFileVersion_V1_EmptyFile()
    {
        var data = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
        var version = new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(new MemoryStream(data), out var encrypted);

        Assert.AreEqual(MegFileVersion.V1, version);
        Assert.IsFalse(encrypted);
    }

    [TestMethod]
    public void Test__GetMegFileVersion_V1_SomeFileWithJunk()
    {
        var data = new byte[]
        {
            1, 0, 0, 0, 1, 0, 0, 0,
            1, 2, 3, 4, 5 // Junk
        };
        var version = new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(new MemoryStream(data), out var encrypted);

        Assert.AreEqual(MegFileVersion.V1, version);
        Assert.IsFalse(encrypted);
    }

    [TestMethod]
    public void Test__GetMegFileVersion_V1_NumFilesMatchesId()
    {
        var data = new byte[]
        {
            0xa4, 0x70, 0x7d, 0x3f, 0xa4, 0x70, 0x7d, 0x3f,
            1, 2, 3, 4, 5 // Junk
        };
        var version = new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(new MemoryStream(data), out var encrypted);

        Assert.AreEqual(MegFileVersion.V1, version);
        Assert.IsFalse(encrypted);
    }

    [TestMethod]
    public void Test__GetMegFileVersion_V1_NumFilesMatchesFlags()
    {
        var data = new byte[]
        {
            0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
            1, 2, 3, 4, 5 // Junk
        };
        var version = new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(new MemoryStream(data), out var encrypted);

        Assert.AreEqual(MegFileVersion.V1, version);
        Assert.IsFalse(encrypted);
    }

    [TestMethod]
    public void Test__GetMegFileVersion_V1_NumFilesMatchesEncryptedFlag()
    {
        var data = new byte[]
        {
            0xff, 0xff, 0xff, 0x8f, 0xff, 0xff, 0xff, 0x8f,
            1, 2, 3, 4, 5 // Junk
        };
        var version = new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(new MemoryStream(data), out var encrypted);

        Assert.AreEqual(MegFileVersion.V1, version);
        Assert.IsFalse(encrypted);
    }

    /*
     * V2 Tests
     */

    [TestMethod]
    [ExpectedException(typeof(BinaryCorruptedException))]
    public void Test__GetMegFileVersion_V2_ThrowsIncompleteData()
    {
        var data = new byte[]
        {
            0xff, 0xff, 0xff, 0xff,
            0xa4, 0x70, 0x7d, 0x3f
        };
        new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(new MemoryStream(data), out _);
    }

    [TestMethod]
    public void Test__GetMegFileVersion_V2_Empty()
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

        Assert.AreEqual(MegFileVersion.V2, version);
        Assert.IsFalse(encrypted);
    }

    [TestMethod]
    public void Test__GetMegFileVersion_V2_WithJunk()
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

        Assert.AreEqual(MegFileVersion.V2, version);
        Assert.IsFalse(encrypted);
    }

    [TestMethod]
    [ExpectedException(typeof(BinaryCorruptedException))]
    public void Test__GetMegFileVersion_V2_1File_ButCorrupted()
    {
        var data = new byte[]
        {
            0xff, 0xff, 0xff, 0xff,
            0xa4, 0x70, 0x7d, 0x3f,
            0x2c, 00, 00, 00,
            1, 0, 0, 0,
            1, 0, 0, 0
        };
        new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(new MemoryStream(data), out _);
    }

    [TestMethod]
    public void Test__GetMegFileVersion_V2_1FileEmpty()
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

        Assert.AreEqual(MegFileVersion.V2, version);
        Assert.IsFalse(encrypted);
    }

    [TestMethod]
    public void Test__GetMegFileVersion_V2_2Files()
    {
        var data = TestUtility.GetEmbeddedResource(typeof(MegVersionIdentifierTest), "Files.v2_2_files_data.meg");
        var version = new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(data, out var encrypted);

        Assert.AreEqual(MegFileVersion.V2, version);
        Assert.IsFalse(encrypted);
    }


    /*
     * V3 Tests (Unencrypted)
     */

    [TestMethod]
    public void Test__GetMegFileVersion_V3_EmptyFile()
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

        Assert.AreEqual(MegFileVersion.V3, version);
        Assert.IsFalse(encrypted);
    }

    [TestMethod]
    public void Test__GetMegFileVersion_V3_EmptyFile_Junk()
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

        Assert.AreEqual(MegFileVersion.V3, version);
        Assert.IsFalse(encrypted);
    }

    [TestMethod]
    public void Test__GetMegFileVersion_V3_1FileEmpty()
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

        Assert.AreEqual(MegFileVersion.V3, version);
        Assert.IsFalse(encrypted);
    }

    [TestMethod]
    [ExpectedException(typeof(BinaryCorruptedException))]
    public void Test__GetMegFileVersion_V3_CorruptedCauseEncryptFlagSet()
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
        new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(new MemoryStream(data), out _);
    }

    [TestMethod]
    [ExpectedException(typeof(BinaryCorruptedException))]
    public void Test__GetMegFileVersion_V3_1File_ButCorrupted()
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
        new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(new MemoryStream(data), out _);
    }

    [TestMethod]
    public void Test__GetMegFileVersion_V3_2Files()
    {
        var data = TestUtility.GetEmbeddedResource(typeof(MegVersionIdentifierTest), "Files.v3n_2_files_data.meg");
        var version = new MegVersionIdentifier(_serviceProvider).GetMegFileVersion(data, out var encrypted);

        Assert.AreEqual(MegFileVersion.V3, version);
        Assert.IsFalse(encrypted);
    }


    /*
     * V3 Tests (Encrypted)
     */

    [TestMethod]
    public void Test__GetMegFileVersion_V3_Enc_Empty()
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

        Assert.AreEqual(MegFileVersion.V3, version);
        Assert.IsTrue(encrypted);
    }

    [TestMethod]
    public void Test__GetMegFileVersion_V3_Enc_WithJunk()
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

        Assert.AreEqual(MegFileVersion.V3, version);
        Assert.IsTrue(encrypted);
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