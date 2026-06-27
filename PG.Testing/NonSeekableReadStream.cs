using System;
using System.IO;

namespace PG.Testing;

/// <summary>
/// Represents a read-only, forward-only <see cref="Stream"/> wrapper that reports <see cref="CanSeek"/> as <see langword="false"/>.
/// </summary>
/// <remarks>
/// Useful for exercising code paths that must handle non-seekable streams.
/// </remarks>
/// <param name="data">The data exposed by the stream.</param>
public sealed class NonSeekableReadStream(byte[] data) : Stream
{
    private readonly MemoryStream _inner = new(data, writable: false);

    /// <inheritdoc/>
    public override bool CanRead => true;
    /// <inheritdoc/>
    public override bool CanSeek => false;
    /// <inheritdoc/>
    public override bool CanWrite => false;
    /// <inheritdoc/>
    public override long Length => throw new NotSupportedException();
    /// <inheritdoc/>
    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public override int Read(byte[] buffer, int offset, int count) => _inner.Read(buffer, offset, count);
    /// <inheritdoc/>
    public override void Flush() { }
    /// <inheritdoc/>
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    /// <inheritdoc/>
    public override void SetLength(long value) => throw new NotSupportedException();
    /// <inheritdoc/>
    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _inner.Dispose();
        base.Dispose(disposing);
    }
}
