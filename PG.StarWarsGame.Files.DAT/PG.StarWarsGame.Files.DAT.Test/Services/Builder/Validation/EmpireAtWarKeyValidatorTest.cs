﻿using System;
using System.Collections.Generic;
using PG.StarWarsGame.Files.DAT.Services.Builder.Validation;
using Xunit;

namespace PG.StarWarsGame.Files.DAT.Test.Services.Builder.Validation;

public class EmpireAtWarKeyValidatorTest
{
    private readonly EmpireAtWarKeyValidator _validator = EmpireAtWarKeyValidator.Instance;

    [Theory]
    [MemberData(nameof(ValidTestData))]
    public void TestValid(string key)
    {
        Assert.True(_validator.Validate(key));
        Assert.True(_validator.Validate(key.AsSpan()));
    }

    [Theory]
    [MemberData(nameof(InvalidTestData))]
    public void TestInvalid(string key)
    {
        Assert.False(_validator.Validate(key));
        Assert.False(_validator.Validate(key.AsSpan()));
    }

    public static IEnumerable<object[]> ValidTestData()
    {
        yield return ["123"];
        yield return ["TEST_KEY-value 123"];
    }

    public static IEnumerable<object[]> InvalidTestData()
    {
        yield return [null!];
        yield return [""];
        yield return ["  leadingSpace"];
        yield return ["trailingSpace  "];
        yield return ["   "];
        yield return ["ÖÄÜnonASCII"];
        yield return ["😊"];
        for (var i = 0; i < 20; i++)
        {
            yield return [$"key{(char)i}key"];
        }
    }
}