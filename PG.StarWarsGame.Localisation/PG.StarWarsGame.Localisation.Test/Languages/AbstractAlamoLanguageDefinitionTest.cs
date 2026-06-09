// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Globalization;
using PG.StarWarsGame.Localisation.Languages;
using Xunit;

namespace PG.StarWarsGame.Localisation.Test.Languages;

public class AbstractAlamoLanguageDefinitionTest
{
    [Fact]
    public void LanguageIdentifier_ReturnsConfiguredValue()
    {
        var lang = new EnglishAlamoLanguageDefinition();
        Assert.Equal("ENGLISH", lang.LanguageIdentifier);
    }

    [Fact]
    public void Culture_ReturnsConfiguredCulture()
    {
        var lang = new EnglishAlamoLanguageDefinition();
        Assert.Equal(CultureInfo.GetCultureInfo("en-US"), lang.Culture);
    }

    [Fact]
    public void IsOfficiallySupported_IsTrue_WhenMarkedWithAttribute()
    {
        var lang = new EnglishAlamoLanguageDefinition();
        Assert.True(lang.IsOfficiallySupported);
    }

    [Fact]
    public void IsDefault_IsTrue_OnlyForEnglish()
    {
        var english = new EnglishAlamoLanguageDefinition();
        var german = new GermanAlamoLanguageDefinition();
        Assert.True(english.IsDefault);
        Assert.False(german.IsDefault);
    }

    [Fact]
    public void Equals_ReturnsTrueForSameLanguageIdentifier()
    {
        var a = new EnglishAlamoLanguageDefinition();
        var b = new EnglishAlamoLanguageDefinition();
        Assert.Equal(a, b);
    }

    [Fact]
    public void Equals_ReturnsFalseForDifferentLanguages()
    {
        var english = new EnglishAlamoLanguageDefinition();
        var german = new GermanAlamoLanguageDefinition();
        Assert.NotEqual<IAlamoLanguageDefinition>(english, german);
    }
}
