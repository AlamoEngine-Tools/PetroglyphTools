using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Binary;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Test.Data.Entries;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Converters;

public abstract class MegBinaryConverterTest
{
    private IMegBinaryConverter _converter = null!;

    public abstract bool SupportsEncryption { get; }

    internal abstract IMegBinaryConverter CreateConverter(IServiceProvider sp);

    [TestInitialize]
    public void InitTest()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(new MockFileSystem());
        _converter = CreateConverter(sc.BuildServiceProvider());
    }

    private protected abstract IMegFileMetadata CreateMetadata(IMegHeader header, IMegFileNameTable fileNameTable,
        IMegFileTable fileTable);

    private protected abstract IMegHeader CreateHeader(uint entriesCount);

    private protected abstract IMegFileNameTable CreateFileNameTable(IList<MegDataEntry> entries);

    private protected abstract IMegFileDescriptor CreateFileDescriptor(MegDataEntry entry, uint index);

    private protected abstract IMegFileTable CreateFileTable(List<IMegFileDescriptor> records);

    [TestMethod]
    public void Test_BinaryToModel()
    {
        var entries = new List<MegDataEntry>
        {
            MegDataEntryTest.CreateEntry("abc", new Crc32(0), 1, 2),
            MegDataEntryTest.CreateEntry("abc", new Crc32(0), 1, 2, true)
        };

        var binary = CreateMetadata(entries);

        var model = _converter.BinaryToModel(binary);

        Assert.AreEqual(entries.Count, model.Count);
        for (var i = 0; i < entries.Count; i++)
        {
            var expectedEntry = entries[i];
            var actualEntry = model[i];

            Assert.AreEqual(expectedEntry.FilePath, actualEntry.FilePath);
            Assert.AreEqual(expectedEntry.OriginalFilePath, actualEntry.OriginalFilePath);
            Assert.AreEqual(expectedEntry.Crc32, actualEntry.Crc32);
            Assert.AreEqual(expectedEntry.Location.Size, actualEntry.Location.Size);
            Assert.AreEqual(expectedEntry.Location.Offset, actualEntry.Location.Offset);

            if (!SupportsEncryption)
                Assert.IsFalse(actualEntry.Encrypted);
            else
                Assert.AreEqual(expectedEntry.Encrypted, actualEntry.Encrypted);
        }
    }

    [TestMethod]
    public void Test_BinaryToModel_EmptyArchive()
    {
        var entries = new List<MegDataEntry>();
        var binary = CreateMetadata(entries);
        var model = _converter.BinaryToModel(binary);
        Assert.AreEqual(0, model.Count);
    }

    [TestMethod]
    public void Test_BinaryToModel_NotSorted_Throws()
    {
        var entries = new List<MegDataEntry>
        {
            MegDataEntryTest.CreateEntry("abc", new Crc32(99)),
            MegDataEntryTest.CreateEntry("abc", new Crc32(0))
        };

        var binary = CreateMetadata(entries);

        Assert.ThrowsException<BinaryCorruptedException>(() => _converter.BinaryToModel(binary));
    }

    [TestMethod]
    public void Test_BinaryToModel_Null_Throws()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _converter.BinaryToModel(null!));
    }


    [TestMethod]
    public void Test_ModelToBinary()
    {
        var model = new MegArchive(new List<MegDataEntry>
        {
            MegDataEntryTest.CreateEntry("abc", new Crc32(0), 1, 2),
            MegDataEntryTest.CreateEntry("abc", new Crc32(0), 1, 2, true)
        });

        var binary = _converter.ModelToBinary(model);

        Assert.AreEqual(model.Count, binary.Header.FileNumber);

        for (var i = 0; i < binary.FileNameTable.Count; i++)
        {
            var nameEntry = binary.FileNameTable[i];
            Assert.AreEqual(model[i].FilePath, nameEntry.FileName);
            Assert.AreEqual(model[i].OriginalFilePath, nameEntry.OriginalFileName);
        }

        for (var i = 0; i < binary.FileTable.Count; i++)
        {
            var fileEntry = binary.FileTable[i];
            Assert.AreEqual(model[i].Crc32, fileEntry.Crc32);

            if (!SupportsEncryption)
                Assert.IsFalse(fileEntry.Encrypted);
            else 
                Assert.AreEqual(model[i].Encrypted, fileEntry.Encrypted);

            Assert.AreEqual(model[i].Location.Size, fileEntry.FileSize);
            Assert.AreEqual(model[i].Location.Offset, fileEntry.FileOffset);
        }
    }

    [TestMethod]
    public void Test_ModelToBinary_Null_Throws()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _converter.ModelToBinary(null!));
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