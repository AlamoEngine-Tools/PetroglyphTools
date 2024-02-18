using System;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.Validation;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.Validation;

[TestClass]
public class MegBinaryValidatorTest
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test_Ctor_Throws()
    {
        new MegBinaryValidator(null!);
    }

    
    [TestMethod]
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
        Assert.IsTrue(validator.TestValidate(info.Object).IsValid);
    }

    [TestMethod]
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
        Assert.IsFalse(validator.TestValidate(info.Object).IsValid);
    }

    [TestMethod]
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
        Assert.IsFalse(validator.TestValidate(info.Object).IsValid);
    }

    [TestMethod]
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
        Assert.IsFalse(validator.TestValidate(info.Object).IsValid);
    }

    private class ValidSizeValidator : AbstractValidator<IMegBinaryValidationInformation>, IMegFileSizeValidator
    {
    }

    private class ValidFileTableValidator : AbstractValidator<IMegFileTable>, IFileTableValidator
    {
    }

    private class InvalidSizeValidator : AbstractValidator<IMegBinaryValidationInformation>, IMegFileSizeValidator
    {
        public InvalidSizeValidator()
        {
            RuleFor(i => i).Must(_ => false);
        }
    }

    private class InvalidFileTableValidator : AbstractValidator<IMegFileTable>, IFileTableValidator
    {
        public InvalidFileTableValidator()
        {
            RuleFor(i => i).Must(_ => false);
        }
    }
}