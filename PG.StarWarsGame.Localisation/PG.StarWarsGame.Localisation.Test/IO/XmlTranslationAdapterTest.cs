// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Xml.Linq;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Localisation.Data;
using PG.StarWarsGame.Localisation.IO.Xml;
using System.Linq;
using PG.StarWarsGame.Localisation.Languages;
using Xunit;

namespace PG.StarWarsGame.Localisation.Test.IO;

public class XmlTranslationAdapterTest : CommonLocalisationTestBase
{
    private static readonly IAlamoLanguageDefinition En = new EnglishAlamoLanguageDefinition();
    private static readonly IAlamoLanguageDefinition De = new GermanAlamoLanguageDefinition();

    private IXmlTranslationImporter CreateImporter() =>
        ServiceProvider.GetRequiredService<IXmlTranslationImporter>();

    private IXmlTranslationExporter CreateExporter() =>
        ServiceProvider.GetRequiredService<IXmlTranslationExporter>();

    [Fact]
    public void RoundTrip_MultiLanguage_PreservesAllTranslations()
    {
        var db = new TranslationDatabaseFactory().CreateKeyed(new[] { En, De });
        db.SetTranslation("HELLO", En, "Hello");
        db.SetTranslation("HELLO", De, "Hallo");
        db.SetTranslation("BYE", En, "Bye");
        db.SetTranslation("BYE", De, "Tschüss");

        var xml = CreateExporter().Export(db);

        var db2 = new TranslationDatabaseFactory().CreateKeyed(new[] { En, De });
        CreateImporter().Import(xml, db2);

        Assert.True(db2.TryGetEntry("HELLO", out var hello));
        Assert.True(hello!.TryGetTranslation(En, out var en));
        Assert.True(hello.TryGetTranslation(De, out var de));
        Assert.Equal("Hello", en);
        Assert.Equal("Hallo", de);
    }

    [Fact]
    public void Export_ProducesValidXml()
    {
        var db = new TranslationDatabaseFactory().CreateKeyed(new[] { En });
        db.SetTranslation("KEY", En, "Value");

        var xml = CreateExporter().Export(db);
        Assert.NotNull(xml.Root);
    }

    [Fact]
    public void Import_EmptyXml_LeavesDbEmpty()
    {
        var xml = new XDocument(new XElement(
            XName.Get("LocalisationData", "http://www.example.org/eaw-translation/")));
        var db = new TranslationDatabaseFactory().CreateKeyed(new[] { En });
        CreateImporter().Import(xml, db);
        Assert.Empty(db);
    }

    [Fact]
    public void Import_UnknownLanguage_IsSkipped()
    {
        var db = new TranslationDatabaseFactory().CreateKeyed(new[] { En });
        db.SetTranslation("K", En, "v");
        var xml = CreateExporter().Export(db);

        var db2 = new TranslationDatabaseFactory().CreateKeyed(new[] { De });
        CreateImporter().Import(xml, db2);
        Assert.Empty(db2);
    }

    [Fact]
    public void Export_KeyedDatabase_WritesKeysAlphabetically()
    {
        var db = new TranslationDatabaseFactory().CreateKeyed(new[] { En });
        db.SetTranslation("ZEBRA", En, "z");
        db.SetTranslation("APPLE", En, "a");
        db.SetTranslation("MONKEY", En, "m");

        var xml = CreateExporter().Export(db);
        XNamespace ns = "http://www.example.org/eaw-translation/";
        var keys = xml.Root!
            .Elements(ns + "Localisation")
            .Select(e => e.Attribute("key")!.Value)
            .ToList();

        Assert.Equal(new[] { "APPLE", "MONKEY", "ZEBRA" }, keys);
    }
}
