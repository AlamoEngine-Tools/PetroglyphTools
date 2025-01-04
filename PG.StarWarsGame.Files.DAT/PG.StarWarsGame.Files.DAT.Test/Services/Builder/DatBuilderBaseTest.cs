using System;
using System.Collections.Generic;
using System.Linq;
using AnakinRaW.CommonUtilities.Hashing;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;
using PG.StarWarsGame.Files.DAT.Services.Builder;
using PG.StarWarsGame.Files.DAT.Test.Services.Builder.Validation;
using PG.StarWarsGame.Files.Test.Services.Builder;
using PG.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Services.Builder;

public abstract class DatBuilderBaseTest : FileBuilderTestBase<DatBuilderBase, IReadOnlyList<DatStringEntry>, DatFileInformation>
{
    protected override string DefaultFileName => "textfile.dat";

    protected abstract BuilderOverrideKind OverrideKind { get; }

    protected override bool FileInfoIsAlwaysValid => true;

    protected override void BuildServices(IServiceCollection sc)
    {
        base.BuildServices(sc);
        sc.AddSingleton<IHashingService>(sp => new HashingService(sp));
        PetroglyphCommons.ContributeServices(sc);
        sc.SupportDAT();
    }

    [Fact]
    public void IsKeyValid()
    {
        var builder = CreateBuilder();

        Assert.Throws<ArgumentNullException>(() => builder.IsKeyValid(null!));
        Assert.True(builder.IsKeyValid(TestUtility.GetRandomStringOfLength(12)));
        Assert.False(builder.IsKeyValid((string)TestUtility.GetRandom(EmpireAtWarKeyValidatorTest.InvalidTestData())[0]));
    }

    #region Clear/Remove/Dispose

    [Fact]
    public void Clear()
    {
        var builder = CreateBuilder();

        var entries = builder.BuilderData;
        Assert.Empty(entries);

        builder.AddEntry("key", "value");

        Assert.Single(builder.BuilderData);

        builder.Clear();

        Assert.Empty(builder.BuilderData);
    }

    [Fact]
    public void Remove_RemoveAll()
    {
        var builder = CreateBuilder();

        var keyEntry = builder.AddEntry("key", "value");
        var key1Entry = builder.AddEntry("key1", "other");

        Assert.Equal(2, builder.BuilderData.Count);

        Assert.False(builder.Remove(default));
        Assert.False(builder.RemoveAllKeys("key3"));

        Assert.Equal(2, builder.BuilderData.Count);

        Assert.True(builder.Remove(new DatStringEntry("key1", key1Entry.AddedEntry!.Value.Crc32, "other")));

        Assert.Equal(
            [new("key", keyEntry.AddedEntry!.Value.Crc32, "value")],
            builder.BuilderData.ToList());

        if (OverrideKind == BuilderOverrideKind.AllowDuplicate)
        {
            builder.AddEntry("key", "otherKeyValue2");
            builder.AddEntry("key", "otherKeyValue3");
           
            Assert.Equal(3, builder.BuilderData.Count);
            Assert.True(builder.Remove(new DatStringEntry("key", keyEntry.AddedEntry!.Value.Crc32, "otherKeyValue2")));
            Assert.Equal(2, builder.BuilderData.Count);
        }

        Assert.True(builder.RemoveAllKeys("key"));
        Assert.Empty(builder.BuilderData);
    }

    [Fact]
    public void Dispose_ThrowsOnAddingMethods()
    {
        var builder = CreateBuilder();

        builder.Dispose();

        Assert.Empty(builder.BuilderData);

        Assert.Throws<ObjectDisposedException>(() => builder.AddEntry("key", "value"));

        ExceptionUtilities.AssertDoesNotThrowException(() => builder.Entries);
        ExceptionUtilities.AssertDoesNotThrowException(builder.Clear);
        ExceptionUtilities.AssertDoesNotThrowException(() => builder.Remove(new DatStringEntry()));
        ExceptionUtilities.AssertDoesNotThrowException(() => builder.RemoveAllKeys("key"));

        ExceptionUtilities.AssertDoesNotThrowException(builder.Dispose);
    }

    #endregion

    #region AddEntry

    [Fact]
    public void AddEntry_Throws()
    {
        var builder = CreateBuilder();
        Assert.Throws<ArgumentNullException>(() => builder.AddEntry(null!, "value"));
        Assert.Throws<ArgumentNullException>(() => builder.AddEntry("key", null!));
    }

    [Theory]
    [InlineData("key")]
    [InlineData("key_with-unusual.but All0w3d:Chars")]
    public void AddEntry_SingleEntry(string key)
    {
        var builder = CreateBuilder();

        var result = builder.AddEntry(key, "value");

        Assert.True(result.Added);
        Assert.False(result.WasOverwrite);
        Assert.Equal(AddEntryState.Added, result.Status);
        Assert.Single(builder.BuilderData);
        Assert.Equal("value", builder.BuilderData.First().Value);
    }

    [Fact]
    public void AddEntry_DoNotAllowDuplicates()
    {
        if (OverrideKind != BuilderOverrideKind.NoOverwrite)
            return;
        var builder = CreateBuilder();

        builder.AddEntry("key", "value");
        var result = builder.AddEntry("key", "value1");

        Assert.False(result.Added);
        Assert.Equal(AddEntryState.NotAddedDuplicate, result.Status);
        Assert.Single(builder.BuilderData);
        Assert.Equal("value", builder.BuilderData.First().Value);
    }

    [Fact]
    public void AddEntry_OverwriteDuplicates()
    {
        if (OverrideKind != BuilderOverrideKind.Overwrite)
            return;

        var builder = CreateBuilder();

        builder.AddEntry("key", "value");
        builder.AddEntry("1", "distract");
        var result = builder.AddEntry("key", "value1");

        Assert.True(result.Added);
        Assert.True(result.WasOverwrite);
        Assert.Equal("value", result.OverwrittenEntry.Value.Value);
        Assert.Equal(AddEntryState.AddedDuplicate, result.Status);
        Assert.Equal(2, builder.BuilderData.Count);
        Assert.Equal("value1", builder.BuilderData.First(x => x.Crc32 == result.AddedEntry.Value.Crc32).Value);
    }

    [Fact]
    public void AddEntry_AllowDuplicates()
    {
        if (OverrideKind != BuilderOverrideKind.AllowDuplicate)
            return;

        var builder = CreateBuilder();

        builder.AddEntry("key", "value");
        builder.AddEntry("other", "other");
        var result = builder.AddEntry("key", "value1");

        Assert.True(result.Added);
        Assert.False(result.WasOverwrite);
        Assert.Null(result.OverwrittenEntry);
        Assert.Equal(AddEntryState.AddedDuplicate, result.Status);
        Assert.Equal(3, builder.BuilderData.Count);

        Assert.Equal("value", builder.BuilderData.First(e => e.Key == "key").Value);
        Assert.Equal("value1", builder.BuilderData.Last(e => e.Key == "key").Value);
    }


    [Fact]
    public void AddEntry_PerformsEncoding()
    {
        var builder = CreateBuilder();

        var result = builder.AddEntry("keyÖÄÜ", "value");

        Assert.True(result.Added);

        Assert.Equal("key???", result.AddedEntry.Value.Key);
        Assert.Equal("keyÖÄÜ", result.AddedEntry.Value.OriginalKey);
    }


    [Fact]
    public void AddEntry_InvalidKey()
    {
        var builder = CreateBuilder();

        var result = builder.AddEntry("INVALID\tKey", "value");

        Assert.False(result.Added);
        Assert.Equal(AddEntryState.InvalidKey, result.Status);
    }

    #endregion

    [Fact]
    public void BuildModel()
    {
        var builder = CreateBuilder();

        builder.AddEntry("key1", "value");
        builder.AddEntry("key2", "value");
        builder.AddEntry("key3", "value");

        var model = builder.BuildModel();
        Assert.Equal(builder.TargetKeySortOrder, model.KeySortOrder);
        Assert.Equal(3, model.Count);
        Assert.Equal(["key1", "key2", "key3"], model.Keys);
    }
}