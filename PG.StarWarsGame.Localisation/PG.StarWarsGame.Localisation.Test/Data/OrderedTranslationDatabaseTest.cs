// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Linq;
using PG.StarWarsGame.Localisation.Data;
using PG.StarWarsGame.Localisation.Languages;
using Xunit;

namespace PG.StarWarsGame.Localisation.Test.Data;

public class OrderedTranslationDatabaseTest
{
    private static readonly IAlamoLanguageDefinition En = new EnglishAlamoLanguageDefinition();
    private static readonly IAlamoLanguageDefinition De = new GermanAlamoLanguageDefinition();

    private static IOrderedTranslationDatabase Create(params IAlamoLanguageDefinition[] langs) =>
        new TranslationDatabaseFactory().CreateOrdered(langs);

    [Fact]
    public void Empty_HasZeroEntries()
    {
        var db = Create(En);
        Assert.Empty(db);
    }

    [Fact]
    public void SetTranslation_PreservesInsertionOrder()
    {
        var db = Create(En);
        db.SetTranslation("A", En, "First");
        db.SetTranslation("B", En, "Second");
        db.SetTranslation("C", En, "Third");
        var keys = db.Select(e => e.Key).ToList();
        Assert.Equal(new[] { "A", "B", "C" }, keys);
    }

    [Fact]
    public void SetTranslation_AllowsDuplicateKeys()
    {
        var db = Create(En);
        db.SetTranslation("CREDITS_LINE", En, "Line 1");
        db.SetTranslation("CREDITS_LINE", En, "Line 2");
        Assert.Equal(2, db.Count);
    }

    [Fact]
    public void GetAllEntriesForKey_ReturnsAllDuplicates()
    {
        var db = Create(En);
        db.SetTranslation("K", En, "First");
        db.SetTranslation("K", En, "Second");
        var entries = db.GetAllEntriesForKey("K");
        Assert.Equal(2, entries.Count);
    }

    [Fact]
    public void GetAllEntriesForKey_ReturnsEmpty_WhenKeyMissing()
    {
        var db = Create(En);
        Assert.Empty(db.GetAllEntriesForKey("MISSING"));
    }

    [Fact]
    public void InsertAt_InsertsAtCorrectPosition()
    {
        var db = Create(En);
        db.SetTranslation("A", En, "First");
        db.SetTranslation("C", En, "Third");
        db.InsertAt(1, "B", En, "Second");
        var keys = db.Select(e => e.Key).ToList();
        Assert.Equal(new[] { "A", "B", "C" }, keys);
    }

    [Fact]
    public void RemoveEntry_RemovesFirstOccurrence()
    {
        var db = Create(En);
        db.SetTranslation("K", En, "First");
        db.SetTranslation("K", En, "Second");
        db.RemoveEntry("K");
        Assert.Single(db);
    }

    [Fact]
    public void Clear_RemovesAll()
    {
        var db = Create(En);
        db.SetTranslation("A", En, "a");
        db.SetTranslation("A", En, "a2");
        db.Clear();
        Assert.Empty(db);
    }

    [Fact]
    public void MultiLanguage_StoredPerLanguagePerEntry()
    {
        var db = Create(En, De);
        db.SetTranslation("K", En, "English");
        db.SetTranslation("K", De, "Deutsch");
        Assert.Equal(2, db.Count);
        var allK = db.GetAllEntriesForKey("K");
        Assert.Equal(2, allK.Count);
    }

    // --- ActiveLanguage ---

    [Fact]
    public void ActiveLanguage_DefaultsToNull()
    {
        var db = Create(En);
        Assert.Null(db.ActiveLanguage);
    }

    [Fact]
    public void ActiveLanguage_CanBeSet()
    {
        var db = Create(En);
        db.ActiveLanguage = En;
        Assert.Equal(En, db.ActiveLanguage);
    }

    [Fact]
    public void ActiveLanguage_Throws_WhenNotInLanguages()
    {
        var db = Create(En);
        Assert.Throws<ArgumentException>(() => db.ActiveLanguage = De);
    }
}
