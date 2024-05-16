using System;
using System.IO;
using PG.StarWarsGame.Files.MEG.Utilities;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Utilities;

public class MegFileDataStreamTest
{
    [Fact]
    public void Test_Ctor_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new MegFileDataStream("path", null!, 0, 0));
        Assert.Throws<ArgumentNullException>(() => new MegFileDataStream(null!, Stream.Null, 0, 0));
        Assert.Throws<ArgumentException>(() => new MegFileDataStream("", Stream.Null, 0, 0));

        Assert.Throws<ArgumentException>(() => new MegFileDataStream("path", new NonReadableStream(), 0, 0));
        Assert.Throws<ArgumentException>(() => new MegFileDataStream("path", new NonSeekableStream(), 0, 0));


        Assert.Throws<ArgumentException>(() => new MegFileDataStream("path", Stream.Null, 1, 0));
        Assert.Throws<ArgumentException>(() => new MegFileDataStream("path", Stream.Null, 0, 1));
    }

    [Fact]
    public void Test_Ctor()
    {
        var ms = new MemoryStream(new byte[20]);
        var stream = new MegFileDataStream("path", ms, 0, 5);

        Assert.Equal("path", stream.EntryPath);
        Assert.True(stream.CanRead);
        Assert.True(stream.CanSeek);
        Assert.False(stream.CanWrite);
        Assert.Equal(5, stream.Length);
    }

    [Fact]
    public void Test_NotSupportedOperations()
    {
        var ms = new MemoryStream();
        var stream = new MegFileDataStream("path", ms, 0, 0);

        Assert.Throws<NotSupportedException>(() => stream.SetLength(1));
        Assert.Throws<NotSupportedException>(() => stream.Write(new byte[1], 0, 0));

    }

    [Fact]
    public void Test_Dispose()
    {
        var ms = new MemoryStream();
        var stream = new MegFileDataStream("path", ms, 0, 0);

        stream.Dispose();
        Assert.Throws<ObjectDisposedException>(() => ms.Position);
        Assert.Throws<ObjectDisposedException>(() => stream.Read([], 0, 0));

        // Double Dispose should not throw
        stream.Dispose();
    }

    [Fact]
    public void Test_Read_Throws()
    {
        var baseStream = new CustomStream();
        var stream = new MegFileDataStream("path", baseStream, 0, 0);

        Assert.Throws<ArgumentNullException>(() => stream.Read(null!, 0, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => stream.Read([], -1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => stream.Read([], 0, -1));

        baseStream.DoNotRead();
        Assert.Throws<NotSupportedException>(() => stream.Read([], 0, 0));
    }

    [Fact]
    public void Test_Read_ThrowsOutOfRange_Computed()
    {
        var baseStream = new MemoryStream([1, 2, 3]);
        var stream = new MegFileDataStream("path", baseStream, 0, 3);

        var buffer = new byte[1];
        Assert.Throws<ArgumentOutOfRangeException>(() => stream.Read(buffer, 2, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => stream.Read(buffer, 1, 1));
        Assert.Throws<ArgumentOutOfRangeException>(() => stream.Read(buffer, 0, 2));
    }

    [Fact]
    public void Test_Read_AllAtOnce()
    {
        // 0xFF represents data we should never read
        var source = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 1, 2, 3, 0xFF, 0xFF, 0xFF, 0xFF };
        var ms = new MemoryStream(source);

        var stream = new MegFileDataStream("path", ms, 4, 3);

        var data = new byte[] { 99, 99, 99, 99, 99 };
        Assert.Equal(3, stream.Read(data, 1, 4));
        Assert.Equal([99, 1, 2, 3, 99], data);
    }

    [Fact]
    public void Test_CopyTo()
    {
        // 0xFF represents data we should never read
        var source = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 1, 2, 3, 0xFF, 0xFF, 0xFF, 0xFF };
        var ms = new MemoryStream(source);

        var stream = new MegFileDataStream("path", ms, 4, 3);

        var dataMs = new MemoryStream(new byte[4]);
        stream.CopyTo(dataMs);
        Assert.Equal([1, 2, 3, 0], dataMs.ToArray());
    }

    [Fact]
    public void Test_Read_BytePerByte()
    {
        // 0xFF represents data we should never read
        var source = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 1, 2, 3, 0xFF, 0xFF, 0xFF, 0xFF };
        var ms = new MemoryStream(source);

        var stream = new MegFileDataStream("path", ms, 4, 3);


        var data = new byte[] {99, 99, 99, 99, 99};
        Assert.Equal(0, stream.Position);
        stream.Read(data, 1, 1);
        Assert.Equal(1, stream.Position);
        stream.Read(data, 2, 1);
        Assert.Equal(2, stream.Position);
        stream.Read(data, 3, 1);
        Assert.Equal(3, stream.Position);

        // Goes out of bounds of the target data
        Assert.Equal(0, stream.Read(data, 3, 1));

        // Last value must not be 0xFF
        Assert.Equal([99, 1, 2, 3, 99], data);
    }

    [Fact]
    public void Test_Read_SuddenCutOfData_Throws()
    {
        // 0xFF represents data we should never read
        var source = new byte[] { 1, 2, 3 };
        var ms = new MemoryStream(source);

        var stream = new MegFileDataStream("path", ms, 0, 3);


        var data = new byte[] { 99, 99, 99, 99, 99 };
        var n = stream.Read(data, 0, 1);
        Assert.Equal(1, n);

        ms.SetLength(1);

        Assert.Throws<InvalidOperationException>(() => stream.Read(data, 0, 1));
    }


    [Fact]
    public void Test_Position()
    {
        var source = new byte[] { 9, 9, 9, 1, 2, 3, 9, 9, 9 };
        var ms = new MemoryStream(source);

        var stream = new MegFileDataStream("path", ms, 3, 3);

        Assert.Throws<ArgumentOutOfRangeException>(() => stream.Position = -1);

        stream.Position = 1;
        Assert.Equal(2, stream.ReadByte());
        stream.Position = 0;
        Assert.Equal(1, stream.ReadByte());
        stream.Position = 2;
        Assert.Equal(3, stream.ReadByte());
        stream.Position = 3;
        Assert.Equal(-1, stream.ReadByte());
    }

    [Fact]
    public void Test_Seek()
    {
        var source = new byte[] { 9, 9, 9, 1, 2, 3, 9, 9, 9 };
        var ms = new MemoryStream(source);

        var stream = new MegFileDataStream("path", ms, 3, 3);

        Assert.Throws<ArgumentOutOfRangeException>(() => stream.Seek(-1, SeekOrigin.Begin));

        Assert.Equal(1, stream.Seek(1, SeekOrigin.Begin));
        Assert.Equal(1, stream.Position);
        Assert.Equal(2, stream.ReadByte());

        Assert.Equal(0, stream.Seek(-2, SeekOrigin.Current));
        Assert.Equal(0, stream.Position);
        Assert.Equal(1, stream.ReadByte());

        stream.Position = 0;
        Assert.Equal(2, stream.Seek(2, SeekOrigin.Current));
        Assert.Equal(2, stream.Position);
        Assert.Equal(3, stream.ReadByte());

        Assert.Equal(3, stream.Seek(0, SeekOrigin.End));
        Assert.Equal(3, stream.Position);
        Assert.Equal(-1, stream.ReadByte());
    }

    [Fact]
    public void Test_Flush_NOP()
    {
        var ms = new MemoryStream();
        var stream = new MegFileDataStream("path", ms, 0, 0);

        stream.Flush();
    }

    [Fact]
    public void Test_EmptyDataStream()
    {
        var stream = MegFileDataStream.CreateEmptyStream("path");

        Assert.Equal("path", stream.EntryPath);
        Assert.Equal(0, stream.Length);

        var buffer = new byte[] { 1, 1, 1 };
        var read = stream.Read(buffer, 0, 3);

        Assert.Equal([1, 1, 1], buffer);
        Assert.Equal(0, read);

        stream.Dispose();
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