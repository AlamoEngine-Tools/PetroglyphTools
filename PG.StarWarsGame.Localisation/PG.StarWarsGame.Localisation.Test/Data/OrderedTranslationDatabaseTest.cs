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

    // --- SetTranslationAt ---

    [Fact]
    public void SetTranslationAt_AddsLanguageToExistingRow()
    {
        var db = Create(En, De);
        db.SetTranslation("K", En, "English");
        db.SetTranslationAt(0, De, "Deutsch");

        Assert.Single(db);
        Assert.True(db[0].TryGetTranslation(En, out var en));
        Assert.True(db[0].TryGetTranslation(De, out var de));
        Assert.Equal("English", en);
        Assert.Equal("Deutsch", de);
    }

    [Fact]
    public void SetTranslationAt_OverwritesValueForSameLanguage()
    {
        var db = Create(En);
        db.SetTranslation("K", En, "First");
        db.SetTranslationAt(0, En, "Second");

        Assert.Single(db);
        Assert.True(db[0].TryGetTranslation(En, out var val));
        Assert.Equal("Second", val);
    }

    [Fact]
    public void SetTranslationAt_TargetsOnlyTheGivenRow()
    {
        var db = Create(En, De);
        db.SetTranslation("CREDITS_LINE", En, "Line 1");
        db.SetTranslation("CREDITS_LINE", En, "Line 2");
        db.SetTranslationAt(1, De, "Zeile 2");

        Assert.Equal(2, db.Count);
        Assert.False(db[0].TryGetTranslation(De, out _));
        Assert.True(db[1].TryGetTranslation(De, out var de));
        Assert.Equal("Zeile 2", de);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1)]
    public void SetTranslationAt_Throws_WhenIndexOutOfRange(int index)
    {
        var db = Create(En);
        db.SetTranslation("K", En, "v");
        Assert.Throws<ArgumentOutOfRangeException>(() => db.SetTranslationAt(index, En, "x"));
    }

    [Fact]
    public void SetTranslationAt_Throws_WhenLanguageNull()
    {
        var db = Create(En);
        db.SetTranslation("K", En, "v");
        Assert.Throws<ArgumentNullException>(() => db.SetTranslationAt(0, null!, "x"));
    }

    // --- language registration ---

    [Fact]
    public void SetTranslation_Throws_WhenLanguageNotRegistered()
    {
        var db = Create(En);
        Assert.Throws<ArgumentException>(() => db.SetTranslation("K", De, "v"));
        Assert.Empty(db);
    }

    [Fact]
    public void SetTranslationAt_Throws_WhenLanguageNotRegistered()
    {
        var db = Create(En);
        db.SetTranslation("K", En, "v");
        Assert.Throws<ArgumentException>(() => db.SetTranslationAt(0, De, "x"));
        Assert.False(db[0].TryGetTranslation(De, out _));
    }

    [Fact]
    public void InsertAt_Throws_WhenLanguageNotRegistered()
    {
        var db = Create(En);
        db.SetTranslation("K", En, "v");
        Assert.Throws<ArgumentException>(() => db.InsertAt(0, "X", De, "x"));
        Assert.Single(db);
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
