// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Localisation.Languages;
using PG.Testing;
using Xunit;

namespace PG.StarWarsGame.Localisation.Baseline.Test;

public class BaselineTranslationProviderTest : PGTestBase
{
    protected override void SetupServices(IServiceCollection serviceCollection)
    {
        base.SetupServices(serviceCollection);
        serviceCollection.SupportLocalisationBaseline();
    }

    private IBaselineTranslationProvider Provider =>
        ServiceProvider.GetRequiredService<IBaselineTranslationProvider>();

    [Theory]
    [InlineData(GameContext.EaW)]
    [InlineData(GameContext.FoC)]
    public void GetMasterText_SingleLanguage_IsNonEmpty(GameContext game)
    {
        var lang = new EnglishAlamoLanguageDefinition();
        var db = Provider.GetMasterText(game, lang);
        Assert.NotEmpty(db);
    }

    [Theory]
    [InlineData(GameContext.EaW)]
    [InlineData(GameContext.FoC)]
    public void GetCreditsText_SingleLanguage_IsNonEmpty(GameContext game)
    {
        var lang = new EnglishAlamoLanguageDefinition();
        var db = Provider.GetCreditsText(game, lang);
        Assert.NotEmpty(db);
    }

    [Fact]
    public void GetMasterText_MultiLanguage_ContainsAllRequestedLanguages()
    {
        var langs = new IAlamoLanguageDefinition[]
        {
            new EnglishAlamoLanguageDefinition(),
            new GermanAlamoLanguageDefinition(),
        };
        var db = Provider.GetMasterText(GameContext.EaW, langs);
        Assert.Equal(2, db.Languages.Count);
    }

    [Fact]
    public void GetMasterText_EaW_English_ContainsKnownKey()
    {
        var lang = new EnglishAlamoLanguageDefinition();
        var db = Provider.GetMasterText(GameContext.EaW, lang);
        Assert.True(db.Count > 100);
    }

    [Theory]
    [InlineData(GameContext.EaW)]
    [InlineData(GameContext.FoC)]
    public void GetCreditsText_MultiLanguage_MergesLanguagesIntoSharedRows(GameContext game)
    {
        var en = new EnglishAlamoLanguageDefinition();
        var de = new GermanAlamoLanguageDefinition();

        var single = Provider.GetCreditsText(game, en);
        var multi = Provider.GetCreditsText(game, new IAlamoLanguageDefinition[] { en, de });

        // Adding a language must not multiply the row count.
        Assert.Equal(single.Count, multi.Count);
        Assert.Contains(multi, e => e.TryGetTranslation(en, out _) && e.TryGetTranslation(de, out _));
    }

    [Theory]
    [InlineData(GameContext.EaW)]
    [InlineData(GameContext.FoC)]
    public void GetCreditsText_MultiLanguage_RowsAlignWithSingleLanguageLoad(GameContext game)
    {
        var en = new EnglishAlamoLanguageDefinition();
        var de = new GermanAlamoLanguageDefinition();

        var single = Provider.GetCreditsText(game, en);
        var multi = Provider.GetCreditsText(game, new IAlamoLanguageDefinition[] { en, de });

        Assert.Equal(single.Count, multi.Count);

        // Every row must sit at the same index and carry the same English text as the single-language load;
        // a merge that slipped by one row would still have the right count but the wrong pairings.
        for (var i = 0; i < single.Count; i++)
        {
            Assert.Equal(single[i].Key, multi[i].Key);
            single[i].TryGetTranslation(en, out var expected);
            multi[i].TryGetTranslation(en, out var actual);
            Assert.Equal(expected, actual);
        }

        Assert.All(multi, e => Assert.True(e.TryGetTranslation(de, out _)));
    }

    [Fact]
    public void GetMasterText_MultiLanguage_MergesLanguagesOntoSharedKeys()
    {
        var en = new EnglishAlamoLanguageDefinition();
        var de = new GermanAlamoLanguageDefinition();

        var single = Provider.GetMasterText(GameContext.EaW, en);
        var multi = Provider.GetMasterText(GameContext.EaW, new IAlamoLanguageDefinition[] { en, de });

        // Keyed storage merges by key, so adding a language must widen entries, never stack copies.
        Assert.True(multi.Count < single.Count * 2);
        Assert.Contains(multi, e => e.TryGetTranslation(en, out _) && e.TryGetTranslation(de, out _));
    }

    [Fact]
    public void GetCreditsText_LanguageMappedWithoutResource_LeavesLoadedRowsIntact()
    {
        var en = new EnglishAlamoLanguageDefinition();
        var ru = new RussianAlamoLanguageDefinition();

        var single = Provider.GetCreditsText(GameContext.EaW, en);
        var multi = Provider.GetCreditsText(GameContext.EaW, new IAlamoLanguageDefinition[] { en, ru });

        // Russian is mapped in LanguageMap but ships no .dat resource; it must be a silent no-op
        // rather than appending empty rows onto the English ones.
        Assert.Equal(single.Count, multi.Count);
        Assert.DoesNotContain(multi, e => e.TryGetTranslation(ru, out _));
    }

    [Fact]
    public void GetMasterText_NullLanguage_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Provider.GetMasterText(GameContext.EaW, (IAlamoLanguageDefinition)null!));
    }
}
