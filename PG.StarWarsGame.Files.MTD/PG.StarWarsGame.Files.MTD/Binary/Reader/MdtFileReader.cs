using System;
using System.Collections.Generic;
using System.IO;
using PG.Commons.Binary;
using PG.Commons.Services;
using PG.Commons.Utilities;
using PG.StarWarsGame.Files.MTD.Binary.Metadata;

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

            for (int i = 0; i < header.Count; i++)
            {
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
        catch (EndOfStreamException)
        {
            throw new BinaryCorruptedException("Unable to read MDT file.");
        }
    }
}