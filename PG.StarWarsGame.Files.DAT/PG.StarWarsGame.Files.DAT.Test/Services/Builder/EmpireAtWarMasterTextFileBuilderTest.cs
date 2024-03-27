using System;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.DAT.Files;
using PG.StarWarsGame.Files.DAT.Services.Builder;
using PG.StarWarsGame.Files.DAT.Services.Builder.Validation;
using Testably.Abstractions.Testing;

namespace PG.StarWarsGame.Files.DAT.Test.Services.Builder;

[TestClass]
public class EmpireAtWarMasterTextFileBuilderTest
{
    private readonly MockFileSystem _fileSystem = new();

    private IServiceProvider CreateServiceProvider()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(_ => _fileSystem);
        DatDomain.RegisterServices(sc);
        return sc.BuildServiceProvider();
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void Test_Ctor(bool overwrite)
    {
        var builder = new EmpireAtWarMasterTextFileBuilder(overwrite, CreateServiceProvider());

        Assert.AreEqual(overwrite ? BuilderOverrideKind.Overwrite : BuilderOverrideKind.NoOverwrite, builder.KeyOverwriteBehavior);
        Assert.AreEqual(DatFileType.OrderedByCrc32, builder.TargetKeySortOrder);
        Assert.IsInstanceOfType<EmpireAtWarKeyValidator>(builder.KeyValidator);
    }
}