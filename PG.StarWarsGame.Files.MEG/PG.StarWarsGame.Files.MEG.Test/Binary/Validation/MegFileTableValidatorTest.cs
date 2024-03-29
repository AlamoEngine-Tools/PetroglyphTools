using Moq;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.Validation;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Validation;

public class MegFileTableValidatorTest
{
    [Fact]
    public void Test_Validate_Empty_IsValid()
    {
        var fileTable = new Mock<IMegFileTable>();
        var validator = new MegFileTableValidator();
        Assert.True(validator.Validate(fileTable.Object).IsValid);
    }

    [Fact]
    public void Test_Validate_IsValid()
    {
        var entry1 = new Mock<IMegFileDescriptor>();
        entry1.SetupGet(e => e.Crc32).Returns(new Crc32(0));
        entry1.SetupGet(e => e.Index).Returns(0);
        var entry2 = new Mock<IMegFileDescriptor>();
        entry2.SetupGet(e => e.Crc32).Returns(new Crc32(1));
        entry2.SetupGet(e => e.Index).Returns(1);
        var entry3 = new Mock<IMegFileDescriptor>();
        entry3.SetupGet(e => e.Crc32).Returns(new Crc32(1));
        entry3.SetupGet(e => e.Index).Returns(2);

        var fileTable = new Mock<IMegFileTable>();
        fileTable.SetupGet(t => t.Count).Returns(3);
        fileTable.SetupGet(t => t[0]).Returns(entry1.Object);
        fileTable.SetupGet(t => t[1]).Returns(entry2.Object);
        fileTable.SetupGet(t => t[2]).Returns(entry3.Object);

        var validator = new MegFileTableValidator();
        Assert.True(validator.Validate(fileTable.Object).IsValid);
    }

    [Fact]
    public void Test_Validate_Invalid_CrcOrder()
    {
        var entry1 = new Mock<IMegFileDescriptor>();
        entry1.SetupGet(e => e.Crc32).Returns(new Crc32(1));
        entry1.SetupGet(e => e.Index).Returns(0);
        var entry2 = new Mock<IMegFileDescriptor>();
        entry2.SetupGet(e => e.Crc32).Returns(new Crc32(0));
        entry2.SetupGet(e => e.Index).Returns(99);

        var fileTable = new Mock<IMegFileTable>();
        fileTable.SetupGet(t => t.Count).Returns(2);
        fileTable.SetupGet(t => t[0]).Returns(entry1.Object);
        fileTable.SetupGet(t => t[1]).Returns(entry2.Object);

        var validator = new MegFileTableValidator();
        Assert.False(validator.Validate(fileTable.Object).IsValid);
    }

    [Fact]
    public void Test_Validate_Invalid_WrongIndex()
    {
        var entry1 = new Mock<IMegFileDescriptor>();
        entry1.SetupGet(e => e.Crc32).Returns(new Crc32(0));
        entry1.SetupGet(e => e.Index).Returns(0);
        var entry2 = new Mock<IMegFileDescriptor>();
        entry2.SetupGet(e => e.Crc32).Returns(new Crc32(1));
        entry2.SetupGet(e => e.Index).Returns(99);

        var fileTable = new Mock<IMegFileTable>();
        fileTable.SetupGet(t => t.Count).Returns(2);
        fileTable.SetupGet(t => t[0]).Returns(entry1.Object);
        fileTable.SetupGet(t => t[1]).Returns(entry2.Object);

        var validator = new MegFileTableValidator();
        Assert.False(validator.Validate(fileTable.Object).IsValid);
    }

    [Fact]
    public void Test_Validate_Invalid_NotSorted()
    {
        var entry1 = new Mock<IMegFileDescriptor>();
        entry1.SetupGet(e => e.Crc32).Returns(new Crc32(99));
        entry1.SetupGet(e => e.Index).Returns(0);
        var entry2 = new Mock<IMegFileDescriptor>();
        entry2.SetupGet(e => e.Crc32).Returns(new Crc32(0));
        entry2.SetupGet(e => e.Index).Returns(1);

        var fileTable = new Mock<IMegFileTable>();
        fileTable.SetupGet(t => t.Count).Returns(2);
        fileTable.SetupGet(t => t[0]).Returns(entry1.Object);
        fileTable.SetupGet(t => t[1]).Returns(entry2.Object);

        var validator = new MegFileTableValidator();
        Assert.False(validator.Validate(fileTable.Object).IsValid);
    }
}