// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Linq;
using PG.StarWarsGame.Localisation.Data;
using PG.StarWarsGame.Localisation.Languages;
using Xunit;

namespace PG.StarWarsGame.Localisation.Test.Data;

public class KeyedTranslationDatabaseTest
{
    private static readonly IAlamoLanguageDefinition En = new EnglishAlamoLanguageDefinition();
    private static readonly IAlamoLanguageDefinition De = new GermanAlamoLanguageDefinition();

    private static IKeyedTranslationDatabase Create(params IAlamoLanguageDefinition[] langs) =>
        new TranslationDatabaseFactory().CreateKeyed(langs);

    // --- empty state ---

    [Fact]
    public void Empty_HasZeroEntries()
    {
        var db = Create(En);
        Assert.Empty(db);
    }

    [Fact]
    public void Empty_ContainsKey_ReturnsFalse()
    {
        var db = Create(En);
        Assert.False(db.ContainsKey("KEY"));
    }

    [Fact]
    public void Empty_TryGetEntry_ReturnsFalse()
    {
        var db = Create(En);
        Assert.False(db.TryGetEntry("KEY", out _));
    }

    // --- single-language CRUD ---

    [Fact]
    public void SetTranslation_AddsNewEntry()
    {
        var db = Create(En);
        db.SetTranslation("HELLO", En, "Hello");
        Assert.True(db.ContainsKey("HELLO"));
        Assert.Single(db);
    }

    [Fact]
    public void SetTranslation_UpdatesExistingValue()
    {
        var db = Create(En);
        db.SetTranslation("HELLO", En, "Hello");
        db.SetTranslation("HELLO", En, "Hi");
        Assert.Single(db);
        Assert.True(db.TryGetEntry("HELLO", out var entry));
        Assert.True(entry!.TryGetTranslation(En, out var val));
        Assert.Equal("Hi", val);
    }

    [Fact]
    public void RemoveEntry_RemovesKey()
    {
        var db = Create(En);
        db.SetTranslation("HELLO", En, "Hello");
        Assert.True(db.RemoveEntry("HELLO"));
        Assert.False(db.ContainsKey("HELLO"));
        Assert.Empty(db);
    }

    [Fact]
    public void RemoveEntry_ReturnsFalse_WhenKeyMissing()
    {
        var db = Create(En);
        Assert.False(db.RemoveEntry("MISSING"));
    }

    [Fact]
    public void Clear_RemovesAllEntries()
    {
        var db = Create(En);
        db.SetTranslation("A", En, "a");
        db.SetTranslation("B", En, "b");
        db.Clear();
        Assert.Empty(db);
    }

    // --- multi-language ---

    [Fact]
    public void SetTranslation_MultipleLanguages_StoredPerLanguage()
    {
        var db = Create(En, De);
        db.SetTranslation("HELLO", En, "Hello");
        db.SetTranslation("HELLO", De, "Hallo");

        Assert.True(db.TryGetEntry("HELLO", out var entry));
        Assert.True(entry!.TryGetTranslation(En, out var en));
        Assert.True(entry.TryGetTranslation(De, out var de));
        Assert.Equal("Hello", en);
        Assert.Equal("Hallo", de);
    }

    [Fact]
    public void TryGetTranslation_ReturnsFalse_ForMissingLanguage()
    {
        var db = Create(En);
        db.SetTranslation("HELLO", En, "Hello");
        Assert.True(db.TryGetEntry("HELLO", out var entry));
        Assert.False(entry!.TryGetTranslation(De, out _));
    }

    // --- uniqueness ---

    [Fact]
    public void SetTranslation_SameKey_DoesNotCreateDuplicate()
    {
        var db = Create(En, De);
        db.SetTranslation("KEY", En, "English");
        db.SetTranslation("KEY", De, "Deutsch");
        Assert.Single(db);
    }

    // --- Languages list ---

    [Fact]
    public void Languages_ReflectsConstructorInput()
    {
        var db = Create(En, De);
        Assert.Equal(2, db.Languages.Count);
        Assert.Contains(En, db.Languages);
        Assert.Contains(De, db.Languages);
    }

    // --- enumeration ---

    [Fact]
    public void Enumeration_ReturnsAllEntries()
    {
        var db = Create(En);
        db.SetTranslation("A", En, "a");
        db.SetTranslation("B", En, "b");
        Assert.Equal(2, db.ToList().Count);
    }

    // --- null guards ---

    [Fact]
    public void SetTranslation_Throws_WhenKeyIsNull()
    {
        var db = Create(En);
        Assert.Throws<ArgumentNullException>(() => db.SetTranslation(null!, En, "v"));
    }

    [Fact]
    public void SetTranslation_Throws_WhenLanguageIsNull()
    {
        var db = Create(En);
        Assert.Throws<ArgumentNullException>(() => db.SetTranslation("K", null!, "v"));
    }

    [Fact]
    public void SetTranslation_Throws_WhenLanguageNotRegistered()
    {
        var db = Create(En);
        Assert.Throws<ArgumentException>(() => db.SetTranslation("K", De, "v"));
    }

    [Fact]
    public void SetTranslation_UnregisteredLanguage_LeavesDbUnchanged()
    {
        var db = Create(En);
        Assert.Throws<ArgumentException>(() => db.SetTranslation("K", De, "v"));
        Assert.Empty(db);
    }

    // --- ActiveLanguage ---

    [Fact]
    public void ActiveLanguage_DefaultsToNull()
    {
        var db = Create(En);
        Assert.Null(db.ActiveLanguage);
    }

    [Fact]
    public void ActiveLanguage_CanBeSet_WhenRegistered()
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

    [Fact]
    public void ActiveLanguage_CanBeSetToNull()
    {
        var db = Create(En);
        db.ActiveLanguage = En;
        db.ActiveLanguage = null;
        Assert.Null(db.ActiveLanguage);
    }

    // --- TryGetTranslation (active language) ---

    [Fact]
    public void TryGetTranslation_ByActiveLanguage_ReturnsValue()
    {
        var db = Create(En);
        db.SetTranslation("HELLO", En, "Hello");
        db.ActiveLanguage = En;
        Assert.True(db.TryGetTranslation("HELLO", out var val));
        Assert.Equal("Hello", val);
    }

    [Fact]
    public void TryGetTranslation_ByActiveLanguage_ReturnsFalse_WhenKeyMissing()
    {
        var db = Create(En);
        db.ActiveLanguage = En;
        Assert.False(db.TryGetTranslation("MISSING", out _));
    }

    [Fact]
    public void TryGetTranslation_Throws_WhenActiveLanguageIsNull()
    {
        var db = Create(En);
        db.SetTranslation("HELLO", En, "Hello");
        Assert.Throws<InvalidOperationException>(() => db.TryGetTranslation("HELLO", out _));
    }
}
