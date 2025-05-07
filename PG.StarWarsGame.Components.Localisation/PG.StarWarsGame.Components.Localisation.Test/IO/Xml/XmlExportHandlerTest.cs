// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Extensibility;
using PG.StarWarsGame.Components.Localisation.IO;
using PG.StarWarsGame.Components.Localisation.IO.Xml;
using PG.StarWarsGame.Components.Localisation.Languages.BuiltIn;
using PG.StarWarsGame.Components.Localisation.Repository.Builtin;
using PG.StarWarsGame.Components.Localisation.Repository.Content;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Components.Localisation.Test.IO.Xml;

public class XmlExportHandlerTest
{
    private readonly MockFileSystem _fileSystem = new();
    private readonly IExportHandler<XmlOutputStrategy> _handler;

    public XmlExportHandlerTest()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(_fileSystem);
        sc.CollectPgServiceContributions();
        _handler = sc.BuildServiceProvider().GetRequiredService<IExportHandler<XmlOutputStrategy>>();
    }

    [Fact]
    public void Test_Export_WithEmptyRepository_NoFileCreated()
    {
        var repository = new InMemoryOrderedTranslationRepository();
        Assert.Empty(repository.Content);
        const string exportBase = "./test_export_empty";
        const string exportBaseFile = "EMPTY_EXPORT";
        var strategy = new XmlOutputStrategy(_fileSystem.DirectoryInfo.New(exportBase), exportBaseFile);
        _handler.Export(strategy, repository);
        var info = _fileSystem.FileInfo.New(strategy.FilePath);
        Assert.False(info.Exists);
    }

    [Fact]
    public void Test_Export_WithRepositoryWithoutContentBesidesLanguages_NoFileCreated()
    {
        var repository = new InMemoryOrderedTranslationRepository();
        repository.AddLanguage(new EnglishAlamoLanguageDefinition());
        repository.AddLanguage(new GermanAlamoLanguageDefinition());
        const string exportBase = "./test_export_languages_only";
        const string exportBaseFile = "EMPTY_LANGUAGES";
        var strategy = new XmlOutputStrategy(_fileSystem.DirectoryInfo.New(exportBase), exportBaseFile);
        _handler.Export(strategy, repository);
        var info = _fileSystem.FileInfo.New(strategy.FilePath);
        Assert.False(info.Exists);
    }

    [Fact]
    public void Test_Export_WithRepositoryWithSingleEntry_FileCreated()
    {
        var repository = new InMemoryOrderedTranslationRepository();
        repository.AddLanguage(new EnglishAlamoLanguageDefinition());
        repository.AddOrUpdateTranslationItem(new EnglishAlamoLanguageDefinition(), OrderedTranslationItem.Of(
            new TranslationItemContent
            {
                Key = "TEST_00",
                Value = "English translation for TEST_00"
            }));
        const string exportBase = "./test_single_entry";
        const string exportBaseFile = "VALID_FILE";
        var strategy = new XmlOutputStrategy(_fileSystem.DirectoryInfo.New(exportBase), exportBaseFile);
        _handler.Export(strategy, repository);
        var info = _fileSystem.FileInfo.New(strategy.FilePath);
        Assert.True(info.Exists);
    }
}
