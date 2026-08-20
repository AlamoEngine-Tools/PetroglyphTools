// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.IO;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Localisation.Data;
using PG.StarWarsGame.Localisation.IO.Csv;
using PG.StarWarsGame.Localisation.Languages;
using Xunit;

namespace PG.StarWarsGame.Localisation.Test.IO;

public class CsvTranslationAdapterTest : CommonLocalisationTestBase
{
    private static readonly IAlamoLanguageDefinition En = new EnglishAlamoLanguageDefinition();
    private static readonly IAlamoLanguageDefinition De = new GermanAlamoLanguageDefinition();

    private ICsvTranslationImporter CreateImporter() =>
        ServiceProvider.GetRequiredService<ICsvTranslationImporter>();

    private ICsvTranslationExporter CreateExporter() =>
        ServiceProvider.GetRequiredService<ICsvTranslationExporter>();

    [Fact]
    public void RoundTrip_MultiLanguage_PreservesAllTranslations()
    {
        var db = new TranslationDatabaseFactory().CreateKeyed(new[] { En, De });
        db.SetTranslation("HELLO", En, "Hello");
        db.SetTranslation("HELLO", De, "Hallo");

        var csv = CreateExporter().Export(db);
        var db2 = new TranslationDatabaseFactory().CreateKeyed(new[] { En, De });
        CreateImporter().Import(new StringReader(csv), db2);

        Assert.True(db2.TryGetEntry("HELLO", out var entry));
        Assert.True(entry!.TryGetTranslation(En, out var en));
        Assert.True(entry.TryGetTranslation(De, out var de));
        Assert.Equal("Hello", en);
        Assert.Equal("Hallo", de);
    }

    [Fact]
    public void Export_ContainsHeaderRow()
    {
        var db = new TranslationDatabaseFactory().CreateKeyed(new[] { En, De });
        db.SetTranslation("K", En, "v");
        var csv = CreateExporter().Export(db);
        var firstLine = csv.Split('\n')[0];
        Assert.Contains("ENGLISH", firstLine);
        Assert.Contains("GERMAN", firstLine);
    }

    [Fact]
    public void RoundTrip_ValueWithComma_IsPreserved()
    {
        var db = new TranslationDatabaseFactory().CreateKeyed(new[] { En });
        db.SetTranslation("K", En, "Hello, World");
        var csv = CreateExporter().Export(db);
        var db2 = new TranslationDatabaseFactory().CreateKeyed(new[] { En });
        CreateImporter().Import(new StringReader(csv), db2);
        Assert.True(db2.TryGetEntry("K", out var entry));
        Assert.True(entry!.TryGetTranslation(En, out var val));
        Assert.Equal("Hello, World", val);
    }

    [Fact]
    public void Import_EmptyCsv_LeavesDbEmpty()
    {
        var db = new TranslationDatabaseFactory().CreateKeyed(new[] { En });
        CreateImporter().Import(new StringReader("key,ENGLISH\n"), db);
        Assert.Empty(db);
    }

    [Fact]
    public void RoundTrip_ValueWithEmbeddedQuotes_IsPreserved()
    {
        var db = new TranslationDatabaseFactory().CreateKeyed(new[] { En });
        db.SetTranslation("K", En, "The \"Resolute\" is a Venator");
        var csv = CreateExporter().Export(db);
        var db2 = new TranslationDatabaseFactory().CreateKeyed(new[] { En });
        CreateImporter().Import(new StringReader(csv), db2);
        Assert.True(db2.TryGetEntry("K", out var entry));
        Assert.True(entry!.TryGetTranslation(En, out var val));
        Assert.Equal("The \"Resolute\" is a Venator", val);
    }

    [Fact]
    public void Import_CrlfLineEndings_AreHandledCorrectly()
    {
        var csv = "key,ENGLISH\r\nHELLO,Hello\r\nWORLD,World\r\n";
        var db = new TranslationDatabaseFactory().CreateKeyed(new[] { En });
        CreateImporter().Import(new StringReader(csv), db);
        Assert.True(db.TryGetEntry("HELLO", out var e1));
        Assert.True(e1!.TryGetTranslation(En, out var v1));
        Assert.Equal("Hello", v1);
        Assert.True(db.TryGetEntry("WORLD", out var e2));
        Assert.True(e2!.TryGetTranslation(En, out var v2));
        Assert.Equal("World", v2);
    }

    [Fact]
    public void Import_ColumnForUnregisteredLanguage_IsSkippedNotThrown()
    {
        // The database rejects unregistered languages, so the importer must filter them out itself
        // rather than letting a stray column abort the whole import.
        var csv = "key,ENGLISH,GERMAN\nHELLO,Hello,Hallo\n";
        var db = new TranslationDatabaseFactory().CreateKeyed(new[] { En });
        CreateImporter().Import(new StringReader(csv), db);

        Assert.Single(db);
        Assert.True(db.TryGetEntry("HELLO", out var entry));
        Assert.True(entry!.TryGetTranslation(En, out var en));
        Assert.Equal("Hello", en);
        Assert.False(entry.TryGetTranslation(De, out _));
    }

    [Fact]
    public void Import_IntoOrderedDatabase_MultiLanguage_CreatesOneEntryPerRow()
    {
        var csv = "key,ENGLISH,GERMAN\nCREDIT_A,Alpha,Alpha-DE\nCREDIT_B,Beta,Beta-DE\n";
        var db = new TranslationDatabaseFactory().CreateOrdered(new[] { En, De });
        CreateImporter().Import(new StringReader(csv), db);

        Assert.Equal(2, db.Count);
        Assert.Equal(new[] { "CREDIT_A", "CREDIT_B" }, db.Select(e => e.Key));

        Assert.True(db[0].TryGetTranslation(En, out var en0));
        Assert.True(db[0].TryGetTranslation(De, out var de0));
        Assert.Equal("Alpha", en0);
        Assert.Equal("Alpha-DE", de0);
    }

    [Fact]
    public void RoundTrip_OrderedDatabase_MultiLanguage_PreservesRowsAndOrder()
    {
        var db = new TranslationDatabaseFactory().CreateOrdered(new[] { En, De });
        db.SetTranslation("CREDIT", En, "Line 1");
        db.SetTranslationAt(0, De, "Zeile 1");
        db.SetTranslation("CREDIT", En, "Line 2");
        db.SetTranslationAt(1, De, "Zeile 2");

        var csv = CreateExporter().Export(db);
        var db2 = new TranslationDatabaseFactory().CreateOrdered(new[] { En, De });
        CreateImporter().Import(new StringReader(csv), db2);

        Assert.Equal(2, db2.Count);
        Assert.Equal(new[] { "CREDIT", "CREDIT" }, db2.Select(e => e.Key));

        Assert.True(db2[0].TryGetTranslation(De, out var de0));
        Assert.True(db2[1].TryGetTranslation(De, out var de1));
        Assert.Equal("Zeile 1", de0);
        Assert.Equal("Zeile 2", de1);
    }

    [Fact]
    public void Export_KeyedDatabase_WritesKeysAlphabetically()
    {
        var db = new TranslationDatabaseFactory().CreateKeyed(new[] { En });
        db.SetTranslation("ZEBRA", En, "z");
        db.SetTranslation("APPLE", En, "a");
        db.SetTranslation("MONKEY", En, "m");

        var csv = CreateExporter().Export(db);
        var lines = csv.Split('\n');
        // lines[0] is header; data lines follow
        Assert.Equal("APPLE", lines[1].Split(',')[0]);
        Assert.Equal("MONKEY", lines[2].Split(',')[0]);
        Assert.Equal("ZEBRA", lines[3].Split(',')[0]);
    }
}
