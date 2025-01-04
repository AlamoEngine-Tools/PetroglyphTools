using System;
using System.Collections.Generic;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Data;
using PG.StarWarsGame.Files.DAT.Files;
using PG.StarWarsGame.Files.DAT.Services.Builder;
using PG.StarWarsGame.Files.DAT.Services.Builder.Validation;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Services.Builder;

public class EmpireAtWarCreditsTextBuilderTest : DatBuilderBaseTest
{
    protected override BuilderOverrideKind OverrideKind => BuilderOverrideKind.AllowDuplicate;

    protected override DatBuilderBase CreateBuilder()
    {
        return new EmpireAtWarCreditsTextBuilder(ServiceProvider);
    }

    protected override DatFileInformation CreateFileInfo(bool valid, string path)
    {
        return new DatFileInformation
        {
            FilePath = valid ? path : string.Empty
        };
    }

    protected override void AddDataToBuilder(IReadOnlyList<DatStringEntry> data, DatBuilderBase builder)
    {
        foreach (var entry in data) 
            builder.AddEntry(entry.Key, entry.Value);
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
    public void Test_Ctor()
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
    public void Test_AddEntry_CorrectCrc()
    {
        var builder = CreateBuilder();

        var result = builder.AddEntry("TEXT_GUI_DIALOG_TOOLTIP_IDC_MAIN_MENU_SINGLE_PLAYER_GAMES", "someValue");
        Assert.Equal(new Crc32(72402613), result.AddedEntry!.Value.Crc32);

        result = builder.AddEntry("Tatooine", "someValue");
        Assert.Equal(new Crc32(-256176565), result.AddedEntry!.Value.Crc32);

        result = builder.AddEntry("Corulag", "someValue");
        Assert.Equal(new Crc32(539193933), result.AddedEntry!.Value.Crc32);

        result = builder.AddEntry("Corulag", "someOtherValue");
        Assert.Equal(new Crc32(539193933), result.AddedEntry!.Value.Crc32);

        Assert.Equal(4, builder.BuilderData.Count);
    }
}