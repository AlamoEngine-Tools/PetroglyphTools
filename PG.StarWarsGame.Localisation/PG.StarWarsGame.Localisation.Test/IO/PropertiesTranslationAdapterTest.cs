// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.IO;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Localisation.Data;
using PG.StarWarsGame.Localisation.IO.Properties;
using PG.StarWarsGame.Localisation.Languages;
using Xunit;

namespace PG.StarWarsGame.Localisation.Test.IO;

public class PropertiesTranslationAdapterTest : CommonLocalisationTestBase
{
    private static readonly IAlamoLanguageDefinition En = new EnglishAlamoLanguageDefinition();

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
}
