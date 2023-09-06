// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;

namespace PG.StarWarsGame.Files.MEG.Binary.V1;

internal abstract class MegFileBinaryServiceBase<TMegMetadata, TMegHeader, TMegFileTable> : DisposableObject, IMegFileBinaryService 
    where TMegMetadata : IMegFileMetadata
    where TMegHeader : IMegHeader
    where TMegFileTable : IMegFileTable
{
    public IMegFileMetadata ReadBinary(Stream byteStream)
    {
        if (byteStream == null) 
            throw new ArgumentNullException(nameof(byteStream));
        if (byteStream.Length == 0)
            throw new ArgumentException("MEG data stream must not be empty.");
        if (IsDisposed)
            throw new ObjectDisposedException(ToString());
       
        using var binaryReader = new BinaryReader(byteStream, MegFileConstants.MegContentFileNameEncoding, true);

        var header = BuildMegHeader(binaryReader);
        var fileNameTable = BuildFileNameTable(binaryReader, header);
        var fileTable = BuildFileTable(binaryReader, header);

        return CreateMegMetadata(header, fileNameTable, fileTable);
    }

    protected abstract TMegMetadata CreateMegMetadata(TMegHeader header, MegFileNameTable fileNameTable, TMegFileTable fileTable);

    protected abstract TMegHeader BuildMegHeader(BinaryReader binaryReader);

    protected abstract TMegFileTable BuildFileTable(BinaryReader binaryReader, TMegHeader header);

    protected virtual MegFileNameTable BuildFileNameTable(BinaryReader binaryReader, TMegHeader header)
    {
        var fileNameTable = new List<MegFileNameTableRecord>();
        var encoding = MegFileConstants.MegContentFileNameEncoding;
        for (uint i = 0; i < header.FileNumber; i++)
        {
            var fileNameLength = binaryReader.ReadUInt16();
            var fileName = GetString(binaryReader, fileNameLength, encoding);
            fileNameTable.Add(new MegFileNameTableRecord(fileName, encoding));
        }

        return new MegFileNameTable(fileNameTable);
    }

    public override string ToString()
    {
        return GetType().Name;
    }

    private static string GetString(BinaryReader reader, ushort length, Encoding encoding)
    {
        if (length == 0)
            return string.Empty;

#if NETSTANDARD2_1_OR_GREATER
       
        // This is the only perf optimization i could get
        if (length <= 256)
        {
            Span<byte> buffer = stackalloc byte[length];
            var br = reader.Read(buffer);
            if (br != length)
                throw new Exception();
            return encoding.GetString(buffer);
        }
#endif

        var bytes = reader.ReadBytes(length);
        if (bytes.Length != length)
            throw new Exception();
        return encoding.GetString(bytes);
    }
}