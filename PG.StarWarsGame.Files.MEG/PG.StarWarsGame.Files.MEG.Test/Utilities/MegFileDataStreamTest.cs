using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Utilities;

namespace PG.StarWarsGame.Files.MEG.Test.Utilities;

[TestClass]
public class MegFileDataStreamTest
{
    [TestMethod]
    public void Test_Ctor_Throws()
    {
        Assert.ThrowsException<ArgumentNullException>(() => new MegFileDataStream(null!, 0, 0));

        Assert.ThrowsException<ArgumentException>(() => new MegFileDataStream(new NonReadableStream(), 0, 0));
        Assert.ThrowsException<ArgumentException>(() => new MegFileDataStream(new NonSeekableStream(), 0, 0));


        Assert.ThrowsException<ArgumentException>(() => new MegFileDataStream(Stream.Null, 1, 0));
        Assert.ThrowsException<ArgumentException>(() => new MegFileDataStream(Stream.Null, 0, 1));
    }

    [TestMethod]
    public void Test_Ctor()
    {
        var ms = new MemoryStream(new byte[20]);
        var stream = new MegFileDataStream(ms, 0, 5);

        Assert.IsTrue(stream.CanRead);
        Assert.IsFalse(stream.CanSeek);
        Assert.IsFalse(stream.CanWrite);
        Assert.AreEqual(5, stream.Length);
    }

    [TestMethod]
    public void Test_NotSupportedOperations()
    {
        var ms = new MemoryStream();
        var stream = new MegFileDataStream(ms, 0, 0);

        Assert.ThrowsException<NotSupportedException>(() => stream.SetLength(1));
        Assert.ThrowsException<NotSupportedException>(() => stream.Position = 1);
        Assert.ThrowsException<NotSupportedException>(() => stream.Seek(1, SeekOrigin.Begin));
        Assert.ThrowsException<NotSupportedException>(() => stream.Write(new byte[1], 0, 0));

    }

    [TestMethod]
    public void Test_Dispose()
    {
        var ms = new MemoryStream();
        var stream = new MegFileDataStream(ms, 0, 0);

        stream.Dispose();
        Assert.ThrowsException<ObjectDisposedException>(() => ms.Position);
        Assert.ThrowsException<ObjectDisposedException>(() => stream.Read(Array.Empty<byte>(), 0, 0));

        // Double Dispose should not throw
        stream.Dispose();
    }

    [TestMethod]
    public void Test_Read_Throws()
    {
        var baseStream = new CustomStream();
        var stream = new MegFileDataStream( baseStream, 0, 0);

        Assert.ThrowsException<ArgumentNullException>(() => stream.Read(null!, 0, 0));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => stream.Read(Array.Empty<byte>(), -1, 0));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => stream.Read(Array.Empty<byte>(), 0, -1));

        baseStream.DoNotRead();
        Assert.ThrowsException<NotSupportedException>(() => stream.Read(Array.Empty<byte>(), 0, 0));
    }

    [TestMethod]
    public void Test_Read_ThrowsOutOfRange_Computed()
    {
        var baseStream = new MemoryStream(new byte[] { 1, 2, 3 });
        var stream = new MegFileDataStream(baseStream, 0, 3);

        var buffer = new byte[1];
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => stream.Read(buffer, 2, 0));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => stream.Read(buffer, 1, 1));
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => stream.Read(buffer, 0, 2));
    }

    [TestMethod]
    public void Test_Read_AllAtOnce()
    {
        // 0xFF represents data we should never read
        var source = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 1, 2, 3, 0xFF, 0xFF, 0xFF, 0xFF };
        var ms = new MemoryStream(source);

        var stream = new MegFileDataStream(ms, 4, 3);

        var data = new byte[] { 99, 99, 99, 99, 99 };
        Assert.AreEqual(3, stream.Read(data, 1, 4));
        CollectionAssert.AreEqual(new byte[] { 99, 1, 2, 3, 99 }, data);
    }

    [TestMethod]
    public void Test_CopyTo()
    {
        // 0xFF represents data we should never read
        var source = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 1, 2, 3, 0xFF, 0xFF, 0xFF, 0xFF };
        var ms = new MemoryStream(source);

        var stream = new MegFileDataStream(ms, 4, 3);

        var dataMs = new MemoryStream(new byte[4]);
        stream.CopyTo(dataMs);
        CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 0 }, dataMs.ToArray());
    }

    [TestMethod]
    public void Test_Read_BytePerByte()
    {
        // 0xFF represents data we should never read
        var source = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 1, 2, 3, 0xFF, 0xFF, 0xFF, 0xFF };
        var ms = new MemoryStream(source);

        var stream = new MegFileDataStream(ms, 4, 3);


        var data = new byte[] {99, 99, 99, 99, 99};
        Assert.AreEqual(0, stream.Position);
        stream.Read(data, 1, 1);
        Assert.AreEqual(1, stream.Position);
        stream.Read(data, 2, 1);
        Assert.AreEqual(2, stream.Position);
        stream.Read(data, 3, 1);
        Assert.AreEqual(3, stream.Position);

        // Goes out of bounds of the target data
        Assert.AreEqual(0, stream.Read(data, 3, 1));

        // Last value must not be 0xFF
        CollectionAssert.AreEqual(new byte[] { 99, 1, 2, 3, 99 }, data);
    }

    [TestMethod]
    public void Test_Read_SuddenCutOfData_Throws()
    {
        // 0xFF represents data we should never read
        var source = new byte[] { 1, 2, 3 };
        var ms = new MemoryStream(source);

        var stream = new MegFileDataStream(ms, 0, 3);


        var data = new byte[] { 99, 99, 99, 99, 99 };
        var n = stream.Read(data, 0, 1);
        Assert.AreEqual(1, n);

        ms.SetLength(1);

        Assert.ThrowsException<InvalidOperationException>(() => stream.Read(data, 0, 1));
    }

    [TestMethod]
    public void Test_Flush_NOP()
    {
        var ms = new MemoryStream();
        var stream = new MegFileDataStream(ms, 0, 0);

        stream.Flush();
    }

    private class NonReadableStream : Stream
    {
        public override void Flush() => throw new NotImplementedException();

        public override int Read(byte[] buffer, int offset, int count) => throw new NotImplementedException();

        public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

        public override void SetLength(long value) => throw new NotImplementedException();

        public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();

        public override bool CanRead => false;
        public override bool CanSeek => true;
        public override bool CanWrite { get; }
        public override long Length { get; }
        public override long Position { get; set; }
    }

    private class NonSeekableStream : Stream
    {
        public override void Flush() => throw new NotImplementedException();

        public override int Read(byte[] buffer, int offset, int count) => throw new NotImplementedException();

        public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

        public override void SetLength(long value) => throw new NotImplementedException();

        public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite { get; }
        public override long Length { get; }
        public override long Position { get; set; }
    }

    private class CustomStream : Stream
    {
        private bool _canRead = true;
        public override void Flush() => throw new NotImplementedException();

        public override int Read(byte[] buffer, int offset, int count) => throw new NotImplementedException();

        public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

        public override void SetLength(long value) => throw new NotImplementedException();

        public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();

        public override bool CanRead => _canRead;
        public override bool CanSeek => true;
        public override bool CanWrite { get; }
        public override long Length { get; }
        public override long Position { get; set; }

        public void DoNotRead()
        {
            _canRead = false;
        }
    }
}