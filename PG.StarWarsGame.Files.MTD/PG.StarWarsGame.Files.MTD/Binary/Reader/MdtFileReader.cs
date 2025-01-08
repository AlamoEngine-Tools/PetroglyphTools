// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO;
using PG.Commons.Services;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.MTD.Binary.Metadata;
using PG.StarWarsGame.Files.Utilities;

namespace PG.StarWarsGame.Files.MTD.Binary;

internal class MdtFileReader(IServiceProvider serviceProvider) : ServiceBase(serviceProvider), IMtdFileReader
{
    public MtdBinaryFile ReadBinary(Stream byteStream)
    {
        if (byteStream == null)
            throw new ArgumentNullException(nameof(byteStream));

        try
        {
            using var reader = new BinaryReader(byteStream);
            var header = new MtdHeader(reader.ReadUInt32());

            var entries = new List<MtdBinaryFileInfo>();

            for (var i = 0; i < header.Count; i++)
            {
                // 64 here because ReadString already handles the \0 character.
                var name = reader.ReadString(64, MtdFileConstants.NameEncoding, true);

                var x = reader.ReadUInt32();
                var y = reader.ReadUInt32();
                var width = reader.ReadUInt32();
                var height = reader.ReadUInt32();
                var alpha = reader.ReadBoolean();

                entries.Add(new MtdBinaryFileInfo(name, x, y, width, height, alpha));
            }

            return new MtdBinaryFile(header, new BinaryTable<MtdBinaryFileInfo>(entries));

        }
        catch (Exception e) when (e is EndOfStreamException or IndexOutOfRangeException or IOException or ArgumentOutOfRangeException)
        {
            throw new BinaryCorruptedException($"Unable to read MDT file: {e.Message}", e);
        }
    }
}