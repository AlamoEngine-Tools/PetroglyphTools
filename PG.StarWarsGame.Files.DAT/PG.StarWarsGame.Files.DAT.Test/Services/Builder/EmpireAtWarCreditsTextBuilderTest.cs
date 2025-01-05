using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Linq;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;
using PG.StarWarsGame.Files.DAT.Services;
using PG.StarWarsGame.Files.DAT.Services.Builder;
using PG.StarWarsGame.Files.DAT.Services.Builder.Validation;
using PG.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Services.Builder;

public class EmpireAtWarCreditsTextBuilderTest : DatBuilderBaseTest
{
    protected override bool IsOrderedBuilder => false;

    protected override BuilderOverrideKind OverrideKind => BuilderOverrideKind.AllowDuplicate;

    protected override DatBuilderBase CreateBuilder()
    {
        return new EmpireAtWarCreditsTextBuilder(ServiceProvider);
    }

    protected override (IReadOnlyList<DatStringEntry> Data, byte[] Bytes) CreateValidData()
    {
        var random = new Random().Next();

        IReadOnlyList<DatStringEntry> model;
        byte[] bytes;
        if (random % 2 == 0)
        {
            bytes = DatTestData.CreateUnsortedBinary().Bytes; 
            model = DatTestData.CreateUnsortedModel();
        }
        else
        {
            bytes = DatTestData.CreateSortedBinary().Bytes;
            model = DatTestData.CreateSortedModel();
        }
        return (model, bytes);
    }

    [Fact]
    public void Ctor()
    {
        var builder = CreateBuilder();
        Assert.NotNull(builder.BuilderData);
        Assert.NotNull(builder.SortedEntries);
        Assert.NotNull(builder.Entries);
        Assert.Equal(BuilderOverrideKind.AllowDuplicate, builder.KeyOverwriteBehavior);
        Assert.Equal(DatFileType.NotOrdered, builder.TargetKeySortOrder);
        Assert.IsType<EmpireAtWarKeyValidator>(builder.KeyValidator);
    }

    [Fact]
    public void IntegrationTest_Unsorted_Credits_CreateFromModelAndBuild()
    {
        using (var fs = FileSystem.FileStream.New("Credits.dat", FileMode.Create))
        {
            using var stream = TestUtility.GetEmbeddedResource(typeof(DatFileServiceTest), "Files.creditstext_english.dat");
            stream.CopyTo(fs);
        }

        var creditsModel = ServiceProvider.GetRequiredService<IDatFileService>().LoadAs("Credits.dat", DatFileType.NotOrdered).Content;

        var builder = CreateBuilder();

        foreach (var entry in creditsModel)
        {
            var addedResult = builder.AddEntry(entry.Key, entry.Value);
            Assert.True(addedResult.Added);
            Assert.False(addedResult.WasOverwrite);
            Assert.Equal(entry, addedResult.AddedEntry);
        }

        Assert.Equal(creditsModel.ToList(), builder.BuildModel().ToList());

        builder.Build(CreateFileInfo(true, DefaultFileName), false);

        Assert.Equal(FileSystem.File.ReadAllBytes("Credits.dat"), FileSystem.File.ReadAllBytes(DefaultFileName));
    }
}