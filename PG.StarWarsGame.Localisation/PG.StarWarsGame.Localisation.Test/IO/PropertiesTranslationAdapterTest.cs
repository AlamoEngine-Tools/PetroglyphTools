// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Localisation.Data;
using PG.StarWarsGame.Localisation.IO;
using PG.StarWarsGame.Localisation.IO.Properties;
using PG.StarWarsGame.Localisation.Languages;
using Xunit;

namespace PG.StarWarsGame.Localisation.Test.IO;

public class PropertiesTranslationAdapterTest : CommonLocalisationTestBase
{
    private static readonly IAlamoLanguageDefinition En = new EnglishAlamoLanguageDefinition();
    private static readonly IAlamoLanguageDefinition De = new GermanAlamoLanguageDefinition();

    private static IReadOnlyList<KeyValuePair<IAlamoLanguageDefinition, TextReader>> Sources(
        params (IAlamoLanguageDefinition Language, string Content)[] entries)
    {
        var list = new List<KeyValuePair<IAlamoLanguageDefinition, TextReader>>();
        foreach (var (language, content) in entries)
            list.Add(new KeyValuePair<IAlamoLanguageDefinition, TextReader>(language, new StringReader(content)));
        return list;
    }

    private IPropertiesTranslationImporter CreateImporter() =>
        ServiceProvider.GetRequiredService<IPropertiesTranslationImporter>();

    private IPropertiesTranslationExporter CreateExporter() =>
        ServiceProvider.GetRequiredService<IPropertiesTranslationExporter>();

    [Fact]
    public void RoundTrip_PreservesKeyValuePairs()
    {
        var db = new TranslationDatabaseFactory().CreateKeyed(new[] { En });
        db.SetTranslation("HELLO", En, "Hello World");
        db.SetTranslation("BYE", En, "Goodbye");

        var props = CreateExporter().Export(db, En);
        var db2 = new TranslationDatabaseFactory().CreateKeyed(new[] { En });
        CreateImporter().Import(new StringReader(props), En, db2);

        Assert.True(db2.TryGetEntry("HELLO", out var hello));
        Assert.True(hello!.TryGetTranslation(En, out var val));
        Assert.Equal("Hello World", val);
    }

    [Fact]
    public void Export_ProducesKeyEqualsValueFormat()
    {
        var db = new TranslationDatabaseFactory().CreateKeyed(new[] { En });
        db.SetTranslation("KEY", En, "value");
        var props = CreateExporter().Export(db, En);
        Assert.Contains("KEY=value", props);
    }

    [Fact]
    public void RoundTrip_ValueWithEqualsSign_IsPreserved()
    {
        var db = new TranslationDatabaseFactory().CreateKeyed(new[] { En });
        db.SetTranslation("EQ", En, "a=b");
        var props = CreateExporter().Export(db, En);
        var db2 = new TranslationDatabaseFactory().CreateKeyed(new[] { En });
        CreateImporter().Import(new StringReader(props), En, db2);
        Assert.True(db2.TryGetEntry("EQ", out var entry));
        Assert.True(entry!.TryGetTranslation(En, out var val));
        Assert.Equal("a=b", val);
    }

    [Fact]
    public void Import_Empty_LeavesDbEmpty()
    {
        var db = new TranslationDatabaseFactory().CreateKeyed(new[] { En });
        CreateImporter().Import(new StringReader(""), En, db);
        Assert.Empty(db);
    }

    [Fact]
    public void Import_SkipsCommentLines()
    {
        const string props = "# This is a comment\nKEY=Value\n";
        var db = new TranslationDatabaseFactory().CreateKeyed(new[] { En });
        CreateImporter().Import(new StringReader(props), En, db);
        Assert.Single(db);
    }

    // --- ImportAll (multi-language via a single-language importer) ---

    [Fact]
    public void ImportAll_IntoOrderedDatabase_WidensRowsInsteadOfStacking()
    {
        var sources = Sources(
            (En, "A=Alpha\nB=Beta\n"),
            (De, "A=Alpha-DE\nB=Beta-DE\n"));

        var db = new TranslationDatabaseFactory().CreateOrdered(new[] { En, De });
        CreateImporter().ImportAll(sources, db);

        Assert.Equal(2, db.Count);

        Assert.True(db[0].TryGetTranslation(En, out var en0));
        Assert.True(db[0].TryGetTranslation(De, out var de0));
        Assert.Equal("Alpha", en0);
        Assert.Equal("Alpha-DE", de0);

        Assert.True(db[1].TryGetTranslation(En, out var en1));
        Assert.True(db[1].TryGetTranslation(De, out var de1));
        Assert.Equal("Beta", en1);
        Assert.Equal("Beta-DE", de1);
    }

    [Fact]
    public void ImportAll_IntoOrderedDatabase_PreservesDuplicateKeyRows()
    {
        var sources = Sources(
            (En, "CREDIT=Line 1\nCREDIT=Line 2\n"),
            (De, "CREDIT=Zeile 1\nCREDIT=Zeile 2\n"));

        var db = new TranslationDatabaseFactory().CreateOrdered(new[] { En, De });
        CreateImporter().ImportAll(sources, db);

        Assert.Equal(2, db.Count);
        Assert.True(db[0].TryGetTranslation(De, out var de0));
        Assert.True(db[1].TryGetTranslation(De, out var de1));
        Assert.Equal("Zeile 1", de0);
        Assert.Equal("Zeile 2", de1);
    }

    [Fact]
    public void ImportAll_IntoKeyedDatabase_MergesByKey()
    {
        var sources = Sources(
            (En, "HELLO=Hello\n"),
            (De, "HELLO=Hallo\n"));

        var db = new TranslationDatabaseFactory().CreateKeyed(new[] { En, De });
        CreateImporter().ImportAll(sources, db);

        Assert.Single(db);
        Assert.True(db.TryGetEntry("HELLO", out var entry));
        Assert.True(entry!.TryGetTranslation(En, out var en));
        Assert.True(entry.TryGetTranslation(De, out var de));
        Assert.Equal("Hello", en);
        Assert.Equal("Hallo", de);
    }

    [Fact]
    public void ImportAll_LaterSourceWithExtraRows_AppendsRatherThanMistranslating()
    {
        var sources = Sources(
            (En, "A=Alpha\n"),
            (De, "A=Alpha-DE\nB=Beta-DE\n"));

        var db = new TranslationDatabaseFactory().CreateOrdered(new[] { En, De });
        CreateImporter().ImportAll(sources, db);

        // Row 0 lines up and widens; the surplus German row must not be forced onto an English row.
        Assert.Equal(2, db.Count);
        Assert.True(db[0].TryGetTranslation(En, out var en0));
        Assert.Equal("Alpha", en0);
        Assert.False(db[1].TryGetTranslation(En, out _));
        Assert.True(db[1].TryGetTranslation(De, out var de1));
        Assert.Equal("Beta-DE", de1);
    }

    [Fact]
    public void Export_KeyedDatabase_WritesKeysAlphabetically()
    {
        var db = new TranslationDatabaseFactory().CreateKeyed(new[] { En });
        db.SetTranslation("ZEBRA", En, "z");
        db.SetTranslation("APPLE", En, "a");
        db.SetTranslation("MONKEY", En, "m");

        var props = CreateExporter().Export(db, En);
        var lines = props.Split('\n');
        var dataLines = System.Array.FindAll(lines, l => l.Contains("="));
        Assert.StartsWith("APPLE=", dataLines[0]);
        Assert.StartsWith("MONKEY=", dataLines[1]);
        Assert.StartsWith("ZEBRA=", dataLines[2]);
    }
}
