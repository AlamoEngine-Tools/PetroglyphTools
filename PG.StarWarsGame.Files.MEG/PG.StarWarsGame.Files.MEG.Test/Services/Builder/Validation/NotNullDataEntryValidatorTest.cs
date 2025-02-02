using System;
using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;
using PG.Testing;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder.Validation;

public class NotNullDataEntryValidatorTest
{
    private readonly NotNullDataEntryValidator _validator = NotNullDataEntryValidator.Instance;

    [Theory]
    [MemberData(nameof(ValidTestData))]
    public void Validate_ValidData(MegFileDataEntryBuilderInfo builderInfo)
    {
        Assert.True(_validator.Validate(builderInfo));
        Assert.True(_validator.Validate(builderInfo.FilePath.AsSpan(), builderInfo.Encrypted, builderInfo.Size));
    }

    [Theory]
    [MemberData(nameof(InvalidTestData))]
    public void Validate_InvalidData(MegFileDataEntryBuilderInfo builderInfo)
    {
        Assert.False(_validator.Validate(builderInfo));
        if (builderInfo is not null) 
            Assert.False(_validator.Validate(builderInfo.FilePath.AsSpan(), builderInfo.Encrypted, builderInfo.Size));
    }

    [Fact]
    public void Validate_Span()
    {
        var random = new Random();
        Assert.False(_validator.Validate(ReadOnlySpan<char>.Empty, random.Next() % 2 == 0, random.Next() % 2 == 0 ? (uint)random.Next() : null));
        Assert.True(_validator.Validate(TestUtility.GetRandomStringOfLength(12).AsSpan(), random.Next() % 2 == 0, random.Next() % 2 == 0 ? (uint)random.Next() : null));
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