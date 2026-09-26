// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Localisation.Data;
using PG.StarWarsGame.Localisation.IO.Csv;
using PG.StarWarsGame.Localisation.IO.Dat;
using PG.StarWarsGame.Localisation.IO.Properties;
using PG.StarWarsGame.Localisation.IO.Xml;
using PG.StarWarsGame.Localisation.Services;
using Xunit;

namespace PG.StarWarsGame.Localisation.Test;

public class LocalisationServiceContributionTest
{
    private static ServiceProvider BuildProvider()
    {
        var sc = new ServiceCollection();
        sc.SupportLocalisation();
        return sc.BuildServiceProvider();
    }

    [Fact]
    public void SupportLocalisation_Resolves_ILanguageService()
    {
        using var sp = BuildProvider();
        Assert.NotNull(sp.GetRequiredService<ILanguageService>());
    }

    [Fact]
    public void SupportLocalisation_Resolves_ITranslationDatabaseFactory()
    {
        using var sp = BuildProvider();
        Assert.NotNull(sp.GetRequiredService<ITranslationDatabaseFactory>());
    }

    [Fact]
    public void SupportLocalisation_Resolves_IDatTranslationImporter()
    {
        using var sp = BuildProvider();
        Assert.NotNull(sp.GetRequiredService<IDatTranslationImporter>());
    }

    [Fact]
    public void SupportLocalisation_Resolves_IDatTranslationExporter()
    {
        using var sp = BuildProvider();
        Assert.NotNull(sp.GetRequiredService<IDatTranslationExporter>());
    }

    [Fact]
    public void SupportLocalisation_Resolves_IXmlTranslationImporter()
    {
        using var sp = BuildProvider();
        Assert.NotNull(sp.GetRequiredService<IXmlTranslationImporter>());
    }

    [Fact]
    public void SupportLocalisation_Resolves_IXmlTranslationExporter()
    {
        using var sp = BuildProvider();
        Assert.NotNull(sp.GetRequiredService<IXmlTranslationExporter>());
    }

    [Fact]
    public void SupportLocalisation_Resolves_ICsvTranslationImporter()
    {
        using var sp = BuildProvider();
        Assert.NotNull(sp.GetRequiredService<ICsvTranslationImporter>());
    }

    [Fact]
    public void SupportLocalisation_Resolves_ICsvTranslationExporter()
    {
        using var sp = BuildProvider();
        Assert.NotNull(sp.GetRequiredService<ICsvTranslationExporter>());
    }

    [Fact]
    public void SupportLocalisation_Resolves_IPropertiesTranslationImporter()
    {
        using var sp = BuildProvider();
        Assert.NotNull(sp.GetRequiredService<IPropertiesTranslationImporter>());
    }

    [Fact]
    public void SupportLocalisation_Resolves_IPropertiesTranslationExporter()
    {
        using var sp = BuildProvider();
        Assert.NotNull(sp.GetRequiredService<IPropertiesTranslationExporter>());
    }

    [Fact]
    public void SupportLocalisation_LanguageService_HasElevenLanguages()
    {
        using var sp = BuildProvider();
        var svc = sp.GetRequiredService<ILanguageService>();
        Assert.Equal(11, svc.AllLanguages.Count);
    }
}
