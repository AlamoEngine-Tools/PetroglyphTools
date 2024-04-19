// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Components.Localisation.IO.Formats.Builtin.Csv;
using PG.StarWarsGame.Components.Localisation.IO.Formats.Builtin.Csv.Translation;
using PG.StarWarsGame.Components.Localisation.Languages.Builtin;
using PG.StarWarsGame.Components.Localisation.Repository;
using PG.StarWarsGame.Components.Localisation.Repository.Translation;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Components.Localisation.Test.IO.Formats.Builtin.Csv.Translation;

public class TranslationCsvFormatterTest
{
    private readonly string _directoryPath =
        $"/tst/{nameof(TranslationCsvFormatterTest)}/{DateTimeOffset.Now.ToUnixTimeSeconds()}/";

    private readonly MockFileSystem _fileSystem = new();
    private readonly IServiceProvider _serviceProvider;

    private TranslationCsvFormatter Formatter { get; }
    private TranslationCsvFormatDescriptor Descriptor { get; }

    private IDirectoryInfo Directory { get; }

    public TranslationCsvFormatterTest()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(_ => _fileSystem);
        _serviceProvider = sc.BuildServiceProvider();

        Descriptor = new TranslationCsvFormatDescriptor(_serviceProvider);
        Formatter = new TranslationCsvFormatter(Descriptor, _serviceProvider);

        _fileSystem.Initialize().WithSubdirectory(_directoryPath);
        Directory = _fileSystem.DirectoryInfo.New(_directoryPath);
    }

    private ITranslationRepository CreateRepository()
    {
        var en = new EnglishAlamoLanguageDefinition();
        var de = new GermanAlamoLanguageDefinition();

        var repository = new TranslationRepository(_serviceProvider);

        var itemRepository0 = new TranslationItemRepository(_serviceProvider);
        var itemRepository1 = new TranslationItemRepository(_serviceProvider);
        var itemRepository2 = new TranslationItemRepository(_serviceProvider);
        var itemRepository3 = new TranslationItemRepository(_serviceProvider);

        itemRepository0.TryCreate(en, new TranslationItem("KEY_00", "VALUE_00 - ENGLISH"));
        itemRepository0.TryCreate(de, new TranslationItem("KEY_00", "VALUE_00 - GERMAN"));

        itemRepository1.TryCreate(en, new TranslationItem("KEY_01", "VALUE_01 - ENGLISH"));
        itemRepository1.TryCreate(de, new TranslationItem("KEY_01", "VALUE_01 - GERMAN"));

        itemRepository2.TryCreate(en, new TranslationItem("KEY_02", "VALUE_02 - ENGLISH"));
        itemRepository2.TryCreate(de, new TranslationItem("KEY_02", "VALUE_02 - GERMAN"));

        itemRepository3.TryCreate(en, new TranslationItem("KEY_03", "VALUE_03 - ENGLISH"));
        itemRepository3.TryCreate(de, new TranslationItem("KEY_03", "VALUE_03 - GERMAN"));

        repository.TryCreate("KEY_00", itemRepository0);
        repository.TryCreate("KEY_01", itemRepository1);
        repository.TryCreate("KEY_02", itemRepository2);
        repository.TryCreate("KEY_03", itemRepository3);

        return repository;
    }

    [Fact]
    public void Test_CreateCsvFiles__ValidOutput()
    {
        var param = new CsvFormatterProcessingInstructionsParam
        {
            Directory = Directory.FullName,
            FileName = "testfile",
            Separator = '='
        };

        Formatter.WithProcessingInstructions(param);
        Formatter.ValidateAndThrow();
        var repository = CreateRepository();
        Formatter.Export(repository);

        var fileEn =
            _fileSystem.Path.Combine(Directory.FullName, "testfile.en.csv");
        var fileDe =
            _fileSystem.Path.Combine(Directory.FullName, "testfile.de.csv");
        Assert.True(_fileSystem.File.Exists(fileEn));
        Assert.True(_fileSystem.File.Exists(fileDe));

        var enContent = _fileSystem.File.ReadLines(fileEn);
        var deContent = _fileSystem.File.ReadLines(fileDe);

        Assert.NotNull(enContent);
        Assert.NotNull(deContent);

        Assert.NotEqual(enContent, deContent);
    }
}