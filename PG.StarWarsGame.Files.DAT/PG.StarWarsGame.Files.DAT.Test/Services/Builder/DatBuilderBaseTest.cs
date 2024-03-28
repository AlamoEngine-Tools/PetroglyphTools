using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;
using PG.StarWarsGame.Files.DAT.Services;
using PG.StarWarsGame.Files.DAT.Services.Builder;
using PG.StarWarsGame.Files.DAT.Services.Builder.Validation;
using PG.Testing;
using Testably.Abstractions.Testing;
using Xunit;

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

    [Fact]
    public void Test_Ctor()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(FileSystem);
        var builderMock = new Mock<DatBuilderBase>(CreateServiceProvider()) { CallBase = true };

        var builder = builderMock.Object;

        Assert.NotNull(builder.KeyValidator);
        Assert.NotNull(builder.BuilderData);
        Assert.NotNull(builder.SortedEntries);
        Assert.NotNull(builder.Entries);
    }

    [Fact]
    public void Test_IsKeyValid()
    {
        KeyValidator.SetupSequence(v => v.Validate("key"))
            .Returns(new ValidationResult())
            .Returns(new ValidationResult([new ValidationFailure("error", "error")]));
        var builder = CreateBuilder();

        Assert.Throws<ArgumentNullException>(() => builder.Object.IsKeyValid(null!));
        Assert.True(builder.Object.IsKeyValid("key"));
        Assert.False(builder.Object.IsKeyValid("key"));
    }

    #region Clear/Remove/Dispose

    [Fact]
    public void Test_Clear()
    {
        KeyValidator.Setup(v => v.Validate(It.IsAny<string>())).Returns(new ValidationResult());
       
        var builder = CreateBuilder();
       

        var entries = builder.Object.BuilderData;
        Assert.Empty(entries);

        builder.Object.AddEntry("key", "value");

        Assert.Single(builder.Object.BuilderData);

        builder.Object.Clear();

        Assert.Empty(builder.Object.BuilderData);
    }

    [Fact]
    public void Test_Remove()
    {
        KeyValidator.Setup(v => v.Validate(It.IsAny<string>()))
            .Returns(new ValidationResult());

        var builder = CreateBuilder();
        builder.SetupGet(b => b.KeyOverwriteBehavior).Returns(BuilderOverrideKind.AllowDuplicate);
        
        builder.Object.AddEntry("key", "value");
        builder.Object.AddEntry("key1", "other");

        Assert.Equal(2, builder.Object.BuilderData.Count);

        Assert.False(builder.Object.Remove(default));
        Assert.False(builder.Object.RemoveAllKeys("key3"));

        Assert.Equal(2, builder.Object.BuilderData.Count);

        Assert.True(builder.Object.Remove(new DatStringEntry("key1", new Crc32(0), "other")));

        Assert.Equal([
                new("key", new Crc32(0), "value")
            ], 
            builder.Object.BuilderData.ToList());

        Assert.True(builder.Object.RemoveAllKeys("key"));

        Assert.Empty(builder.Object.BuilderData);
    }

    [Fact]
    public void Test_Dispose_ThrowsOnAddingMethods()
    {
        KeyValidator.Setup(v => v.Validate(It.IsAny<string>())).Returns(new ValidationResult());

        var builder = CreateBuilder();

        builder.Object.Dispose();

        Assert.Empty(builder.Object.BuilderData);

        Assert.Throws<ObjectDisposedException>(() => builder.Object.AddEntry("key", "value"));
        
        ExceptionUtilities.AssertDoesNotThrowException(() => builder.Object.Entries);
        ExceptionUtilities.AssertDoesNotThrowException(builder.Object.Clear);
        ExceptionUtilities.AssertDoesNotThrowException(() => builder.Object.Remove(new DatStringEntry()));
        ExceptionUtilities.AssertDoesNotThrowException(() => builder.Object.RemoveAllKeys("key"));

        ExceptionUtilities.AssertDoesNotThrowException(builder.Object.Dispose);
    }

    #endregion

    #region AddEntry

    [Fact]
    public void Test_AddEntry_Throws()
    {
        KeyValidator.Setup(v => v.Validate(It.IsAny<string>()))
            .Returns(new ValidationResult());

        var builder = CreateBuilder();

        Assert.Throws<ArgumentNullException>(() => builder.Object.AddEntry(null!, "value"));
        Assert.Throws<ArgumentNullException>(() => builder.Object.AddEntry("key", null!));
    }

    [Fact]
    public void Test_AddEntry()
    {
        KeyValidator.Setup(v => v.Validate(It.IsAny<string>()))
            .Returns(new ValidationResult());

        var builder = CreateBuilder();
        builder.SetupGet(b => b.KeyOverwriteBehavior)
            .Returns(BuilderOverrideKind.NoOverwrite);

        var result = builder.Object.AddEntry("key", "value");

        Assert.True(result.Added);
        Assert.False(result.WasOverwrite);
        Assert.Equal(AddEntryState.Added, result.Status);
        Assert.Single(builder.Object.BuilderData);
        Assert.Equal("value", builder.Object.BuilderData.First().Value);
    }

    [Fact]
    public void Test_AddEntry_DoNotAllowDuplicates()
    {
        KeyValidator.Setup(v => v.Validate(It.IsAny<string>()))
            .Returns(new ValidationResult());

        var builder = CreateBuilder();
        builder.SetupGet(b => b.KeyOverwriteBehavior)
            .Returns(BuilderOverrideKind.NoOverwrite);

        builder.Object.AddEntry("key", "value");
        var result = builder.Object.AddEntry("key", "value1");

        Assert.False(result.Added);
        Assert.Equal(AddEntryState.NotAddedDuplicate, result.Status);
        Assert.Single( builder.Object.BuilderData);
        Assert.Equal("value", builder.Object.BuilderData.First().Value);
    }

    [Fact]
    public void Test_AddEntry_OverwriteDuplicates()
    {
        KeyValidator.Setup(v => v.Validate(It.IsAny<string>()))
            .Returns(new ValidationResult());

        var builder = CreateBuilder();
        builder.SetupGet(b => b.KeyOverwriteBehavior)
            .Returns(BuilderOverrideKind.Overwrite);

        builder.Object.AddEntry("key", "value");
        var result = builder.Object.AddEntry("key", "value1");

        Assert.True(result.Added);
        Assert.True(result.WasOverwrite);
        Assert.NotNull(result.OverwrittenEntry);
        Assert.Equal(AddEntryState.AddedDuplicate, result.Status);
        Assert.Single(builder.Object.BuilderData);
        Assert.Equal("value1", builder.Object.BuilderData.First().Value);
    }

    [Fact]
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

        Assert.True(result.Added);
        Assert.False(result.WasOverwrite);
        Assert.Null(result.OverwrittenEntry);
        Assert.Equal(AddEntryState.AddedDuplicate, result.Status);
        Assert.Equal(3, builder.Object.BuilderData.Count);

        Assert.Equal("value", builder.Object.BuilderData.First(e => e.Key == "key").Value);
        Assert.Equal("value1", builder.Object.BuilderData.Last(e => e.Key == "key").Value);
    }

    [Fact]
    public void Test_AddEntry_PerformsEncoding()
    {
        KeyValidator.Setup(v => v.Validate("key???"))
            .Returns(new ValidationResult()); 

        var builder = CreateBuilder();

        var result = builder.Object.AddEntry("keyÖÄÜ", "value");

        Assert.True(result.Added);

        Assert.Equal("key???", result.AddedEntry.Value.Key);
        Assert.Equal("keyÖÄÜ", result.AddedEntry.Value.OriginalKey);
    }


    [Fact]
    public void Test_AddEntry_InvalidKey()
    {
        KeyValidator.Setup(v => v.Validate(It.IsAny<string>()))
            .Returns(new ValidationResult(new List<ValidationFailure> { new("error", "error") }));

        HashingService.Setup(h => h.GetCrc32("key", Encoding.ASCII)).Returns(new Crc32(2));

        var builder = CreateBuilder();

        var result = builder.Object.AddEntry("key", "value");

        Assert.False(result.Added);
        Assert.Equal(AddEntryState.InvalidKey, result.Status);
    }

    #endregion

    [Fact]
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

        Assert.Equal(new byte[]{1,2,3}, FileSystem.File.ReadAllBytes("test.dat"));
    }
}