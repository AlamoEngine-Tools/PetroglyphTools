using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Test.Data.Entries;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Converters;

public abstract class MegBinaryConverterTest
{
    private readonly IMegBinaryConverter _converter = null!;

    public abstract bool SupportsEncryption { get; }

    internal abstract IMegBinaryConverter CreateConverter(IServiceProvider sp);

    protected MegBinaryConverterTest()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(new MockFileSystem());
        _converter = CreateConverter(sc.BuildServiceProvider());
    }

    private protected abstract IMegFileMetadata CreateMetadata(IMegHeader header, BinaryTable<MegFileNameTableRecord> fileNameTable,
        IMegFileTable fileTable);

    private protected abstract IMegHeader CreateHeader(uint entriesCount);

    private protected abstract BinaryTable<MegFileNameTableRecord> CreateFileNameTable(IList<MegDataEntry> entries);

    private protected abstract IMegFileDescriptor CreateFileDescriptor(MegDataEntry entry, uint index);

    private protected abstract IMegFileTable CreateFileTable(List<IMegFileDescriptor> records);

    [Fact]
    public void Test_BinaryToModel()
    {
        var entries = new List<MegDataEntry>
        {
            MegDataEntryTest.CreateEntry("abc", new Crc32(0), 1, 2),
            MegDataEntryTest.CreateEntry("abc", new Crc32(0), 1, 2, true)
        };

        var binary = CreateMetadata(entries);

        var model = _converter.BinaryToModel(binary);

        Assert.Equal(entries.Count, model.Count);
        for (var i = 0; i < entries.Count; i++)
        {
            var expectedEntry = entries[i];
            var actualEntry = model[i];

            Assert.Equal(expectedEntry.FilePath, actualEntry.FilePath);
            Assert.Equal(expectedEntry.OriginalFilePath, actualEntry.OriginalFilePath);
            Assert.Equal(expectedEntry.Crc32, actualEntry.Crc32);
            Assert.Equal(expectedEntry.Location.Size, actualEntry.Location.Size);
            Assert.Equal(expectedEntry.Location.Offset, actualEntry.Location.Offset);

            if (!SupportsEncryption)
                Assert.False(actualEntry.Encrypted);
            else
                Assert.Equal(expectedEntry.Encrypted, actualEntry.Encrypted);
        }
    }

    [Fact]
    public void Test_BinaryToModel_EmptyArchive()
    {
        var entries = new List<MegDataEntry>();
        var binary = CreateMetadata(entries);
        var model = _converter.BinaryToModel(binary);
        Assert.Empty(model);
    }

    [Fact]
    public void Test_BinaryToModel_NotSorted_Throws()
    {
        var entries = new List<MegDataEntry>
        {
            MegDataEntryTest.CreateEntry("abc", new Crc32(99)),
            MegDataEntryTest.CreateEntry("abc", new Crc32(0))
        };

        var binary = CreateMetadata(entries);

        Assert.Throws<BinaryCorruptedException>(() => _converter.BinaryToModel(binary));
    }

    [Fact]
    public void Test_BinaryToModel_Null_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => _converter.BinaryToModel(null!));
    }


    [Fact]
    public void Test_ModelToBinary()
    {
        var model = new MegArchive(new List<MegDataEntry>
        {
            MegDataEntryTest.CreateEntry("abc", new Crc32(0), 1, 2),
            MegDataEntryTest.CreateEntry("abc", new Crc32(0), 1, 2, true)
        });

        var binary = _converter.ModelToBinary(model);

        Assert.Equal(model.Count, binary.Header.FileNumber);

        for (var i = 0; i < binary.FileNameTable.Count; i++)
        {
            var nameEntry = binary.FileNameTable[i];
            Assert.Equal(model[i].FilePath, nameEntry.FileName);
            Assert.Equal(model[i].OriginalFilePath, nameEntry.OriginalFilePath);
        }

        for (var i = 0; i < binary.FileTable.Count; i++)
        {
            var fileEntry = binary.FileTable[i];
            Assert.Equal(model[i].Crc32, fileEntry.Crc32);

            if (!SupportsEncryption)
                Assert.False(fileEntry.Encrypted);
            else 
                Assert.Equal(model[i].Encrypted, fileEntry.Encrypted);

            Assert.Equal(model[i].Location.Size, fileEntry.FileSize);
            Assert.Equal(model[i].Location.Offset, fileEntry.FileOffset);
        }
    }

    [Fact]
    public void Test_ModelToBinary_Null_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => _converter.ModelToBinary(null!));
    }


    private IMegFileMetadata CreateMetadata(IList<MegDataEntry> entries)
    {
        var header = CreateHeader((uint)entries.Count);
        var fileNameTable = CreateFileNameTable(entries);

        var records = new List<IMegFileDescriptor>();
        for (var i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            records.Add(CreateFileDescriptor(entry, (uint)i));
        }

        var fileTable = CreateFileTable(records);
        return CreateMetadata(header, fileNameTable, fileTable);
    }
}