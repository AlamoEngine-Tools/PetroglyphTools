// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO;
using AnakinRaW.CommonUtilities.Extensions;
using PG.Commons.Binary;
using PG.Commons.Services;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;

namespace PG.StarWarsGame.Files.MEG.Binary;

internal abstract class MegFileBinaryReaderBase<TMegMetadata, TMegHeader, TMegFileTable>(IServiceProvider services) :
    ServiceBase(services),
    IMegFileBinaryReader
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

        using var binaryReader = new BinaryReader(byteStream, MegFileConstants.MegDataEntryPathEncoding, true);

        var header = BuildMegHeader(binaryReader) ?? throw new InvalidOperationException("MEG header must not be null.");
        var fileNameTable = BuildFileNameTable(binaryReader, header) ?? throw new InvalidOperationException("MEG file name table must not be null.");
        var fileTable = BuildFileTable(binaryReader, header) ?? throw new InvalidOperationException("MEG file table must not be null."); ;

        return CreateMegMetadata(header, fileNameTable, fileTable);
    }

    protected internal abstract TMegMetadata CreateMegMetadata(TMegHeader header, BinaryTable<MegFileNameTableRecord> fileNameTable, TMegFileTable fileTable);

    protected internal abstract TMegHeader BuildMegHeader(BinaryReader binaryReader);

    protected internal abstract TMegFileTable BuildFileTable(BinaryReader binaryReader, TMegHeader header);

    protected internal virtual BinaryTable<MegFileNameTableRecord> BuildFileNameTable(BinaryReader binaryReader, TMegHeader header)
    {
        var fileNameTable = new List<MegFileNameTableRecord>();
        
        var normalEncoding = MegFileConstants.MegDataEntryPathEncoding;

        // NB: We use Latin1 encoding here, so that we can stay compatible with Mike.NL's tools. 
        var extendedEncoding = MegFileConstants.ExtendedMegEntryPathEncoding;

        for (uint i = 0; i < header.FileNumber; i++)
        {
            var fileNameLength = binaryReader.ReadUInt16();

            // Reading the string as ASCII has the potential of creating PG/Windows illegal file names due to the replacement character '?'. 
            // However, in order to stay compatible with Mike's MEG Editor we don't validate file paths here.
            // Extracting such files, without their name changed, will cause an exception. This is by design.
            var originalFileName = binaryReader.ReadString(fileNameLength, extendedEncoding);
            var asciiFileName = normalEncoding.EncodeString(originalFileName);

            fileNameTable.Add(new MegFileNameTableRecord(asciiFileName, originalFileName));
        }

        return new BinaryTable<MegFileNameTableRecord>(fileNameTable);
    }
}