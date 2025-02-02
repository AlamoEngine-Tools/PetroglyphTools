using System.Collections.Generic;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.Validation;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Validation;

public abstract class MegFileTableValidatorTestBase
{
    internal class TestFileDescriptorInfo
    {
        public Crc32 Crc32 { get; init; }
        public uint Index { get; init; }
        public uint FileSize { get; init; }
        public uint FileOffset { get; init; }
        public uint FileNameIndex { get; init; }
    }

    private protected abstract IMegFileTable CreateFileTable(IList<TestFileDescriptorInfo> files);

    [Fact]
    public void Validate_Empty_IsValid()
    {
        var fileTable = CreateFileTable([]);
        var validator = new MegFileTableValidator();
        Assert.True(validator.Validate(fileTable));
    }

    [Fact]
    public void Validate_IsValid()
    {
        var entry1 = new TestFileDescriptorInfo
        {
            Crc32 = new Crc32(0),
            Index = 0
        };
        var entry2 = new TestFileDescriptorInfo
        {
            Crc32 = new Crc32(1),
            Index = 1
        };
        var entry3 = new TestFileDescriptorInfo
        {
            Crc32 = new Crc32(1), // Same CRC but updated index
            Index = 2
        };

        var fileTable = CreateFileTable([entry1, entry2, entry3]);

        var validator = new MegFileTableValidator();
        Assert.True(validator.Validate(fileTable));
    }

    [Fact]
    public void Validate_Invalid_CrcOrder()
    {
        var entry1 = new TestFileDescriptorInfo
        {
            Crc32 = new Crc32(1),
            Index = 0
        };
        var entry2 = new TestFileDescriptorInfo
        {
            Crc32 = new Crc32(0),
            Index = 1
        };

        var fileTable = CreateFileTable([entry1, entry2]);

        var validator = new MegFileTableValidator();
        Assert.False(validator.Validate(fileTable));
    }

    [Fact]
    public void Validate_Invalid_WrongIndex()
    {
        var entry1 = new TestFileDescriptorInfo
        {
            Crc32 = new Crc32(1),
            Index = 0
        };
        var entry2 = new TestFileDescriptorInfo
        {
            Crc32 = new Crc32(0),
            Index = 999
        };

        var fileTable = CreateFileTable([entry1, entry2]);

        var validator = new MegFileTableValidator();
        Assert.False(validator.Validate(fileTable));
    }
}