// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.DAT.Services.Builder;
using PG.StarWarsGame.Localisation.Data;
using PG.StarWarsGame.Localisation.IO.Dat;
using PG.StarWarsGame.Localisation.Languages;
using Xunit;

namespace PG.StarWarsGame.Localisation.Test.IO;

public class DatTranslationAdapterTest : CommonLocalisationTestBase
{
    private static readonly IAlamoLanguageDefinition En = new EnglishAlamoLanguageDefinition();

    private IDatTranslationImporter CreateImporter() =>
        ServiceProvider.GetRequiredService<IDatTranslationImporter>();

    private IDatTranslationExporter CreateExporter() =>
        ServiceProvider.GetRequiredService<IDatTranslationExporter>();

    [Fact]
    public void RoundTrip_KeyedDatabase_PreservesEntriesAndValues()
    {
        var builder = new EmpireAtWarMasterTextBuilder(false, ServiceProvider);
        builder.AddEntry("HELLO", "Hello World");
        builder.AddEntry("GOODBYE", "Goodbye World");
        var model = builder.BuildModel();

        var db = new TranslationDatabaseFactory().CreateKeyed(new[] { En });
        CreateImporter().Import(model, En, db);

        Assert.True(db.ContainsKey("HELLO"));
        Assert.True(db.ContainsKey("GOODBYE"));
        Assert.True(db.TryGetEntry("HELLO", out var entry));
        Assert.True(entry!.TryGetTranslation(En, out var val));
        Assert.Equal("Hello World", val);
    }

    [Fact]
    public void RoundTrip_OrderedDatabase_PreservesOrder()
    {
        var builder = new EmpireAtWarCreditsTextBuilder(ServiceProvider);
        builder.AddEntry("LINE", "First");
        builder.AddEntry("LINE", "Second");
        var model = builder.BuildModel();

        var db = new TranslationDatabaseFactory().CreateOrdered(new[] { En });
        CreateImporter().Import(model, En, db);

        Assert.Equal(2, db.Count);
        Assert.True(db[0].TryGetTranslation(En, out var first));
        Assert.True(db[1].TryGetTranslation(En, out var second));
        Assert.Equal("First", first);
        Assert.Equal("Second", second);
    }

    [Fact]
    public void Export_KeyedDatabase_ProducesEquivalentDatModel()
    {
        var db = new TranslationDatabaseFactory().CreateKeyed(new[] { En });
        db.SetTranslation("HELLO", En, "Hello");
        db.SetTranslation("BYE", En, "Bye");

        var exported = CreateExporter().Export(db, En);
        Assert.Equal(2, exported.Count);
        Assert.Equal("Hello", exported.GetValue("HELLO"));
        Assert.Equal("Bye", exported.GetValue("BYE"));
    }

    [Fact]
    public void Export_OrderedDatabase_PreservesInsertionOrder()
    {
        var db = new TranslationDatabaseFactory().CreateOrdered(new[] { En });
        db.SetTranslation("LINE", En, "First");
        db.SetTranslation("LINE", En, "Second");

        var exported = CreateExporter().Export(db, En);
        Assert.Equal(2, exported.Count);
        var entries = new List<Files.DAT.Data.DatStringEntry>(exported);
        Assert.Equal("First", entries[0].Value);
        Assert.Equal("Second", entries[1].Value);
    }

    [Fact]
    public void Import_EmptyModel_LeavesDbEmpty()
    {
        var builder = new EmpireAtWarMasterTextBuilder(false, ServiceProvider);
        var model = builder.BuildModel();
        var db = new TranslationDatabaseFactory().CreateKeyed(new[] { En });
        CreateImporter().Import(model, En, db);
        Assert.Empty(db);
    }
}
