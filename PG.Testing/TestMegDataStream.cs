using PG.Commons.Data;
using System.IO;

namespace PG.Testing;

/// <summary>
/// Represents a test <see cref="IMegFileDataStream"/> that wraps an inner stream and associates it with a MEG entry path.
/// </summary>
public class TestMegDataStream : Stream, IMegFileDataStream
{
    private readonly Stream _innerStream;

    /// <inheritdoc/>
    public override bool CanRead => _innerStream.CanRead;
    /// <inheritdoc/>
    public override bool CanSeek => _innerStream.CanSeek;
    /// <inheritdoc/>
    public override bool CanWrite => _innerStream.CanWrite;
    /// <inheritdoc/>
    public override long Length => _innerStream.Length;
    /// <inheritdoc/>
    public override long Position
    {
        get => _innerStream.Position;
        set => _innerStream.Position = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestMegDataStream"/> class.
    /// </summary>
    /// <param name="entryPath">The path of the entry used in the MEG archive.</param>
    /// <param name="data">The data exposed by the stream.</param>
    public TestMegDataStream(string entryPath, byte[] data)
    {
        EntryPath = entryPath;
        _innerStream = new MemoryStream(data);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestMegDataStream"/> class.
    /// </summary>
    /// <param name="entryPath">The path of the entry used in the MEG archive.</param>
    /// <param name="dataStream">The inner stream that supplies the data.</param>
    public TestMegDataStream(string entryPath, Stream dataStream)
    {
        EntryPath = entryPath;
        _innerStream = dataStream;
    }

    /// <inheritdoc/>
    public string EntryPath { get; }


    /// <inheritdoc/>
    public override void Flush()
    {
        _innerStream.Flush();
    }

    /// <inheritdoc/>
    public override int Read(byte[] buffer, int offset, int count)
    {
        return _innerStream.Read(buffer, offset, count);
    }

    /// <inheritdoc/>
    public override long Seek(long offset, SeekOrigin origin)
    {
        return _innerStream.Seek(offset, origin);
    }

    /// <inheritdoc/>
    public override void SetLength(long value)
    {
        _innerStream.SetLength(value);
    }

    /// <inheritdoc/>
    public override void Write(byte[] buffer, int offset, int count)
    {
        _innerStream.Write(buffer, offset, count);
    }
}