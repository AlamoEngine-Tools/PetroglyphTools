using System;
using Moq;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.Validation;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Validation;

public class MegBinaryValidatorTest
{
    [Fact]
    public void Test_Ctor_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new MegBinaryValidator(null!));
    }

    
    [Fact]
    public void Test_Validate_Composite_Valid()
    {
        var fileTable = new Mock<IMegFileTable>();
        var metadata = new Mock<IMegFileMetadata>();
        metadata.SetupGet(m => m.FileTable).Returns(fileTable.Object);
        var info = new Mock<IMegBinaryValidationInformation>();
        info.SetupGet(i => i.Metadata).Returns(metadata.Object);

        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IMegFileSizeValidator))).Returns(new ValidSizeValidator());
        sp.Setup(s => s.GetService(typeof(IFileTableValidator))).Returns(new ValidFileTableValidator());
        
        var validator = new MegBinaryValidator(sp.Object);

        // true | true --> true
        Assert.True(validator.Validate(info.Object));
    }

    [Fact]
    public void Test_Validate_Composite_Invalid1()
    {
        var fileTable = new Mock<IMegFileTable>();
        var metadata = new Mock<IMegFileMetadata>();
        metadata.SetupGet(m => m.FileTable).Returns(fileTable.Object);
        var info = new Mock<IMegBinaryValidationInformation>();
        info.SetupGet(i => i.Metadata).Returns(metadata.Object);

        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IMegFileSizeValidator))).Returns(new InvalidSizeValidator());
        sp.Setup(s => s.GetService(typeof(IFileTableValidator))).Returns(new ValidFileTableValidator());

        var validator = new MegBinaryValidator(sp.Object);

        // false | true --> false
        Assert.False(validator.Validate(info.Object));
    }

    [Fact]
    public void Test_Validate_Composite_Invalid2()
    {
        var fileTable = new Mock<IMegFileTable>();
        var metadata = new Mock<IMegFileMetadata>();
        metadata.SetupGet(m => m.FileTable).Returns(fileTable.Object);
        var info = new Mock<IMegBinaryValidationInformation>();
        info.SetupGet(i => i.Metadata).Returns(metadata.Object);

        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IMegFileSizeValidator))).Returns(new ValidSizeValidator());
        sp.Setup(s => s.GetService(typeof(IFileTableValidator))).Returns(new InvalidFileTableValidator());

        var validator = new MegBinaryValidator(sp.Object);

        // true | false --> false
        Assert.False(validator.Validate(info.Object));
    }

    [Fact]
    public void Test_Validate_Composite_Invalid3()
    {
        var fileTable = new Mock<IMegFileTable>();
        var metadata = new Mock<IMegFileMetadata>();
        metadata.SetupGet(m => m.FileTable).Returns(fileTable.Object);
        var info = new Mock<IMegBinaryValidationInformation>();
        info.SetupGet(i => i.Metadata).Returns(metadata.Object);

        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IMegFileSizeValidator))).Returns(new InvalidSizeValidator());
        sp.Setup(s => s.GetService(typeof(IFileTableValidator))).Returns(new InvalidFileTableValidator());

        var validator = new MegBinaryValidator(sp.Object);

        // false | false --> false
        Assert.False(validator.Validate(info.Object));
    }

    private class ValidSizeValidator : IMegFileSizeValidator
    {
        public bool Validate(IMegBinaryValidationInformation info) => true;
    }

    private class ValidFileTableValidator : IFileTableValidator
    {
        public bool Validate(IMegFileTable fileTable) => true;
    }

    private class InvalidSizeValidator : IMegFileSizeValidator
    {
        public bool Validate(IMegBinaryValidationInformation info) => false;
    }

    private class InvalidFileTableValidator : IFileTableValidator
    {
        public bool Validate(IMegFileTable fileTable) => false;
    }
}