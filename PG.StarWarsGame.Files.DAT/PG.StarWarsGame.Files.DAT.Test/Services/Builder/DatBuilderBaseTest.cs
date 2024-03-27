using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;
using PG.StarWarsGame.Files.DAT.Services;
using PG.StarWarsGame.Files.DAT.Services.Builder;
using PG.StarWarsGame.Files.DAT.Services.Builder.Validation;
using PG.Testing;
using Testably.Abstractions.Testing;

namespace PG.StarWarsGame.Files.DAT.Test.Services.Builder;

public abstract class DatBuilderBaseTest
{
    protected readonly Mock<IDatKeyValidator> KeyValidator = new();
    protected readonly MockFileSystem FileSystem = new();
    protected readonly Mock<IDatFileService> DatFileService = new();
    protected readonly Mock<ICrc32HashingService> HashingService = new();

    protected abstract Mock<DatBuilderBase> CreateBuilder();
    
    protected IServiceProvider CreateServiceProvider()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(FileSystem);
        sc.AddSingleton(_ => DatFileService.Object);
        sc.AddSingleton(_ => KeyValidator.Object);
        sc.AddSingleton(_ => HashingService.Object);
        return sc.BuildServiceProvider();
    }

    [TestMethod]
    public void Test_Ctor()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(FileSystem);
        var builderMock = new Mock<DatBuilderBase>(CreateServiceProvider()) { CallBase = true };

        var builder = builderMock.Object;

        Assert.IsNotNull(builder.KeyValidator);
        Assert.IsNotNull(builder.BuilderData);
        Assert.IsNotNull(builder.SortedEntries);
        Assert.IsNotNull(builder.Entries);
    }

    [TestMethod]
    public void Test_IsKeyValid()
    {
        KeyValidator.SetupSequence(v => v.Validate("key"))
            .Returns(new ValidationResult())
            .Returns(new ValidationResult([new ValidationFailure("error", "error")]));
        var builder = CreateBuilder();

        Assert.ThrowsException<ArgumentNullException>(() => builder.Object.IsKeyValid(null!));
        Assert.IsTrue(builder.Object.IsKeyValid("key"));
        Assert.IsFalse(builder.Object.IsKeyValid("key"));
    }

    #region Clear/Remove/Dispose

    [TestMethod]
    public void Test_Clear()
    {
        KeyValidator.Setup(v => v.Validate(It.IsAny<string>())).Returns(new ValidationResult());
       
        var builder = CreateBuilder();
       

        var entries = builder.Object.BuilderData;
        Assert.AreEqual(0, entries.Count);

        builder.Object.AddEntry("key", "value");

        Assert.AreEqual(1, builder.Object.BuilderData.Count);

        builder.Object.Clear();

        Assert.AreEqual(0, builder.Object.BuilderData.Count);
    }

    [TestMethod]
    public void Test_Remove()
    {
        KeyValidator.Setup(v => v.Validate(It.IsAny<string>()))
            .Returns(new ValidationResult());

        var builder = CreateBuilder();
        builder.SetupGet(b => b.KeyOverwriteBehavior).Returns(BuilderOverrideKind.AllowDuplicate);
        
        builder.Object.AddEntry("key", "value");
        builder.Object.AddEntry("key1", "other");

        Assert.AreEqual(2, builder.Object.BuilderData.Count);

        Assert.IsFalse(builder.Object.Remove(default));
        Assert.IsFalse(builder.Object.RemoveAllKeys("key3"));

        Assert.AreEqual(2, builder.Object.BuilderData.Count);

        Assert.IsTrue(builder.Object.Remove(new DatStringEntry("key1", new Crc32(0), "other")));

        CollectionAssert.AreEqual(new List<DatStringEntry>
            {
                new("key", new Crc32(0), "value"),
            }, 
            builder.Object.BuilderData.ToList());

        Assert.IsTrue(builder.Object.RemoveAllKeys("key"));

        Assert.AreEqual(0, builder.Object.BuilderData.Count);
    }

    [TestMethod]
    public void Test_Dispose_ThrowsOnAddingMethods()
    {
        KeyValidator.Setup(v => v.Validate(It.IsAny<string>())).Returns(new ValidationResult());

        var builder = CreateBuilder();

        builder.Object.Dispose();

        Assert.AreEqual(0, builder.Object.BuilderData.Count);

        Assert.ThrowsException<ObjectDisposedException>(() => builder.Object.AddEntry("key", "value"));
        
        ExceptionUtilities.AssertDoesNotThrowException(() => builder.Object.Entries);
        ExceptionUtilities.AssertDoesNotThrowException(builder.Object.Clear);
        ExceptionUtilities.AssertDoesNotThrowException(() => builder.Object.Remove(new DatStringEntry()));
        ExceptionUtilities.AssertDoesNotThrowException(() => builder.Object.RemoveAllKeys("key"));

        ExceptionUtilities.AssertDoesNotThrowException(builder.Object.Dispose);
    }

    #endregion

    #region AddEntry

    [TestMethod]
    public void Test_AddEntry_Throws()
    {
        KeyValidator.Setup(v => v.Validate(It.IsAny<string>()))
            .Returns(new ValidationResult());

        var builder = CreateBuilder();

        Assert.ThrowsException<ArgumentNullException>(() => builder.Object.AddEntry(null!, "value"));
        Assert.ThrowsException<ArgumentNullException>(() => builder.Object.AddEntry("key", null!));
    }

    [TestMethod]
    public void Test_AddEntry()
    {
        KeyValidator.Setup(v => v.Validate(It.IsAny<string>()))
            .Returns(new ValidationResult());

        var builder = CreateBuilder();
        builder.SetupGet(b => b.KeyOverwriteBehavior)
            .Returns(BuilderOverrideKind.NoOverwrite);

        var result = builder.Object.AddEntry("key", "value");

        Assert.IsTrue(result.Added);
        Assert.IsFalse(result.WasOverwrite);
        Assert.AreEqual(AddEntryState.Added, result.Status);
        Assert.AreEqual(1, builder.Object.BuilderData.Count);
        Assert.AreEqual("value", builder.Object.BuilderData.First().Value);
    }

    [TestMethod]
    public void Test_AddEntry_DoNotAllowDuplicates()
    {
        KeyValidator.Setup(v => v.Validate(It.IsAny<string>()))
            .Returns(new ValidationResult());

        var builder = CreateBuilder();
        builder.SetupGet(b => b.KeyOverwriteBehavior)
            .Returns(BuilderOverrideKind.NoOverwrite);

        builder.Object.AddEntry("key", "value");
        var result = builder.Object.AddEntry("key", "value1");

        Assert.IsFalse(result.Added);
        Assert.AreEqual(AddEntryState.NotAddedDuplicate, result.Status);
        Assert.AreEqual(1, builder.Object.BuilderData.Count);
        Assert.AreEqual("value", builder.Object.BuilderData.First().Value);
    }

    [TestMethod]
    public void Test_AddEntry_OverwriteDuplicates()
    {
        KeyValidator.Setup(v => v.Validate(It.IsAny<string>()))
            .Returns(new ValidationResult());

        var builder = CreateBuilder();
        builder.SetupGet(b => b.KeyOverwriteBehavior)
            .Returns(BuilderOverrideKind.Overwrite);

        builder.Object.AddEntry("key", "value");
        var result = builder.Object.AddEntry("key", "value1");

        Assert.IsTrue(result.Added);
        Assert.IsTrue(result.WasOverwrite);
        Assert.IsNotNull(result.OverwrittenEntry);
        Assert.AreEqual(AddEntryState.AddedDuplicate, result.Status);
        Assert.AreEqual(1, builder.Object.BuilderData.Count);
        Assert.AreEqual("value1", builder.Object.BuilderData.First().Value);
    }

    [TestMethod]
    public void Test_AddEntry_AllowDuplicates()
    {
        KeyValidator.Setup(v => v.Validate(It.IsAny<string>()))
            .Returns(new ValidationResult());
        HashingService.Setup(h => h.GetCrc32("key", Encoding.ASCII)).Returns(new Crc32(1));
        HashingService.Setup(h => h.GetCrc32("key1", Encoding.ASCII)).Returns(new Crc32(2));

        var builder = CreateBuilder();
        builder.SetupGet(b => b.KeyOverwriteBehavior)
            .Returns(BuilderOverrideKind.AllowDuplicate);

        builder.Object.AddEntry("key", "value");
        builder.Object.AddEntry("key1", "other");
        var result = builder.Object.AddEntry("key", "value1");

        Assert.IsTrue(result.Added);
        Assert.IsFalse(result.WasOverwrite);
        Assert.IsNull(result.OverwrittenEntry);
        Assert.AreEqual(AddEntryState.AddedDuplicate, result.Status);
        Assert.AreEqual(3, builder.Object.BuilderData.Count);

        Assert.AreEqual("value", builder.Object.BuilderData.First(e => e.Key == "key").Value);
        Assert.AreEqual("value1", builder.Object.BuilderData.Last(e => e.Key == "key").Value);
    }

    [TestMethod]
    public void Test_AddEntry_PerformsEncoding()
    {
        KeyValidator.Setup(v => v.Validate("key???"))
            .Returns(new ValidationResult()); 

        var builder = CreateBuilder();

        var result = builder.Object.AddEntry("keyÖÄÜ", "value");

        Assert.IsTrue(result.Added);

        Assert.AreEqual("key???", result.AddedEntry.Value.Key);
        Assert.AreEqual("keyÖÄÜ", result.AddedEntry.Value.OriginalKey);
    }


    [TestMethod]
    public void Test_AddEntry_InvalidKey()
    {
        KeyValidator.Setup(v => v.Validate(It.IsAny<string>()))
            .Returns(new ValidationResult(new List<ValidationFailure> { new("error", "error") }));

        HashingService.Setup(h => h.GetCrc32("key", Encoding.ASCII)).Returns(new Crc32(2));

        var builder = CreateBuilder();

        var result = builder.Object.AddEntry("key", "value");

        Assert.IsFalse(result.Added);
        Assert.AreEqual(AddEntryState.InvalidKey, result.Status);
    }

    #endregion

    [TestMethod]
    public void Test_Build()
    {
        var builder = CreateBuilder();

        DatFileService.Setup(s => s.CreateDatFile(It.IsAny<FileSystemStream>(), It.IsAny<IEnumerable<DatStringEntry>>(),
            builder.Object.TargetKeySortOrder))
            .Callback((FileSystemStream fs, IEnumerable<DatStringEntry> _, DatFileType _) =>
            {
                fs.WriteByte(1);
                fs.WriteByte(2);
                fs.WriteByte(3);
            });

        builder.Object.Build(new DatFileInformation
        {
            FilePath = "test.dat"
        }, false);

        CollectionAssert.AreEqual(new byte[]{1,2,3}, FileSystem.File.ReadAllBytes("test.dat"));
    }
}