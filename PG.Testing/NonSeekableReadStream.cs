using System;
using System.IO;

namespace PG.Testing;

/// <summary>
/// A read-only, forward-only <see cref="Stream"/> wrapper that reports <see cref="CanSeek"/> as <see langword="false"/>.
/// Useful for exercising code paths that must handle non-seekable streams.
/// </summary>
public sealed class NonSeekableReadStream(byte[] data) : Stream
{
    private readonly MemoryStream _inner = new(data, writable: false);

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => throw new NotSupportedException();
    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public override int Read(byte[] buffer, int offset, int count) => _inner.Read(buffer, offset, count);
    public override void Flush() { }
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _inner.Dispose();
        base.Dispose(disposing);
    }
}
