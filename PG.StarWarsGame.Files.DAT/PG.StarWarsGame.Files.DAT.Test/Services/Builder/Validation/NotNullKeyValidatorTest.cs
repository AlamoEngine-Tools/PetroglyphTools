using System.Collections.Generic;
using PG.StarWarsGame.Files.DAT.Services.Builder.Validation;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Services.Builder.Validation;

public class NotNullKeyValidatorTest
{
    private readonly NotNullKeyValidator _validator = NotNullKeyValidator.Instance;

    [Theory]
    [MemberData(nameof(ValidTestData))]
    public void TestValid(string key)
    {
        Assert.True(_validator.Validate(key));
    }

    [Theory]
    [MemberData(nameof(InvalidTestData))]
    public void TestInvalid(string key)
    {
        Assert.False(_validator.Validate(key));
    }

    public static IEnumerable<object[]> ValidTestData()
    {
        yield return ["123"];
    }

    public static IEnumerable<object[]> InvalidTestData()
    {
        yield return [null];
    }
}