// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Components.Localisation.IO.Formats.Builtin.Csv;
using PG.StarWarsGame.Components.Localisation.IO.Formats.Builtin.Csv.Translation;
using PG.StarWarsGame.Components.Localisation.Languages.Builtin;
using PG.StarWarsGame.Components.Localisation.Repository;
using PG.StarWarsGame.Components.Localisation.Repository.Translation;
using PG.Testing;

namespace PG.StarWarsGame.Components.Localisation.Test.IO.Formats.Builtin.Csv.Translation;

[TestClass]
public class TranslationCsvFormatterTest
{
    private readonly string _directoryPath =
        $"/tst/{nameof(TranslationCsvFormatterTest)}/{DateTimeOffset.Now.ToUnixTimeSeconds()}/";

    private TranslationCsvFormatter? Formatter { get; set; }
    private TranslationCsvFormatDescriptor? Descriptor { get; set; }

    private IDirectoryInfo? Directory { get; set; }

    [TestInitialize]
    public void TestInitialize()
    {
        Descriptor = new TranslationCsvFormatDescriptor(TestConstants.Services);
        Formatter = new TranslationCsvFormatter(Descriptor, TestConstants.Services);
        var fs = TestConstants.Services.GetService<IFileSystem>();
        Debug.Assert(fs != null, nameof(fs) + " != null");
        Directory = fs.Directory.CreateDirectory(_directoryPath);
        Debug.Assert(Directory != null, nameof(Directory) + " != null");
        Assert.IsTrue(fs.Directory.Exists(Directory.FullName));
    }

    [TestCleanup]
    public void TestCleanup()
    {
        Descriptor = null;
        Formatter = null;
        Directory = null;
    }

    private static ITranslationRepository CreateRepository()
    {
        var en = new EnglishAlamoLanguageDefinition();
        var de = new GermanAlamoLanguageDefinition();

        var repository = new TranslationRepository(TestConstants.Services);

        var itemRepository0 = new TranslationItemRepository(TestConstants.Services);
        var itemRepository1 = new TranslationItemRepository(TestConstants.Services);
        var itemRepository2 = new TranslationItemRepository(TestConstants.Services);
        var itemRepository3 = new TranslationItemRepository(TestConstants.Services);

        itemRepository0.TryCreate(en, new TranslationItem("KEY_00", "VALUE_00 - ENGLISH"));
        itemRepository0.TryCreate(de, new TranslationItem("KEY_00", "VALUE_00 - GERMAN"));

        itemRepository1.TryCreate(en, new TranslationItem("KEY_01", "VALUE_01 - ENGLISH"));
        itemRepository1.TryCreate(de, new TranslationItem("KEY_01", "VALUE_01 - GERMAN"));

        itemRepository2.TryCreate(en, new TranslationItem("KEY_02", "VALUE_02 - ENGLISH"));
        // itemRepository2.TryCreate(german, new TranslationItem("KEY_02", "VALUE_02 - GERMAN"));

        // itemRepository3.TryCreate(english, new TranslationItem("KEY_03", "VALUE_03 - ENGLISH"));
        itemRepository3.TryCreate(de, new TranslationItem("KEY_03", "VALUE_03 - GERMAN"));

        repository.TryCreate("KEY_00", itemRepository0);
        repository.TryCreate("KEY_01", itemRepository1);
        repository.TryCreate("KEY_02", itemRepository2);
        repository.TryCreate("KEY_03", itemRepository3);

        return repository;
    }

    [TestMethod]
    public void Test_CreateCsvFiles__ValidOutput()
    {
        Debug.Assert(Directory != null, nameof(Directory) + " != null");
        var param = new CsvFormatterProcessingInstructionsParam()
        {
            Directory = Directory.FullName, 
            FileName = "testfile", 
            Separator= '='
        };
        Debug.Assert(Formatter != null, nameof(Formatter) + " != null");
        Formatter.WithProcessingInstructions(param);
        Formatter.ValidateAndThrow();
        var repository = CreateRepository();
        Formatter.Export(repository);

        var fs = TestConstants.Services.GetService<IFileSystem>();
        Debug.Assert(fs != null, nameof(fs) + " != null");

        var fileEn = Directory.FullName + fs.Path.DirectorySeparatorChar +
                     "testfile.en.csv"; // Workaround for IPath.Join shenanigans.
        var fileDe = Directory.FullName + fs.Path.DirectorySeparatorChar +
                     "testfile.de.csv"; // Workaround for IPath.Join shenanigans.
        Assert.IsTrue(fs.File.Exists(fileEn));
        Assert.IsTrue(fs.File.Exists(fileDe));

        var enContent = fs.File.ReadLines(fileEn);
        var deContent = fs.File.ReadLines(fileEn);

        Assert.IsNotNull(enContent);
        Assert.IsNotNull(deContent);

        Assert.AreNotEqual(enContent, deContent);
    }
}