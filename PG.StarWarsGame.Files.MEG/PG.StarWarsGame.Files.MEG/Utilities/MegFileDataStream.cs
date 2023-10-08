// // Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO;

namespace PG.StarWarsGame.Files.MEG.Utilities;

internal class MegFileDataStream : Stream
{
    public override bool CanRead { get; }
    public override bool CanSeek { get; }
    public override bool CanWrite => false;
    public override long Length { get; }
    public override long Position { get; set; }

    private readonly Stream _fileStream;

    private readonly uint _offset;
    private readonly uint _dataSize;

    public MegFileDataStream(Stream fileStream, uint fileOffset, uint dataSize)
    {
        _fileStream = fileStream;
        _offset = fileOffset;
        _dataSize = dataSize;
    }

    public override void Flush()
    {
        throw new NotImplementedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }
}