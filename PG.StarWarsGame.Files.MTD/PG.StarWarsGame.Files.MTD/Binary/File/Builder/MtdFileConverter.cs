// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Text;
using PG.StarWarsGame.Files.MTD.Binary.File.Type.Definition;
using PG.StarWarsGame.Files.MTD.Holder;

namespace PG.StarWarsGame.Files.MTD.Binary.File.Builder;

internal class MtdFileConverter : IBinaryFileConverter<MtdFile, MtdFileHolder>
{
    private const int HEADER_START_OFFSET = 0;
    private const int IMAGE_TABLE_START_OFFSET = sizeof(uint);
    private const int HEADER_SIZE = 4;
    private const int RECORD_SIZE = 81;
    private const int MIN_FILE_SIZE = HEADER_SIZE + RECORD_SIZE;

    public MtdFile FromBytes(byte[] byteStream)
    {
        if ((byteStream == null) || (byteStream.Length < MIN_FILE_SIZE) ||
            (((byteStream.Length - HEADER_SIZE) % RECORD_SIZE) != 0))
        {
            throw new ArgumentException(nameof(byteStream), "The provided file is empty or corrupted.");
        }

        MtdHeader? header = BuildMtdHeaderInternal(byteStream);
        MtdImageTable? imageTable = BuildMtdImageTableInternal(byteStream);
        return new MtdFile(header, imageTable);
    }

    private static MtdHeader BuildMtdHeaderInternal(byte[] byteStream)
    {
        var imageTableEntryCount = BitConverter.ToUInt32(byteStream, HEADER_START_OFFSET);
        return new MtdHeader(imageTableEntryCount);
    }

    private static MtdImageTable BuildMtdImageTableInternal(byte[] byteStream)
    {
        var imageTableRecords = new List<MtdImageTableRecord>();

        for (int i = IMAGE_TABLE_START_OFFSET; i < byteStream.Length; i += RECORD_SIZE)
        {
            int currentOffset = i;
            string? name = Encoding.ASCII.GetString(byteStream, currentOffset, MtdImageTableRecord.BINARY_NAME_LENGTH);
            name = name.Replace("\0", null).Trim();
            currentOffset += MtdImageTableRecord.BINARY_NAME_LENGTH;
            var xPosition = BitConverter.ToUInt32(byteStream, currentOffset);
            currentOffset += sizeof(uint);
            var yPosition = BitConverter.ToUInt32(byteStream, currentOffset);
            currentOffset += sizeof(uint);
            var xExtend = BitConverter.ToUInt32(byteStream, currentOffset);
            currentOffset += sizeof(uint);
            var yExtend = BitConverter.ToUInt32(byteStream, currentOffset);
            currentOffset += sizeof(uint);
            var alpha = BitConverter.ToBoolean(byteStream, currentOffset);
            imageTableRecords.Add(new MtdImageTableRecord(name, xPosition, yPosition, xExtend, yExtend, alpha));
        }

        return new MtdImageTable(imageTableRecords);
    }

    public MtdFile FromHolder(MtdFileHolder holder)
    {
        throw new NotImplementedException();
    }
}