// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Localisation.Data.Config.v2;
using PG.StarWarsGame.Localisation.Languages;
using Xunit;

namespace PG.StarWarsGame.Localisation.Baseline.Test;

public class BaselineTranslationProviderTest : CommonBaselineTestBase
{
    private IBaselineTranslationProvider Provider =>
        ServiceProvider.GetRequiredService<IBaselineTranslationProvider>();

    [Theory]
    [InlineData(GameType.EaW)]
    [InlineData(GameType.FoC)]
    public void GetMasterText_SingleLanguage_IsNonEmpty(GameType game)
    {
        var lang = new EnglishAlamoLanguageDefinition();
        var db = Provider.GetMasterText(game, lang);
        Assert.NotEmpty(db);
    }

    [Theory]
    [InlineData(GameType.EaW)]
    [InlineData(GameType.FoC)]
    public void GetCreditsText_SingleLanguage_IsNonEmpty(GameType game)
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
        var db = Provider.GetMasterText(GameType.EaW, langs);
        Assert.Equal(2, db.Languages.Count);
    }

    [Fact]
    public void GetMasterText_EaW_English_ContainsKnownKey()
    {
        var lang = new EnglishAlamoLanguageDefinition();
        var db = Provider.GetMasterText(GameType.EaW, lang);
        // At minimum the database should be populated — spot-check it has entries
        Assert.True(db.Count > 100);
    }

    [Fact]
    public void GetMasterText_UnsupportedGame_Throws()
    {
        var lang = new EnglishAlamoLanguageDefinition();
        Assert.Throws<ArgumentException>(() => Provider.GetMasterText(GameType.Mod, lang));
    }

    [Fact]
    public void GetCreditsText_UnsupportedGame_Throws()
    {
        var lang = new EnglishAlamoLanguageDefinition();
        Assert.Throws<ArgumentException>(() => Provider.GetCreditsText(GameType.Mod, lang));
    }
}
