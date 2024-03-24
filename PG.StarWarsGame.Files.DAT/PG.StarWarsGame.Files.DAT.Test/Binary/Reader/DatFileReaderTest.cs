﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Binary;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Binary;
using PG.StarWarsGame.Files.DAT.Files;
using PG.Testing;
using Testably.Abstractions.Testing;

namespace PG.StarWarsGame.Files.DAT.Test.Binary.Reader;

[TestClass]
public class DatFileReaderTest
{
    private DatFileReader _reader = null!;

    [TestInitialize]
    public void Setup()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(new MockFileSystem());
        _reader = new DatFileReader(sc.BuildServiceProvider());
    }

    [TestMethod]
    public void Test_PeekFileType_ThrowsArgumentNullException()
    {
        Assert.ThrowsException<ArgumentNullException>(() => _reader.PeekFileType(null!));
    }

    [TestMethod]
    public void Test_PeekFileType_ThrowsBinaryCorruptedException()
    {
        Assert.ThrowsException<BinaryCorruptedException>(() => _reader.PeekFileType(new MemoryStream()));
    }

    [DataTestMethod]
    [DynamicData(nameof(DatFileTypeTestData), DynamicDataSourceType.Method)]
    public void Test_PeekFileType(Stream stream, DatFileType expectedFileType)
    {
        var fileType = _reader.PeekFileType(stream);
        Assert.AreEqual(expectedFileType, fileType);
    }

    private static IEnumerable<object[]> DatFileTypeTestData()
    {
        return new[]
        {
            [
                // Empty .DAT: While the file type is not specified by the interface, this test must not crash.
                new MemoryStream([0x0, 0x0, 0x0, 0x0]),
                DatFileType.OrderedByCrc32
            ],
            [
                new MemoryStream([
                    0x3, 0x0, 0x0, 0x0, // Header
                    0x1, 0x0, 0x0, 0x0, // First Crc (1)
                    0x0, 0x0, 0x0, 0x0,
                    0x0, 0x0, 0x0, 0x0,
                    0x1, 0x0, 0x0, 0x0, // Second Crc (1) (Duplicate)
                    0x0, 0x0, 0x0, 0x0,
                    0x0, 0x0, 0x0, 0x0,
                    0x2, 0x0, 0x0, 0x0, // Third Crc (2)
                    0x0, 0x0, 0x0, 0x0,
                    0x0, 0x0, 0x0, 0x0
                ]),
                DatFileType.OrderedByCrc32
            ],
            [
                new MemoryStream([
                    0x3, 0x0, 0x0, 0x0, // Header
                    0x2, 0x0, 0x0, 0x0, // First Crc (2)
                    0x0, 0x0, 0x0, 0x0,
                    0x0, 0x0, 0x0, 0x0,
                    0x2, 0x0, 0x0, 0x0, // Second Crc (2) (Duplicate)
                    0x0, 0x0, 0x0, 0x0,
                    0x0, 0x0, 0x0, 0x0,
                    0x1, 0x0, 0x0, 0x0, // Third Crc (1)
                    0x0, 0x0, 0x0, 0x0,
                    0x0, 0x0, 0x0, 0x0
                ]),
                DatFileType.NotOrdered
            ],
            [
                TestUtility.GetEmbeddedResource(typeof(DatFileReaderTest), "Files.EmptyKeyWithValue.dat"),
                DatFileType.OrderedByCrc32
            ],
            [
                TestUtility.GetEmbeddedResource(typeof(DatFileReaderTest), "Files.SingleEmptyEntry.dat"),
                DatFileType.OrderedByCrc32
            ],
            [
                TestUtility.GetEmbeddedResource(typeof(DatFileReaderTest), "Files.SingleEntry.dat"),
                DatFileType.OrderedByCrc32
            ],
            [
                TestUtility.GetEmbeddedResource(typeof(DatFileReaderTest), "Files.Sorted_TwoEntries.dat"),
                DatFileType.OrderedByCrc32
            ],
            [
                TestUtility.GetEmbeddedResource(typeof(DatFileReaderTest), "Files.Sorted_TwoEntriesDuplicate.dat"),
                DatFileType.OrderedByCrc32
            ],
            [
                TestUtility.GetEmbeddedResource(typeof(DatFileReaderTest), "Files.mastertextfile_english.dat"),
                DatFileType.OrderedByCrc32
            ],
            new object[]
            {
                TestUtility.GetEmbeddedResource(typeof(DatFileReaderTest), "Files.Index_WithDuplicates.dat"),
                DatFileType.NotOrdered
            },
            new object[]
            {
                TestUtility.GetEmbeddedResource(typeof(DatFileReaderTest), "Files.creditstext_english.dat"),
                DatFileType.NotOrdered
            },
        };
    }

    [TestMethod]
    public void Test_ReadBinary_Integration()
    {
        ExceptionUtilities.AssertDoesNotThrowException(() => TestUtility.GetEmbeddedResource(typeof(DatFileReaderTest), "Files.mastertextfile_english.dat"));
        ExceptionUtilities.AssertDoesNotThrowException(() => TestUtility.GetEmbeddedResource(typeof(DatFileReaderTest), "Files.creditstext_english.dat"));
    }

    [DataTestMethod]
    [DynamicData(nameof(DatReadTestData), DynamicDataSourceType.Method)]
    public void Test_ReadBinary(Stream stream, ExpectedDatData expectedDat)
    {
        var binary = _reader.ReadBinary(stream);
        Assert.AreEqual(expectedDat.Number, binary.RecordNumber);
        CollectionAssert.AreEqual(expectedDat.Checksums.ToList(), binary.IndexTable.Select(k => k.Crc32).ToList());
        CollectionAssert.AreEqual(expectedDat.Keys.ToList(), binary.KeyTable.Select(k => k.Key).ToList());
        CollectionAssert.AreEqual(expectedDat.Values.ToList(), binary.ValueTable.Select(k => k.Value).ToList());
    }

    private static IEnumerable<object[]> DatReadTestData()
    {
        return new[]
        {
            new object[]
            {
                TestUtility.GetEmbeddedResource(typeof(DatFileReaderTest), "Files.Empty.dat"),
                new ExpectedDatData
                {
                    Number = 0,
                    Checksums = new List<Crc32>(),
                    Keys = new List<string>(),
                    Values = new List<string>()
                }
            },
            new object[]
            {
                TestUtility.GetEmbeddedResource(typeof(DatFileReaderTest), "Files.EmptyKeyWithValue.dat"),
                new ExpectedDatData
                {
                    Number = 1,
                    Checksums = new List<Crc32>{default},
                    Keys = new List<string>{string.Empty},
                    Values = new List<string>{"a"}
                }
            },
            new object[]
            {
                TestUtility.GetEmbeddedResource(typeof(DatFileReaderTest), "Files.Index_WithDuplicates.dat"),
                new ExpectedDatData
                {
                    Number = 5,
                    Checksums = new List<Crc32>{new(2212294583), new(2212294583), new(2212294583), new(4088798008), new(2226203566) },
                    Keys = new List<string>{"1", "1", "1", "4", "5"},
                    Values = new List<string>{"1", "2", "3", "4", "5"}
                }
            },
            new object[]
            {
                TestUtility.GetEmbeddedResource(typeof(DatFileReaderTest), "Files.Sorted_TwoEntries.dat"),
                new ExpectedDatData
                {
                    Number = 2,
                    Checksums = new List<Crc32>{new(450215437), new(2212294583) },
                    Keys = new List<string>{"2", "1"},
                    Values = new List<string>{"2", "1"}
                }
            },
            new object[]
            {
                TestUtility.GetEmbeddedResource(typeof(DatFileReaderTest), "Files.Sorted_TwoEntriesDuplicate.dat"),
                new ExpectedDatData
                {
                    Number = 2,
                    Checksums = new List<Crc32>{new(3904355907), new(3904355907) },
                    Keys = new List<string>{"a", "a"},
                    Values = new List<string>{"a", "b"}
                }
            },

            new object[]
            {
                new MemoryStream(new byte[]
                {
                    0x1, 0x0, 0x0, 0x0, // Header
                    0x1, 0x0, 0x0, 0x0, // Crc
                    0x1, 0x0, 0x0, 0x0, // VL
                    0x1, 0x0, 0x0, 0x0, // KL
                    0x1, 0x0,  // Value
                    0x1  // Key

                }),
                new ExpectedDatData
                {
                    Number = 1,
                    Checksums = new List<Crc32>{new(1) },
                    Keys = new List<string>{"?"},
                    Values = new List<string>{"a"}
                }
            },
        };
    }

    public class ExpectedDatData
    {
        public int Number { get; init; }
        public IList<Crc32> Checksums { get; init; }
        public IList<string> Keys { get; init; }
        public IList<string> Values { get; init; }
    }

}