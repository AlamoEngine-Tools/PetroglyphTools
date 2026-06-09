// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using PG.StarWarsGame.Localisation.Languages;
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
        Assert.All(svc.OfficiallySupported, l => Assert.True(l.IsOfficiallySupported));
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

    private sealed class UnsupportedTestLanguage : AlamoLanguageDefinitionBase
    {
        protected override string ConfiguredLanguageIdentifier => "KLINGON";
        protected override System.Globalization.CultureInfo ConfiguredCulture =>
            System.Globalization.CultureInfo.InvariantCulture;
    }
}
