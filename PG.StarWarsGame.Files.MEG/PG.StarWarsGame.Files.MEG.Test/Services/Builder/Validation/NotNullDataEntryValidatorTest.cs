using System;
using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder.Validation;

public class NotNullDataEntryValidatorTest
{
    private readonly NotNullDataEntryValidator _validator = NotNullDataEntryValidator.Instance;

    [Theory]
    [MemberData(nameof(ValidTestData))]
    public void TestValid(MegFileDataEntryBuilderInfo builderInfo)
    {
        Assert.True(_validator.Validate(builderInfo));
        Assert.True(_validator.Validate(builderInfo.FilePath.AsSpan(), builderInfo.Encrypted, builderInfo.Size));
    }

    [Theory]
    [MemberData(nameof(InvalidTestData))]
    public void TestInvalid(MegFileDataEntryBuilderInfo builderInfo)
    {
        Assert.False(_validator.Validate(builderInfo));
        if (builderInfo is not null) 
            Assert.False(_validator.Validate(builderInfo.FilePath.AsSpan(), builderInfo.Encrypted, builderInfo.Size));
    }

    public static IEnumerable<object[]> ValidTestData()
    {
        yield return [new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path"))];
    }

    public static IEnumerable<object?[]> InvalidTestData()
    {
        yield return [null];
    }
}