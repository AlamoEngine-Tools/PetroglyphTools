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
public class EmpireAtWarCreditsTextFileBuilderTest
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
    public void Test_Ctor()
    {
        var builder = new EmpireAtWarCreditsTextFileBuilder(CreateServiceProvider());
        
        Assert.AreEqual(BuilderOverrideKind.AllowDuplicate,builder.KeyOverwriteBehavior);
        Assert.AreEqual(DatFileType.NotOrdered, builder.TargetKeySortOrder);
        Assert.IsInstanceOfType<EmpireAtWarKeyValidator>(builder.KeyValidator);
    }
}