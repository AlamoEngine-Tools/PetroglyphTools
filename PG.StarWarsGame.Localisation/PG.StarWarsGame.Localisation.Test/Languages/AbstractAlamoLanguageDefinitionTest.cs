// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Globalization;
using PG.StarWarsGame.Localisation.Languages;
using PG.StarWarsGame.Localisation.Languages.Attributes;
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
        Assert.True(lang.IsOfficiallySupported());
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

    [Fact]
    public void IsOfficiallySupported_WithContext_ReturnsTrueForBothContexts_WhenNoContextsSpecified()
    {
        var lang = new EnglishAlamoLanguageDefinition();
        Assert.True(lang.IsOfficiallySupported(AlamoGameContext.BaseGame));
        Assert.True(lang.IsOfficiallySupported(AlamoGameContext.Expansion));
    }

    [Fact]
    public void IsOfficiallySupported_WithContext_ReturnsFalse_WhenLanguageHasNoAttribute()
    {
        var lang = new UnsupportedTestLanguage();
        Assert.False(lang.IsOfficiallySupported(AlamoGameContext.BaseGame));
        Assert.False(lang.IsOfficiallySupported(AlamoGameContext.Expansion));
    }

    [Fact]
    public void IsOfficiallySupported_WithContext_ReturnsTrue_OnlyForMatchingContext()
    {
        var baseOnly = new BaseGameOnlyTestLanguage();
        Assert.True(baseOnly.IsOfficiallySupported(AlamoGameContext.BaseGame));
        Assert.False(baseOnly.IsOfficiallySupported(AlamoGameContext.Expansion));

        var expOnly = new ExpansionOnlyTestLanguage();
        Assert.False(expOnly.IsOfficiallySupported(AlamoGameContext.BaseGame));
        Assert.True(expOnly.IsOfficiallySupported(AlamoGameContext.Expansion));
    }

    private sealed class UnsupportedTestLanguage : AlamoLanguageDefinitionBase
    {
        protected override string ConfiguredLanguageIdentifier => "KLINGON";
        protected override CultureInfo ConfiguredCulture => CultureInfo.InvariantCulture;
    }

    [OfficiallySupportedLanguage(AlamoGameContext.BaseGame)]
    private sealed class BaseGameOnlyTestLanguage : AlamoLanguageDefinitionBase
    {
        protected override string ConfiguredLanguageIdentifier => "BASE_ONLY";
        protected override CultureInfo ConfiguredCulture => CultureInfo.InvariantCulture;
    }

    [OfficiallySupportedLanguage(AlamoGameContext.Expansion)]
    private sealed class ExpansionOnlyTestLanguage : AlamoLanguageDefinitionBase
    {
        protected override string ConfiguredLanguageIdentifier => "EXP_ONLY";
        protected override CultureInfo ConfiguredCulture => CultureInfo.InvariantCulture;
    }
}
