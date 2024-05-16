// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO;
using AnakinRaW.CommonUtilities;

namespace PG.StarWarsGame.Files.MEG.Utilities;

/// <summary>
/// Represent a read-only, non-seekable file stream that points to a single data entry inside a MEG file.
/// </summary>
public sealed class MegFileDataStream : Stream
{
    /// <summary>
    /// Gets the path of the entry used the MEG archive.
    /// </summary>
    public string EntryPath { get; }

    /// <inheritdoc />
    public override bool CanRead => true;

    /// <inheritdoc />
    public override bool CanSeek => true;

    /// <inheritdoc />
    public override bool CanWrite => false;

    /// <inheritdoc />
    public override long Length => _dataSize;

    /// <inheritdoc />
    public override long Position
    {
        get => _currentPos;
        set
        {
            if (_baseStream is null)
                throw new ObjectDisposedException(null);

            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "value cannot be negative");
            if (value >= Length)
                throw new ArgumentOutOfRangeException(nameof(value),
                    "cannot set position beyond the length of this stream.");

            _currentPos = value;
            _baseStream.Seek(_fileOffset + value, SeekOrigin.Begin);
        }
    }

    private Stream? _baseStream;

    private readonly uint _fileOffset;
    private readonly uint _dataSize;

    private long _currentPos;

    internal MegFileDataStream(string entryPath, Stream baseStream, uint fileOffset, uint dataSize)
    {
        ThrowHelper.ThrowIfNullOrEmpty(entryPath);
        
        EntryPath = entryPath;
        _baseStream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
        _fileOffset = fileOffset;

        if (!baseStream.CanRead || !baseStream.CanSeek)
            throw new ArgumentException("Base stream is not readable or seekable", nameof(baseStream));

        if (fileOffset > baseStream.Length)
            throw new ArgumentException("MEG data offset exceeds total MEG archive size", nameof(fileOffset));

        if (fileOffset + dataSize > baseStream.Length)
            throw new ArgumentException("MEG data size exceeds total MEG archive size");

        _dataSize = dataSize;
        _currentPos = 0;

        baseStream.Position = fileOffset;
    }

    internal static MegFileDataStream CreateEmptyStream(string entryPath)
    {
        return new MegFileDataStream(entryPath, Null, 0, 0);
    }

    /// <inheritdoc />
    public override void Flush()
    {
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public override long Seek(long offset, SeekOrigin origin)
    {
        if (_baseStream is null)
            throw new ObjectDisposedException(null);

        return SeekCore(offset, origin switch
        {
            SeekOrigin.Begin => 0,
            SeekOrigin.Current => _currentPos,
            SeekOrigin.End => Length,
            _ => throw new ArgumentOutOfRangeException(nameof(origin), origin, null)
        });
    }

    private long SeekCore(long offset, long loc)
    {
        var newPosition = unchecked(loc + (int)offset);
        return Position = newPosition;
    }


    /// <inheritdoc />
    public override void SetLength(long value)
    {
        throw new NotSupportedException("Setting the length of this stream is not supported");
    }

    /// <inheritdoc />
    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException("Writing to this stream is not supported");
    }

    /// <inheritdoc />
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