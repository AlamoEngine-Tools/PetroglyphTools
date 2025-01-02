using System;
using System.IO.Abstractions;
using AnakinRaW.CommonUtilities.Hashing;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.DAT.Files;
using PG.StarWarsGame.Files.DAT.Services.Builder;
using PG.StarWarsGame.Files.DAT.Services.Builder.Validation;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Services.Builder;

public class EmpireAtWarMasterTextBuilderTest
{
    private readonly MockFileSystem _fileSystem = new();

    private IServiceProvider CreateServiceProvider()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(_ => _fileSystem);
        sc.AddSingleton<IHashingService>(sp => new HashingService(sp));
        PetroglyphCommons.ContributeServices(sc);
        sc.SupportDAT();
        return sc.BuildServiceProvider();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_Ctor(bool overwrite)
    {
        var builder = new EmpireAtWarMasterTextBuilder(overwrite, CreateServiceProvider());

        Assert.Equal(overwrite ? BuilderOverrideKind.Overwrite : BuilderOverrideKind.NoOverwrite, builder.KeyOverwriteBehavior);
        Assert.Equal(DatFileType.OrderedByCrc32, builder.TargetKeySortOrder);
        Assert.IsType<EmpireAtWarKeyValidator>(builder.KeyValidator);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Test_AddEntry_CorrectCrc(bool overwrite)
    {
        var builder = new EmpireAtWarMasterTextBuilder(overwrite, CreateServiceProvider());

        var result = builder.AddEntry("TEXT_GUI_DIALOG_TOOLTIP_IDC_MAIN_MENU_SINGLE_PLAYER_GAMES", "someValue");
        Assert.Equal(new Crc32(72402613), result.AddedEntry!.Value.Crc32);

        result = builder.AddEntry("Tatooine", "someValue");
        Assert.Equal(new Crc32(-256176565), result.AddedEntry!.Value.Crc32);

        result = builder.AddEntry("Corulag", "someValue");
        Assert.Equal(new Crc32(539193933), result.AddedEntry!.Value.Crc32);

        var dupResult = builder.AddEntry("Corulag", "someOtherValue");
        if (overwrite)
        {
            Assert.True(dupResult.Added);
            Assert.Equal("someOtherValue", dupResult.AddedEntry.Value.Value);
            Assert.Equal("someValue", dupResult.OverwrittenEntry!.Value.Value);
        }
        else
        {
            Assert.False(dupResult.Added);
        }
    }
}