using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;
using Testably.Abstractions.Testing;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder.Validation;

[TestClass]
public class PetroglyphMegDataEntryValidatorTest
{
    private TestPetroglyphMegDataEntryValidator _validator;

    [TestInitialize]
    public void Setup()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(new MockFileSystem());
        _validator = new TestPetroglyphMegDataEntryValidator(sc.BuildServiceProvider());
    }

    [TestMethod]
    [DynamicData(nameof(ValidTestData), DynamicDataSourceType.Method)]
    public void TestValid(MegFileDataEntryBuilderInfo builderInfo)
    {
        Assert.IsTrue(_validator.Validate(builderInfo).IsValid);
    }

    [TestMethod]
    [DynamicData(nameof(InvalidTestData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(NotNullDataEntryValidatorTest))]
    [DynamicData(nameof(InvalidTestData), DynamicDataSourceType.Method)]
    public void TestInvalid(MegFileDataEntryBuilderInfo builderInfo)
    {
        Assert.IsFalse(_validator.Validate(builderInfo).IsValid);
    }

    public static IEnumerable<object[]> ValidTestData()
    {
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo(".path"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path/text.txt"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path\\text.txt"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path"), overrideEncrypted: true)];
    }

    public static IEnumerable<object[]> InvalidTestData()
    {
        // We do not allow directory names
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("dir/"))];

        // We do not allow directory operators
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("./test.txt"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("../test.txt"))];

        // We do not allow absolute, rooted or URI paths
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("C:/test.txt"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("C:test.txt"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("C:test.txt"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("/test.txt"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("/test.txt"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("file://test.txt"))];

        // We do not allow paths with are longer than 256 characters, as that's the default Windows limit.
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo(new string('a', 256)))];
    }

    private class TestPetroglyphMegDataEntryValidator(IServiceProvider serviceProvider)
        : PetroglyphMegDataEntryValidator(serviceProvider);
}