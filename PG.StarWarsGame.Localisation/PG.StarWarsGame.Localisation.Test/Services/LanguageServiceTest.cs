// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Globalization;
using PG.StarWarsGame.Localisation.Languages;
using PG.StarWarsGame.Localisation.Languages.Attributes;
using PG.StarWarsGame.Localisation.Services;
using Xunit;

namespace PG.StarWarsGame.Localisation.Test.Services;

public class LanguageServiceTest
{
    private static ILanguageService CreateService(IEnumerable<IAlamoLanguageDefinition>? langs = null)
    {
        var all = langs ?? new IAlamoLanguageDefinition[]
        {
            new EnglishAlamoLanguageDefinition(),
            new GermanAlamoLanguageDefinition(),
            new FrenchAlamoLanguageDefinition(),
            new SpanishAlamoLanguageDefinition(),
            new ItalianAlamoLanguageDefinition(),
            new ChineseAlamoLanguageDefinition(),
            new JapaneseAlamoLanguageDefinition(),
            new KoreanAlamoLanguageDefinition(),
            new PolishAlamoLanguageDefinition(),
            new RussianAlamoLanguageDefinition(),
            new ThaiAlamoLanguageDefinition(),
        };
        return new LanguageService(all);
    }

    [Fact]
    public void AllLanguages_ReturnsAllRegistered()
    {
        var svc = CreateService();
        Assert.Equal(11, svc.AllLanguages.Count);
    }

    [Fact]
    public void OfficiallySupported_ReturnsOnlySupported()
    {
        var svc = CreateService();
        Assert.All(svc.OfficiallySupported(), l => Assert.True(l.IsOfficiallySupported()));
    }

    [Fact]
    public void Default_ReturnsEnglish()
    {
        var svc = CreateService();
        Assert.Equal("ENGLISH", svc.Default.LanguageIdentifier);
    }

    [Fact]
    public void Default_Throws_WhenNoDefaultRegistered()
    {
        var svc = new LanguageService(new[] { new GermanAlamoLanguageDefinition() });
        Assert.Throws<InvalidOperationException>(() => _ = svc.Default);
    }

    [Fact]
    public void TryGetByIdentifier_ReturnsTrue_OnExactMatch()
    {
        var svc = CreateService();
        Assert.True(svc.TryGetByIdentifier("ENGLISH", out var lang));
        Assert.NotNull(lang);
        Assert.Equal("ENGLISH", lang!.LanguageIdentifier);
    }

    [Fact]
    public void TryGetByIdentifier_IsCaseInsensitive()
    {
        var svc = CreateService();
        Assert.True(svc.TryGetByIdentifier("english", out var lang));
        Assert.NotNull(lang);
    }

    [Fact]
    public void TryGetByIdentifier_ReturnsFalse_WhenNotFound()
    {
        var svc = CreateService();
        Assert.False(svc.TryGetByIdentifier("KLINGON", out var lang));
        Assert.Null(lang);
    }

    [Fact]
    public void IsOfficiallySupported_ReturnsTrue_ForSupportedLanguage()
    {
        var svc = CreateService();
        Assert.True(svc.IsOfficiallySupported(new EnglishAlamoLanguageDefinition()));
    }

    [Fact]
    public void IsOfficiallySupported_ReturnsFalse_ForUnsupportedLanguage()
    {
        var svc = CreateService();
        var unsupported = new UnsupportedTestLanguage();
        Assert.False(svc.IsOfficiallySupported(unsupported));
    }

    [Fact]
    public void IsOfficiallySupported_Throws_OnNull()
    {
        var svc = CreateService();
        Assert.Throws<ArgumentNullException>(() => svc.IsOfficiallySupported(null!));
    }

    [Fact]
    public void OfficiallySupported_WithContext_ReturnsOnlyContextMatchingLanguages()
    {
        var baseOnly = new BaseGameOnlyTestLanguage();
        var expOnly = new ExpansionOnlyTestLanguage();
        var both = new EnglishAlamoLanguageDefinition();
        var svc = new LanguageService(new IAlamoLanguageDefinition[] { baseOnly, expOnly, both });

        var baseResult = svc.OfficiallySupported(GameContext.EaW);
        var expResult = svc.OfficiallySupported(GameContext.FoC);

        Assert.Contains(baseOnly, baseResult);
        Assert.Contains(both, baseResult);
        Assert.DoesNotContain(expOnly, baseResult);

        Assert.Contains(expOnly, expResult);
        Assert.Contains(both, expResult);
        Assert.DoesNotContain(baseOnly, expResult);
    }

    [Fact]
    public void IsOfficiallySupported_WithContext_ReturnsCorrectResultPerContext()
    {
        var baseOnly = new BaseGameOnlyTestLanguage();
        var svc = new LanguageService(new IAlamoLanguageDefinition[] { baseOnly, new EnglishAlamoLanguageDefinition() });

        Assert.True(svc.IsOfficiallySupported(baseOnly, GameContext.EaW));
        Assert.False(svc.IsOfficiallySupported(baseOnly, GameContext.FoC));
    }

    [Fact]
    public void IsOfficiallySupported_WithContext_Throws_OnNull()
    {
        var svc = CreateService();
        Assert.Throws<ArgumentNullException>(() => svc.IsOfficiallySupported(null!, GameContext.EaW));
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
