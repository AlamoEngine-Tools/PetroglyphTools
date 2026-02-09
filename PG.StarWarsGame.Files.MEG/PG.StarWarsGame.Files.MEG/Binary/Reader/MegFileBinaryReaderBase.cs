// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using AnakinRaW.CommonUtilities.Extensions;
using PG.Commons.Services;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.Validation;
using System;
using System.Collections.Generic;
using System.IO;
using PG.Commons.Hashing;

namespace PG.StarWarsGame.Files.MEG.Binary;

internal abstract class MegFileBinaryReaderBase<TMegMetadata, TMegHeader, TMegFileTable, TMegFileDescriptor>(IServiceProvider services) :
    ServiceBase(services),
    IMegFileBinaryReader
    where TMegMetadata : IMegFileMetadata
    where TMegHeader : IMegHeader
    where TMegFileTable : IMegFileTable
    where TMegFileDescriptor : IMegFileDescriptor
{
    protected abstract IMegBinaryValidator<TMegMetadata> Validator { get; }

    public IMegFileMetadata ReadBinary(Stream byteStream)
    {
        if (byteStream == null)
            throw new ArgumentNullException(nameof(byteStream));
        if (byteStream.Length == 0)
            throw new ArgumentException("MEG data stream must not be empty.");
        // There is no reason to validate the archive's size if we cannot access the whole stream size. 
        // We also don't want to read the whole stream if this is a "lazy" stream (such as a pipe)
        if (!byteStream.CanSeek)
            throw new NotSupportedException("Non-seekable streams are not supported.");

        var startPosition = byteStream.Position;
        using var binaryReader = new PetroglyphBinaryReader(byteStream, true);

        var header = BuildMegHeader(binaryReader) ?? throw new InvalidOperationException("MEG header must not be null.");
        var fileNameTable = BuildFileNameTable(binaryReader, header.FileNumber) ?? throw new InvalidOperationException("MEG file name table must not be null.");
        var fileTable = BuildFileTable(binaryReader, header) ?? throw new InvalidOperationException("MEG file table must not be null.");
        var endPosition = byteStream.Position;

        var metadata = CreateMegMetadata(header, fileNameTable, fileTable);
        
        var metadataSize = endPosition - startPosition;
        var actualMegSize = byteStream.Length - startPosition;
        
        Validator.Validate(metadata, metadataSize, actualMegSize);

        return metadata;
    }

    protected internal abstract TMegMetadata CreateMegMetadata(TMegHeader header, BinaryTable<MegFileNameTableRecord> fileNameTable, TMegFileTable fileTable);

    protected internal abstract TMegHeader BuildMegHeader(PetroglyphBinaryReader binaryReader);

    protected internal TMegFileTable BuildFileTable(PetroglyphBinaryReader binaryReader, TMegHeader header)
    {
        var fileNumber = header.FileNumber;
        var fileDescriptors = new List<TMegFileDescriptor>(fileNumber);

        var lastCrc = new Crc32(0);
        for (var i = 0; i < fileNumber; i++)
        {
            var record = BuildFileDescriptor(binaryReader);
            if (record.Index != i)
                throw new BinaryCorruptedException("The index of the file table record does not match the actual iteration index.");
            if (record.Crc32 < lastCrc)
                throw new BinaryCorruptedException("The file table is not sorted by CRC values.");
            lastCrc = record.Crc32;
            fileDescriptors.Add(record);
        }

        return CreateMegFileTable(fileDescriptors);
    }

    protected abstract TMegFileDescriptor BuildFileDescriptor(PetroglyphBinaryReader binaryReader);

    protected abstract TMegFileTable CreateMegFileTable(IList<TMegFileDescriptor> fileDescriptors);

    public virtual BinaryTable<MegFileNameTableRecord> BuildFileNameTable(PetroglyphBinaryReader binaryReader, int fileNumber)
    {
        var fileNameTable = new List<MegFileNameTableRecord>();

        // NB: We use Latin1 encoding here, so that we can stay compatible with Mike.NL's tools. 
        var extendedEncoding = MegFileConstants.ExtendedMegEntryPathEncoding;
        var normalEncoding = MegFileConstants.MegDataEntryPathEncoding;
        
        for (uint i = 0; i < fileNumber; i++)
        {
            var fileNameLength = binaryReader.ReadUInt16();

            // Reading the string as ASCII has the potential of creating PG/Windows illegal file names due to the replacement character '?'. 
            // However, in order to stay compatible with Mike's MEG Editor we don't validate file paths here.
            // Extracting such files, without their name changed, will cause an exception. This is by design.
            var originalFileName = binaryReader.ReadString(extendedEncoding, fileNameLength);
            var asciiFileName = normalEncoding.EncodeString(originalFileName);

            fileNameTable.Add(new MegFileNameTableRecord(asciiFileName, originalFileName));
        }

        return new BinaryTable<MegFileNameTableRecord>(fileNameTable);
    }
}