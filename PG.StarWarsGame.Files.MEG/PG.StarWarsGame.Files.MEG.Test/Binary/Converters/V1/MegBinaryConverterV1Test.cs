using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.Metadata.V1;
using PG.StarWarsGame.Files.MEG.Binary.V1;
using PG.StarWarsGame.Files.MEG.Data.Entries;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Converters.V1;

[TestClass]
public class MegBinaryConverterV1Test : MegBinaryConverterTest
{
    public override bool SupportsEncryption => false;

    internal override IMegBinaryConverter CreateConverter(IServiceProvider sp)
    {
        return new MegBinaryConverterV1(sp);
    }

    private protected override IMegFileNameTable CreateFileNameTable(IList<MegDataEntry> entries)
    {
        return new MegFileNameTable(entries.Select(e => new MegFileNameTableRecord(e.FilePath, e.OriginalFilePath)).ToList());
    }

    private protected override IMegFileTable CreateFileTable(List<IMegFileDescriptor> records)
    {
        return new MegFileTable(records.Cast<MegFileTableRecord>().ToList());
    }

    private protected override IMegFileDescriptor CreateFileDescriptor(MegDataEntry entry, uint index)
    {
        return new MegFileTableRecord(entry.Crc32, index, entry.Location.Size, entry.Location.Offset, index);
    }

    private protected override IMegFileMetadata CreateMetadata(IMegHeader header, IMegFileNameTable fileNameTable, IMegFileTable fileTable)
    {
        return new MegMetadata((MegHeader)header, (MegFileNameTable)fileNameTable, (MegFileTable)fileTable);
    }

    private protected override IMegHeader CreateHeader(uint fileCount)
    {
        return new MegHeader(fileCount, fileCount);
    }
}