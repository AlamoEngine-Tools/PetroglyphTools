using System;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Services.Builder;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder;

[TestClass]
public class EmpireAtWarMegBuilderTest : MegBuilderTestSuite
{
    public const string BasePath = "/Games/Petroglyph/corruption/";

    protected override Type ExpectedFileInfoValidatorType => typeof(MegBuilderBase.DefaultFileInformationValidator);
    protected override Type ExpectedDataEntryValidatorType => typeof(MegBuilderBase.NotNullDataEntryValidator);
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
    public new void Test_Ctor_Throws()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(FileSystem); 
        Assert.ThrowsException<ArgumentNullException>(() => new EmpireAtWarMegBuilder(null!, sc.BuildServiceProvider()));
        Assert.ThrowsException<ArgumentException>(() => new EmpireAtWarMegBuilder("", sc.BuildServiceProvider()));
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