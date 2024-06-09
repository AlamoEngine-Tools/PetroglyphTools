using System;

using System.Collections.Generic;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;
using Testably.Abstractions.Testing;
using System.Reflection;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder.Validation;


public class PetroglyphMegDataEntryValidatorTest
{
    private readonly TestPetroglyphMegDataEntryValidator _validator;

    public PetroglyphMegDataEntryValidatorTest()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(new MockFileSystem());
        _validator = new TestPetroglyphMegDataEntryValidator(sc.BuildServiceProvider());
    }

    [Theory]
    [MemberData(nameof(ValidTestData))]
    public void TestValid(MegFileDataEntryBuilderInfo builderInfo)
    {
        Assert.True(_validator.Validate(builderInfo));
    }

    [Theory]
    [MemberData(nameof(InvalidTestData), MemberType = typeof(NotNullDataEntryValidatorTest))]
    [MemberData(nameof(InvalidTestData))]
    public void TestInvalid(MegFileDataEntryBuilderInfo builderInfo)
    {
        Assert.False(_validator.Validate(builderInfo));
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
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("/"))];

        // We do not allow directory operators
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("./test.txt"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("a/./test.txt"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("../test.txt"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("a/../test.txt"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("a/.."))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo(".."))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("."))];

        // We do not allow absolute, rooted or URI paths
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("C:/test.txt"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("C:test.txt"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("C:test.txt"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("/test.txt"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("/test.txt"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("file://test.txt"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo(@"\\Server2\Share\Test\Foo.txt\t"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("//Server2/Share/Test/Foo.txt/t"))];

        // We do not allow paths with are longer than 256 characters, as that's the default Windows limit.
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo(new string('a', 261)))];

        // Because XML parsing is sometimes done on space as delimiter, we cannot use them
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("thisIsAPathWithA Space"))];
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("my/path with/space"))];
    }

    public static string GetValidationDataDisplayName(MethodInfo methodInfo, object[] data)
    {
        var builderInfo = data[0] as MegFileDataEntryBuilderInfo;
        var filePath = builderInfo?.FilePath;

        if (filePath is not null && filePath.Length > 30)
            filePath = filePath.Substring(0, 30) + "..." + filePath.Length + "]";

        return $"{methodInfo.Name} ({filePath})";
    }

    private class TestPetroglyphMegDataEntryValidator(IServiceProvider serviceProvider)
        : PetroglyphMegDataEntryValidator(serviceProvider);
}