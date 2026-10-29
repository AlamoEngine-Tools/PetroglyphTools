// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Localisation.Data;
using PG.StarWarsGame.Localisation.Languages;
using Xunit;

namespace PG.StarWarsGame.Localisation.Test.Data;

public class TranslationDatabaseBuilderTest
{
    private static readonly IAlamoLanguageDefinition En = new EnglishAlamoLanguageDefinition();
    private static readonly IAlamoLanguageDefinition De = new GermanAlamoLanguageDefinition();

    private static ITranslationDatabaseBuilder Builder() =>
        new TranslationDatabaseFactory().CreateDatabase();

    [Fact]
    public void CreateDatabase_BuildsKeyedDatabase()
    {
        var db = Builder().WithLanguage(En).BuildKeyed();
        Assert.NotNull(db);
    }

    [Fact]
    public void CreateDatabase_BuildsOrderedDatabase()
    {
        var db = Builder().WithLanguage(En).BuildOrdered();
        Assert.NotNull(db);
    }

    [Fact]
    public void WithLanguage_AddsSingleLanguage()
    {
        var db = Builder().WithLanguage(En).BuildKeyed();
        Assert.Single(db.Languages);
        Assert.Contains(En, db.Languages);
    }

    [Fact]
    public void WithLanguages_AddsMultipleLanguages()
    {
        var db = Builder().WithLanguages(new[] { En, De }).BuildKeyed();
        Assert.Equal(2, db.Languages.Count);
    }

    [Fact]
    public void WithActiveLanguage_SetsActiveLanguageOnBuiltDatabase()
    {
        var db = Builder().WithLanguage(En).SetActiveLanguage(En).BuildKeyed();
        Assert.Equal(En, db.ActiveLanguage);
    }

    [Fact]
    public void WithActiveLanguage_NotInLanguages_ThrowsAtBuildTime()
    {
        Assert.Throws<ArgumentException>(() =>
            Builder().WithLanguage(En).SetActiveLanguage(De).BuildKeyed());
    }

    [Fact]
    public void WithLanguages_Empty_BuildsEmptyLanguagesDatabase()
    {
        var db = Builder().BuildKeyed();
        Assert.Empty(db.Languages);
    }

    [Fact]
    public void Builder_IsChainable()
    {
        var db = Builder()
            .WithLanguage(En)
            .WithLanguage(De)
            .SetActiveLanguage(En)
            .BuildOrdered();

        Assert.Equal(2, db.Languages.Count);
        Assert.Equal(En, db.ActiveLanguage);
    }

    [Fact]
    public void WithLanguage_Throws_OnNull()
    {
        Assert.Throws<ArgumentNullException>(() => Builder().WithLanguage(null!));
    }

    [Fact]
    public void WithLanguages_Throws_OnNull()
    {
        Assert.Throws<ArgumentNullException>(() => Builder().WithLanguages(null!));
    }
}
