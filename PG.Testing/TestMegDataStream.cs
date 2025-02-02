using System.IO;
using PG.Commons.Data;

namespace PG.Testing;

public class TestMegDataStream : Stream, IMegFileDataStream
{
    private readonly Stream _innerStream;

    public override bool CanRead => _innerStream.CanRead;
    public override bool CanSeek => _innerStream.CanSeek;
    public override bool CanWrite => _innerStream.CanWrite;
    public override long Length => _innerStream.Length;
    public override long Position
    {
        get => _innerStream.Position;
        set => _innerStream.Position = value;
    }

    public TestMegDataStream(string entryPath, byte[] data)
    {
        EntryPath = entryPath;
        _innerStream = new MemoryStream(data);
    }

    public TestMegDataStream(string entryPath, Stream dataStream)
    {
        EntryPath = entryPath;
        _innerStream = dataStream;
    }

    public string EntryPath { get; }


    public override void Flush()
    {
        _innerStream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return _innerStream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return _innerStream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        _innerStream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        _innerStream.Write(buffer, offset, count);
    }
}