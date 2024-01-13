using System;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Services.Builder;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder;

[TestClass]
public class EmpireAtWarMegBuilderTest : PetroglyphGameMegBuilderTest
{
    protected override Type ExpectedFileInfoValidatorType => typeof(DefaultFileInformationValidator);
    protected override Type ExpectedDataEntryValidatorType => typeof(NotNullDataEntryValidator);
    protected override Type? ExpectedDataEntryPathNormalizerType => null;
    protected override bool? ExpectedOverwritesDuplicates => true;
    protected override bool? ExpectedAutomaticallyAddFileSizes => true;

    private EmpireAtWarMegBuilder CreateEaWBuilder(IServiceProvider serviceProvider)
    {
        return new EmpireAtWarMegBuilder(BasePath, serviceProvider);
    }

    protected override MegBuilderBase CreateBuilder(IServiceProvider serviceProvider)
    {
        return CreateEaWBuilder(serviceProvider);
    }

    [TestMethod]
    public new void Test_Ctor()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(FileSystem);
        var builder = CreateEaWBuilder(sc.BuildServiceProvider());
        Assert.AreEqual(FileSystem.Path.GetFullPath(BasePath), builder.BaseDirectory);
    }
}