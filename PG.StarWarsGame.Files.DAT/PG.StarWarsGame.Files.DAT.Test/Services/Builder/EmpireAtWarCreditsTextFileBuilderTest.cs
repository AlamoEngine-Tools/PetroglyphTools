using System;
using System.IO.Abstractions;
using AnakinRaW.CommonUtilities.Hashing;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Extensibility;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Files;
using PG.StarWarsGame.Files.DAT.Services.Builder;
using PG.StarWarsGame.Files.DAT.Services.Builder.Validation;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Services.Builder;

public class EmpireAtWarCreditsTextFileBuilderTest
{
    private readonly MockFileSystem _fileSystem = new();

    private IServiceProvider CreateServiceProvider()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(_ => _fileSystem);
        sc.AddSingleton<IHashingService>(sp => new HashingService(sp));
        sc.CollectPgServiceContributions();
        return sc.BuildServiceProvider();
    }

    [Fact]
    public void Test_Ctor()
    {
        var builder = new EmpireAtWarCreditsTextFileBuilder(CreateServiceProvider());
        
        Assert.Equal(BuilderOverrideKind.AllowDuplicate,builder.KeyOverwriteBehavior);
        Assert.Equal(DatFileType.NotOrdered, builder.TargetKeySortOrder);
        Assert.IsType<EmpireAtWarKeyValidator>(builder.KeyValidator);
    }

    [Fact]
    public void Test_AddEntry_CorrectCrc()
    {
        var builder = new EmpireAtWarCreditsTextFileBuilder(CreateServiceProvider());

        var result = builder.AddEntry("TEXT_GUI_DIALOG_TOOLTIP_IDC_MAIN_MENU_SINGLE_PLAYER_GAMES", "someValue");
        Assert.Equal(new Crc32(72402613), result.AddedEntry!.Value.Crc32);

        result = builder.AddEntry("Tatooine", "someValue");
        Assert.Equal(new Crc32(-256176565), result.AddedEntry!.Value.Crc32);

        result = builder.AddEntry("Corulag", "someValue");
        Assert.Equal(new Crc32(539193933), result.AddedEntry!.Value.Crc32);
    }
}