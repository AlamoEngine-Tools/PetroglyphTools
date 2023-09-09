// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO;
using PG.Commons.Services;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;

namespace PG.StarWarsGame.Files.MEG.Binary;

internal abstract class MegFileBinaryServiceBase<TMegMetadata, TMegHeader, TMegFileTable> : ServiceBase, IMegFileBinaryService
    where TMegMetadata : IMegFileMetadata
    where TMegHeader : IMegHeader
    where TMegFileTable : IMegFileTable 
{
    protected MegFileBinaryServiceBase(IServiceProvider services) : base(services)
    {
    }


    public IMegFileMetadata ReadBinary(Stream byteStream)
    {
        if (byteStream == null)
            throw new ArgumentNullException(nameof(byteStream));
        if (byteStream.Length == 0)
            throw new ArgumentException("MEG data stream must not be empty.");
        if (IsDisposed)
            throw new ObjectDisposedException(ToString());

        using var binaryReader = new BinaryReader(byteStream, MegFileConstants.MegContentFileNameEncoding, true);

        var header = BuildMegHeader(binaryReader) ?? throw new InvalidOperationException("MEG header must not be null.");
        var fileNameTable = BuildFileNameTable(binaryReader, header) ?? throw new InvalidOperationException("MEG file name table must not be null.");
        var fileTable = BuildFileTable(binaryReader, header) ?? throw new InvalidOperationException("MEG file table must not be null."); ;

        return CreateMegMetadata(header, fileNameTable, fileTable);
    }

    protected internal abstract TMegMetadata CreateMegMetadata(TMegHeader header, MegFileNameTable fileNameTable, TMegFileTable fileTable);

    protected internal abstract TMegHeader BuildMegHeader(BinaryReader binaryReader);

    protected internal abstract TMegFileTable BuildFileTable(BinaryReader binaryReader, TMegHeader header);

    protected internal virtual MegFileNameTable BuildFileNameTable(BinaryReader binaryReader, TMegHeader header)
    {
        var fileNameTable = new List<MegFileNameTableRecord>();
        var encoding = MegFileConstants.MegContentFileNameEncoding;
        for (uint i = 0; i < header.FileNumber; i++)
        {
            var fileNameLength = binaryReader.ReadUInt16();
            
            // Reading the string as ASCII has the potential of creating PG/Windows
            // illegal file names due to the replacement character '?'. 
            // At this stage we don't check for sanity in order to read .MEG files created by other tools, such as Mike's MEG Editor.
            // See also MegFileNameTableRecord.cs
            var fileName = binaryReader.ReadString(fileNameLength, encoding);

            fileNameTable.Add(new MegFileNameTableRecord(fileName));
        }

        return new MegFileNameTable(fileNameTable);
    }

    public override string ToString()
    {
        return GetType().Name;
    }
}