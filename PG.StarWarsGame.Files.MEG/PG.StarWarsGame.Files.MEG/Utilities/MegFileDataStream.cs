// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO;

namespace PG.StarWarsGame.Files.MEG.Utilities;

internal class MegFileDataStream : Stream
{
    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => _dataSize;

    public override long Position
    {
        get => _currentPos;
        set => throw new NotSupportedException();
    }

    private Stream? _baseStream;

    private readonly uint _dataSize;

    private long _currentPos;

    public MegFileDataStream(Stream baseStream, uint fileOffset, uint dataSize)
    {
        _baseStream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));

        if (!_baseStream.CanRead || !_baseStream.CanSeek)
            throw new ArgumentException("Base stream is not readable or seekable", nameof(baseStream));

        if (fileOffset > _baseStream.Length)
            throw new ArgumentException("MEG data offset exceeds total MEG archive size", nameof(fileOffset));

        if (fileOffset + dataSize > _baseStream.Length)
            throw new ArgumentException("MEG data size exceeds total MEG archive size");

        _dataSize = dataSize;
        _currentPos = 0;

        baseStream.Position = fileOffset;
    }

    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (buffer is null)
            throw new ArgumentNullException(nameof(buffer));
        if (offset < 0)
            throw new ArgumentOutOfRangeException(nameof(offset));
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        if (_baseStream is null)
            throw new ObjectDisposedException(null);

        if (!_baseStream.CanRead)
            throw new NotSupportedException("Underlying stream is not readable");

        if (buffer.Length - offset < count)
            throw new ArgumentOutOfRangeException();

        var bytesRemaining = _dataSize - _currentPos;

        // Since count is an int32, Math.Min ensures that bytesToRead always fits into an int32. Thus casting it to int is safe
        var bytesToRead = (int)Math.Min(bytesRemaining, count);

        if (bytesToRead <= 0)
            return 0;

        var bytesRead = _baseStream.Read(buffer, offset, bytesToRead);
        if (bytesRead != bytesToRead)
            throw new InvalidOperationException("Expected bytes did not match bytes actually read.");

        _currentPos += bytesRead;

        return bytesRead;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException("Seeking this stream is not supported");
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException("Setting the length of this stream is not supported");
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException("Writing to this stream is not supported");
    }

    protected override void Dispose(bool disposing)
    {
        try
        {
            if (disposing && _baseStream != null) 
                _baseStream.Dispose();
        }
        finally
        {
            _baseStream = null;
            _currentPos = 0;
            base.Dispose(disposing);
        }
    }
}