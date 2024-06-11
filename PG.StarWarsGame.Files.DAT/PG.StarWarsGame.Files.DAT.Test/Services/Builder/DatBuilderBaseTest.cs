using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using AnakinRaW.CommonUtilities.Hashing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PG.Commons.Extensibility;
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
    protected readonly MockFileSystem FileSystem = new();
    protected readonly Mock<IDatFileService> DatFileService = new();

    protected readonly TestKeyValidator KeyValidator = new();

    protected abstract Mock<DatBuilderBase> CreateBuilder(BuilderOverrideKind overrideKind);
    
    protected IServiceProvider CreateServiceProvider()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(FileSystem);
        sc.AddSingleton<IHashingService>(sp => new HashingService(sp));
        sc.CollectPgServiceContributions();
       
        sc.AddSingleton(_ => DatFileService.Object);
        return sc.BuildServiceProvider();
    }

    [Theory]
    [InlineData(BuilderOverrideKind.NoOverwrite)]
    [InlineData(BuilderOverrideKind.Overwrite)]
    [InlineData(BuilderOverrideKind.AllowDuplicate)]
    public void Test_Ctor(BuilderOverrideKind overrideKind)
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(FileSystem);
        var builderMock = new Mock<DatBuilderBase>(overrideKind, CreateServiceProvider()) { CallBase = true };

        var builder = builderMock.Object;

        Assert.NotNull(builder.KeyValidator);
        Assert.NotNull(builder.BuilderData);
        Assert.NotNull(builder.SortedEntries);
        Assert.NotNull(builder.Entries);
        Assert.Equal(overrideKind, builder.KeyOverwriteBehavior);
    }

    [Theory]
    [InlineData(BuilderOverrideKind.NoOverwrite)]
    [InlineData(BuilderOverrideKind.Overwrite)]
    [InlineData(BuilderOverrideKind.AllowDuplicate)]
    public void Test_IsKeyValid(BuilderOverrideKind overrideKind)
    {
        var builder = CreateBuilder(overrideKind);

        Assert.Throws<ArgumentNullException>(() => builder.Object.IsKeyValid(null!));
        Assert.True(builder.Object.IsKeyValid("key"));
        Assert.False(builder.Object.IsKeyValid("INVALID"));
    }

    #region Clear/Remove/Dispose

    [Theory]
    [InlineData(BuilderOverrideKind.NoOverwrite)]
    [InlineData(BuilderOverrideKind.Overwrite)]
    [InlineData(BuilderOverrideKind.AllowDuplicate)]
    public void Test_Clear(BuilderOverrideKind overrideKind)
    {
        var builder = CreateBuilder(overrideKind);

        var entries = builder.Object.BuilderData;
        Assert.Empty(entries);

        builder.Object.AddEntry("key", "value");

        Assert.Single(builder.Object.BuilderData);

        builder.Object.Clear();

        Assert.Empty(builder.Object.BuilderData);
    }

    [Theory]
    [InlineData(BuilderOverrideKind.NoOverwrite)]
    [InlineData(BuilderOverrideKind.Overwrite)]
    [InlineData(BuilderOverrideKind.AllowDuplicate)]
    public void Test_Remove(BuilderOverrideKind overrideKind)
    {
        var builder = CreateBuilder(overrideKind);

        var keyEntry = builder.Object.AddEntry("key", "value");
        var key1Entry = builder.Object.AddEntry("key1", "other");

        Assert.Equal(2, builder.Object.BuilderData.Count);

        Assert.False(builder.Object.Remove(default));
        Assert.False(builder.Object.RemoveAllKeys("key3"));

        Assert.Equal(2, builder.Object.BuilderData.Count);

        Assert.True(builder.Object.Remove(new DatStringEntry("key1", key1Entry.AddedEntry!.Value.Crc32, "other")));

        Assert.Equal([
                new("key", keyEntry.AddedEntry!.Value.Crc32, "value")
            ],
            builder.Object.BuilderData.ToList());

        if (overrideKind == BuilderOverrideKind.AllowDuplicate)
        {
            builder.Object.AddEntry("key", "otherKeyValue2");
            builder.Object.AddEntry("key", "otherKeyValue3");
            Assert.Equal(3, builder.Object.BuilderData.Count);

            Assert.True(builder.Object.Remove(new DatStringEntry("key", keyEntry.AddedEntry!.Value.Crc32, "otherKeyValue2")));

            Assert.Equal(2, builder.Object.BuilderData.Count);
        }


        Assert.True(builder.Object.RemoveAllKeys("key"));

        Assert.Empty(builder.Object.BuilderData);
    }

    [Theory]
    [InlineData(BuilderOverrideKind.NoOverwrite)]
    [InlineData(BuilderOverrideKind.Overwrite)]
    [InlineData(BuilderOverrideKind.AllowDuplicate)]
    public void Test_Dispose_ThrowsOnAddingMethods(BuilderOverrideKind overrideKind)
    {
        var builder = CreateBuilder(overrideKind);

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

    [Theory]
    [InlineData(BuilderOverrideKind.NoOverwrite)]
    [InlineData(BuilderOverrideKind.Overwrite)]
    [InlineData(BuilderOverrideKind.AllowDuplicate)]
    public void Test_AddEntry_Throws(BuilderOverrideKind overrideKind)
    {
        var builder = CreateBuilder(overrideKind);

        Assert.Throws<ArgumentNullException>(() => builder.Object.AddEntry(null!, "value"));
        Assert.Throws<ArgumentNullException>(() => builder.Object.AddEntry("key", null!));
    }

    [Theory]
    [InlineData(BuilderOverrideKind.NoOverwrite)]
    [InlineData(BuilderOverrideKind.Overwrite)]
    [InlineData(BuilderOverrideKind.AllowDuplicate)]
    public void Test_AddEntry(BuilderOverrideKind overrideKind)
    {
        var builder = CreateBuilder(overrideKind);

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
        var builder = CreateBuilder(BuilderOverrideKind.NoOverwrite);

        builder.Object.AddEntry("key", "value");
        var result = builder.Object.AddEntry("key", "value1");

        Assert.False(result.Added);
        Assert.Equal(AddEntryState.NotAddedDuplicate, result.Status);
        Assert.Single(builder.Object.BuilderData);
        Assert.Equal("value", builder.Object.BuilderData.First().Value);
    }

    [Fact]
    public void Test_AddEntry_OverwriteDuplicates()
    {
        var builder = CreateBuilder(BuilderOverrideKind.Overwrite);

        builder.Object.AddEntry("key", "value");
        builder.Object.AddEntry("1", "distract");
        var result = builder.Object.AddEntry("key", "value1");

        Assert.True(result.Added);
        Assert.True(result.WasOverwrite);
        Assert.Equal("value", result.OverwrittenEntry.Value.Value);
        Assert.Equal(AddEntryState.AddedDuplicate, result.Status);
        Assert.Equal(2, builder.Object.BuilderData.Count);
        Assert.Equal("value1", builder.Object.BuilderData.First(x => x.Crc32 == result.AddedEntry.Value.Crc32).Value);
    }

    [Fact]
    public void Test_AddEntry_AllowDuplicates()
    {
        var builder = CreateBuilder(BuilderOverrideKind.AllowDuplicate);

        builder.Object.AddEntry("key", "value");
        builder.Object.AddEntry("other", "other");
        var result = builder.Object.AddEntry("key", "value1");

        Assert.True(result.Added);
        Assert.False(result.WasOverwrite);
        Assert.Null(result.OverwrittenEntry);
        Assert.Equal(AddEntryState.AddedDuplicate, result.Status);
        Assert.Equal(3, builder.Object.BuilderData.Count);

        Assert.Equal("value", builder.Object.BuilderData.First(e => e.Key == "key").Value);
        Assert.Equal("value1", builder.Object.BuilderData.Last(e => e.Key == "key").Value);
    }

    [Theory]
    [InlineData(BuilderOverrideKind.NoOverwrite)]
    [InlineData(BuilderOverrideKind.Overwrite)]
    [InlineData(BuilderOverrideKind.AllowDuplicate)]
    public void Test_AddEntry_PerformsEncoding(BuilderOverrideKind overrideKind)
    {
        var builder = CreateBuilder(overrideKind);

        var result = builder.Object.AddEntry("keyÖÄÜ", "value");

        Assert.True(result.Added);

        Assert.Equal("key???", result.AddedEntry.Value.Key);
        Assert.Equal("keyÖÄÜ", result.AddedEntry.Value.OriginalKey);
    }


    [Theory]
    [InlineData(BuilderOverrideKind.NoOverwrite)]
    [InlineData(BuilderOverrideKind.Overwrite)]
    [InlineData(BuilderOverrideKind.AllowDuplicate)]
    public void Test_AddEntry_InvalidKey(BuilderOverrideKind overrideKind)
    {
        var builder = CreateBuilder(overrideKind);

        var result = builder.Object.AddEntry("INVALID", "value");

        Assert.False(result.Added);
        Assert.Equal(AddEntryState.InvalidKey, result.Status);
    }

    #endregion

    [Theory]
    [InlineData(BuilderOverrideKind.NoOverwrite)]
    [InlineData(BuilderOverrideKind.Overwrite)]
    [InlineData(BuilderOverrideKind.AllowDuplicate)]
    public void Test_BuildModel(BuilderOverrideKind overrideKind)
    {
        var builder = CreateBuilder(overrideKind);

        builder.Object.AddEntry("key1", "value");
        builder.Object.AddEntry("key2", "value");
        builder.Object.AddEntry("key3", "value");

        var model = builder.Object.BuildModel();
        Assert.Equal(builder.Object.TargetKeySortOrder, model.KeySortOder);
        Assert.Equal(3, model.Count);
        Assert.Equal(["key1", "key2", "key3"], model.Keys);
    }

    [Theory]
    [InlineData(BuilderOverrideKind.NoOverwrite)]
    [InlineData(BuilderOverrideKind.Overwrite)]
    [InlineData(BuilderOverrideKind.AllowDuplicate)]
    public void Test_Build(BuilderOverrideKind overrideKind)
    {
        var builder = CreateBuilder(overrideKind);

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

        Assert.Equal([1,2,3], FileSystem.File.ReadAllBytes("test.dat"));
    }

    protected class TestKeyValidator : IDatKeyValidator
    {
        public bool Validate(string key)
        {
            return Validate(key.AsSpan());
        }

        public bool Validate(ReadOnlySpan<char> key)
        {
            if (key.Equals("INVALID".AsSpan(), StringComparison.Ordinal))
                return false;
            return true;
        }
    }
}