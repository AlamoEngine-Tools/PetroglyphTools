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

    // The EaW text editor's file, as it writes it.
    [Fact]
    public void Import_TheTextEditorsFile_ReadsEveryTranslation()
    {
        var xml = XDocument.Parse("""
                                  <?xml version="1.0"?>
                                  <LocalisationData xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
                                    <Localisation Key="TEXT_HOTH">
                                      <TranslationData>
                                        <Translation Language="GERMAN"><![CDATA[Angriff auf Hoth]]></Translation>
                                        <Translation Language="ENGLISH"><![CDATA[Assault on Hoth]]></Translation>
                                      </TranslationData>
                                    </Localisation>
                                  </LocalisationData>
                                  """);
        var db = new TranslationDatabaseFactory().CreateKeyed(new[] { En, De });

        CreateImporter().Import(xml, db);

        Assert.True(db.TryGetEntry("TEXT_HOTH", out var hoth));
        Assert.True(hoth!.TryGetTranslation(En, out var en));
        Assert.True(hoth.TryGetTranslation(De, out var de));
        Assert.Equal("Assault on Hoth", en);
        Assert.Equal("Angriff auf Hoth", de);
    }

    // The shape earlier builds of this library wrote: a namespace, key, one TranslationData per language.
    [Fact]
    public void Import_TheNamespacedFile_ReadsEveryTranslation()
    {
        var xml = XDocument.Parse("""
                                  <LocalisationData xmlns="urn:alamoenginetools:localisation:v1">
                                    <Localisation key="TEXT_HOTH">
                                      <TranslationData><Translation Language="ENGLISH">Assault on Hoth</Translation></TranslationData>
                                      <TranslationData><Translation Language="GERMAN">Angriff auf Hoth</Translation></TranslationData>
                                    </Localisation>
                                  </LocalisationData>
                                  """);
        var db = new TranslationDatabaseFactory().CreateKeyed(new[] { En, De });

        CreateImporter().Import(xml, db);

        Assert.True(db.TryGetEntry("TEXT_HOTH", out var hoth));
        Assert.True(hoth!.TryGetTranslation(En, out var en));
        Assert.True(hoth.TryGetTranslation(De, out var de));
        Assert.Equal("Assault on Hoth", en);
        Assert.Equal("Angriff auf Hoth", de);
    }

    [Theory]
    [InlineData("<LocalisationData/>", "")]
    [InlineData("<LocalisationData xmlns=\"urn:alamoenginetools:localisation:v1\"/>", "urn:alamoenginetools:localisation:v1")]
    public void Of_ADocument_IsTheShapeItIsWrittenIn(string text, string ns)
    {
        var format = XmlTranslationFormat.Of(XDocument.Parse(text));

        Assert.Equal(XName.Get("LocalisationData", ns), format.Root);
    }

    // By default a file is written in the library's namespace, keyed by key.
    [Fact]
    public void Export_WritesTheNamespacedShape()
    {
        var db = new TranslationDatabaseFactory().CreateKeyed(new[] { En });
        db.SetTranslation("HELLO", En, "Hello");

        var root = CreateExporter().Export(db).Root!;

        XNamespace ns = "urn:alamoenginetools:localisation:v1";
        Assert.Equal(ns + "LocalisationData", root.Name);
        var entry = Assert.Single(root.Elements(ns + "Localisation"));
        Assert.Equal("HELLO", entry.Attribute("key")?.Value);
        Assert.Equal("Hello", entry.Element(ns + "TranslationData")!.Element(ns + "Translation")!.Value);
    }

    // The text editor's format: no namespace, Key, one TranslationData per key.
    [Fact]
    public void Export_OmittingTheNamespace_WritesTheTextEditorsFormat()
    {
        var db = new TranslationDatabaseFactory().CreateKeyed(new[] { En, De });
        db.SetTranslation("HELLO", En, "Hello");
        db.SetTranslation("HELLO", De, "Hallo");

        var root = CreateExporter().Export(db, omitNamespace: true).Root!;

        Assert.Equal(XName.Get("LocalisationData"), root.Name);
        var entry = Assert.Single(root.Elements("Localisation"));
        Assert.Equal("HELLO", entry.Attribute("Key")?.Value);
        var data = Assert.Single(entry.Elements("TranslationData"));
        Assert.Equal(new[] { "ENGLISH", "GERMAN" },
            data.Elements("Translation").Select(t => t.Attribute("Language")!.Value));
    }

    [Fact]
    public void Import_EmptyXml_LeavesDbEmpty()
    {
        var xml = new XDocument(new XElement("LocalisationData"));
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
    public void RoundTrip_OrderedDatabase_MultiLanguage_PreservesRowsAndOrder()
    {
        var db = new TranslationDatabaseFactory().CreateOrdered(new[] { En, De });
        db.SetTranslation("CREDIT", En, "Line 1");
        db.SetTranslationAt(0, De, "Zeile 1");
        db.SetTranslation("CREDIT", En, "Line 2");
        db.SetTranslationAt(1, De, "Zeile 2");

        var xml = CreateExporter().Export(db);
        var db2 = new TranslationDatabaseFactory().CreateOrdered(new[] { En, De });
        CreateImporter().Import(xml, db2);

        Assert.Equal(2, db2.Count);
        Assert.Equal(new[] { "CREDIT", "CREDIT" }, db2.Select(e => e.Key));

        Assert.True(db2[0].TryGetTranslation(En, out var en0));
        Assert.True(db2[0].TryGetTranslation(De, out var de0));
        Assert.True(db2[1].TryGetTranslation(De, out var de1));
        Assert.Equal("Line 1", en0);
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

        var xml = CreateExporter().Export(db, omitNamespace: true);
        var keys = xml.Root!
            .Elements("Localisation")
            .Select(e => e.Attribute("Key")!.Value)
            .ToList();

        Assert.Equal(new[] { "APPLE", "MONKEY", "ZEBRA" }, keys);
    }
}
