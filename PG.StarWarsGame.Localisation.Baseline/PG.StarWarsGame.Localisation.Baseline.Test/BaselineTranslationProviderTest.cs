// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
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

    [Fact]
    public void GetMasterText_NullLanguage_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Provider.GetMasterText(GameContext.EaW, (IAlamoLanguageDefinition)null!));
    }
}
