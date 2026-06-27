using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AnakinRaW.CommonUtilities.Testing;
using AnakinRaW.CommonUtilities.Testing.Extensions;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.DAT.Binary;
using PG.StarWarsGame.Files.DAT.Data;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Binary.Reader;

public class DatFileReaderTest : TestBaseWithFileSystem
{
    private readonly DatFileReader _reader;

    public DatFileReaderTest()
    {
        _reader = new DatFileReader(ServiceProvider);
    }

    [Fact]
    public void PeekLayout_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _reader.PeekLayout(null!));
    }

    [Fact]
    public void PeekLayout_ThrowsBinaryCorruptedException()
    {
        Assert.Throws<BinaryCorruptedException>(() => _reader.PeekLayout(new MemoryStream()));
    }

    [Theory]
    [MemberData(nameof(DatLayoutKindTestData))]
    public void PeekLayout(Stream stream, DatLayoutKind expectedLayout)
    {
        var layout = _reader.PeekLayout(stream);
        Assert.Equal(expectedLayout, layout);

        // Ensure that stream is not disposed after read operation
        stream.Position = 1;
    }

    public static IEnumerable<object[]> DatLayoutKindTestData()
    {
        return
        [
            [
                // Empty .DAT: While the file type is not specified by the interface, this test must not crash.
                new MemoryStream([0x0, 0x0, 0x0, 0x0]),
                DatLayoutKind.OrderedByCrc32
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
                DatLayoutKind.OrderedByCrc32
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
                DatLayoutKind.NotOrdered
            ],
            [
                TestingHelpers.GetEmbeddedResource(typeof(DatFileReaderTest), "Files.EmptyKeyWithValue.dat"),
                DatLayoutKind.OrderedByCrc32
            ],
            [
                TestingHelpers.GetEmbeddedResource(typeof(DatFileReaderTest), "Files.SingleEmptyEntry.dat"),
                DatLayoutKind.OrderedByCrc32
            ],
            [
                TestingHelpers.GetEmbeddedResource(typeof(DatFileReaderTest), "Files.SingleEntry.dat"),
                DatLayoutKind.OrderedByCrc32
            ],
            [
                TestingHelpers.GetEmbeddedResource(typeof(DatFileReaderTest), "Files.Sorted_TwoEntries.dat"),
                DatLayoutKind.OrderedByCrc32
            ],
            [
                TestingHelpers.GetEmbeddedResource(typeof(DatFileReaderTest), "Files.Sorted_TwoEntriesDuplicate.dat"),
                DatLayoutKind.OrderedByCrc32
            ],
            [
                TestingHelpers.GetEmbeddedResource(typeof(DatFileReaderTest), "Files.mastertextfile_english.dat"),
                DatLayoutKind.OrderedByCrc32
            ],
            [
                TestingHelpers.GetEmbeddedResource(typeof(DatFileReaderTest), "Files.Index_WithDuplicates.dat"),
                DatLayoutKind.NotOrdered
            ],
            [
                TestingHelpers.GetEmbeddedResource(typeof(DatFileReaderTest), "Files.creditstext_english.dat"),
                DatLayoutKind.NotOrdered
            ]
        ];
    }

    [Fact]
    public void ReadBinary_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _reader.ReadBinary(null!));
    }

    [Fact]
    public void ReadBinary_ThrowsBinaryCorruptedException()
    {
        Assert.Throws<BinaryCorruptedException>(() => _reader.ReadBinary(new MemoryStream()));
    }

    [Fact]
    public void ReadBinary_Integration()
    {
        Assert.DoesNotThrow(() => TestingHelpers.GetEmbeddedResource(typeof(DatFileReaderTest), "Files.mastertextfile_english.dat"));
        Assert.DoesNotThrow(() => TestingHelpers.GetEmbeddedResource(typeof(DatFileReaderTest), "Files.creditstext_english.dat"));
    }

    [Theory]
    [MemberData(nameof(DatReadTestData))]
    public void ReadBinary(Stream stream, ExpectedDatData expectedDat)
    {
        var binary = _reader.ReadBinary(stream);
        Assert.Equal(expectedDat.Number, binary.RecordNumber);
        Assert.Equal(expectedDat.Checksums.ToList(), binary.IndexTable.Select(k => k.Crc32).ToList());
        Assert.Equal(expectedDat.Keys.ToList(), binary.KeyTable.Select(k => k.Key).ToList());
        Assert.Equal(expectedDat.OriginalKeys.ToList(), binary.KeyTable.Select(k => k.OriginalKey).ToList());
        Assert.Equal(expectedDat.Values.ToList(), binary.ValueTable.Select(k => k.Value).ToList());

        // Ensure that stream is not disposed after read operation
        stream.Position = 1;
    }

    public static IEnumerable<object[]> DatReadTestData()
    {
        return
        [
            [
                TestingHelpers.GetEmbeddedResource(typeof(DatFileReaderTest), "Files.Empty.dat"),
                new ExpectedDatData
                {
                    Number = 0,
                    Checksums = new List<Crc32>(),
                    Keys = new List<string>(),
                    OriginalKeys = new List<string>(),
                    Values = new List<string>()
                }
            ],
            [
                TestingHelpers.GetEmbeddedResource(typeof(DatFileReaderTest), "Files.EmptyKeyWithValue.dat"),
                new ExpectedDatData
                {
                    Number = 1,
                    Checksums = new List<Crc32>{default},
                    Keys = new List<string>{string.Empty},
                    OriginalKeys = new List<string>{string.Empty},
                    Values = new List<string>{"a"}
                }
            ],
            [
                TestingHelpers.GetEmbeddedResource(typeof(DatFileReaderTest), "Files.Index_WithDuplicates.dat"),
                new ExpectedDatData
                {
                    Number = 5,
                    Checksums = new List<Crc32>{new(2212294583), new(2212294583), new(2212294583), new(4088798008), new(2226203566) },
                    Keys = new List<string>{"1", "1", "1", "4", "5"},
                    OriginalKeys = new List<string>{"1", "1", "1", "4", "5"},
                    Values = new List<string>{"1", "2", "3", "4", "5"}
                }
            ],
            [
                TestingHelpers.GetEmbeddedResource(typeof(DatFileReaderTest), "Files.Sorted_TwoEntries.dat"),
                new ExpectedDatData
                {
                    Number = 2,
                    Checksums = new List<Crc32>{new(450215437), new(2212294583) },
                    Keys = new List<string>{"2", "1"},
                    OriginalKeys = new List<string>{"2", "1"},
                    Values = new List<string>{"2", "1"}
                }
            ],
            [
                TestingHelpers.GetEmbeddedResource(typeof(DatFileReaderTest), "Files.Sorted_TwoEntriesDuplicate.dat"),
                new ExpectedDatData
                {
                    Number = 2,
                    Checksums = new List<Crc32>{new(3904355907), new(3904355907) },
                    Keys = new List<string>{"a", "a"},
                    OriginalKeys = new List<string>{"a", "a"},
                    Values = new List<string>{"a", "b"}
                }
            ],

            [
                new MemoryStream([
                    0x1, 0x0, 0x0, 0x0, // Header
                    0x1, 0x0, 0x0, 0x0, // Crc
                    0x1, 0x0, 0x0, 0x0, // VL
                    0x1, 0x0, 0x0, 0x0, // KL
                    0x61, 0x0,  // Value
                    0xE4  // Key

                ]),
                new ExpectedDatData
                {
                    Number = 1,
                    Checksums = new List<Crc32>{new(1) },
                    Keys = new List<string>{"?"},
                    OriginalKeys = new List<string>{"ä"},
                    Values = new List<string>{"a"}
                }
            ]
        ];
    }

    public class ExpectedDatData
    {
        public int Number { get; init; }
        public required IList<Crc32> Checksums { get; init; }
        public required IList<string> Keys { get; init; }
        public required IList<string> OriginalKeys { get; init; }
        public required IList<string> Values { get; init; }
    }
}