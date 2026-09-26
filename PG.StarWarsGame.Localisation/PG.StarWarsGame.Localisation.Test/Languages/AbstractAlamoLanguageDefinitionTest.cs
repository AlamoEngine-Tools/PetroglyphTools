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
        Assert.True(lang.IsOfficiallySupported(GameContext.EaW));
        Assert.True(lang.IsOfficiallySupported(GameContext.FoC));
    }

    [Fact]
    public void IsOfficiallySupported_WithContext_ReturnsFalse_WhenLanguageHasNoAttribute()
    {
        var lang = new UnsupportedTestLanguage();
        Assert.False(lang.IsOfficiallySupported(GameContext.EaW));
        Assert.False(lang.IsOfficiallySupported(GameContext.FoC));
    }

    [Fact]
    public void IsOfficiallySupported_WithContext_ReturnsTrue_OnlyForMatchingContext()
    {
        var baseOnly = new BaseGameOnlyTestLanguage();
        Assert.True(baseOnly.IsOfficiallySupported(GameContext.EaW));
        Assert.False(baseOnly.IsOfficiallySupported(GameContext.FoC));

        var expOnly = new ExpansionOnlyTestLanguage();
        Assert.False(expOnly.IsOfficiallySupported(GameContext.EaW));
        Assert.True(expOnly.IsOfficiallySupported(GameContext.FoC));
    }

    private sealed class UnsupportedTestLanguage : AlamoLanguageDefinitionBase
    {
        protected override string ConfiguredLanguageIdentifier => "KLINGON";
        protected override CultureInfo ConfiguredCulture => CultureInfo.InvariantCulture;
    }

    [OfficiallySupportedLanguage(GameContext.EaW)]
    private sealed class BaseGameOnlyTestLanguage : AlamoLanguageDefinitionBase
    {
        protected override string ConfiguredLanguageIdentifier => "BASE_ONLY";
        protected override CultureInfo ConfiguredCulture => CultureInfo.InvariantCulture;
    }

    [OfficiallySupportedLanguage(GameContext.FoC)]
    private sealed class ExpansionOnlyTestLanguage : AlamoLanguageDefinitionBase
    {
        protected override string ConfiguredLanguageIdentifier => "EXP_ONLY";
        protected override CultureInfo ConfiguredCulture => CultureInfo.InvariantCulture;
    }
}
