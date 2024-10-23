// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.IO.Abstractions;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Extensibility;
using PG.StarWarsGame.Components.Localisation.IO;
using PG.StarWarsGame.Components.Localisation.IO.Xml;
using PG.StarWarsGame.Components.Localisation.Repository.Builtin;
using PG.StarWarsGame.Components.Localisation.Repository.Content;
using PG.Testing;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Components.Localisation.Test.IO.Xml;

public class XmlImportHandlerTest
{
    private static readonly string EmptyXml = "empty.xml";
    private static readonly string MultiKeysXml = "multi_keys.xml";
    private static readonly string SingleKeyXml = "single_key.xml";

    private readonly MockFileSystem _fileSystem = new();
    private readonly IImportHandler<XmlInputStrategy> _handler;

    public XmlImportHandlerTest()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(_fileSystem);
        sc.CollectPgServiceContributions();
        _handler = sc.BuildServiceProvider().GetRequiredService<IImportHandler<XmlInputStrategy>>();
    }

    private string CreateMockFilePath(string directory, string fileName)
    {
        return _fileSystem.Path.Combine(_fileSystem.Directory.CreateDirectory(directory).FullName, fileName);
    }

    private static string GetResourcePath(string resourceName)
    {
        return $"IO.Xml.v1.Serializable.translation_manifest_{resourceName}";
    }

    [Fact]
    public void Test_Import_WithEmptyXml()
    {
        var filePath = CreateMockFilePath("./empty_test", EmptyXml);
        var resourcePath = GetResourcePath(EmptyXml);
        TestUtility.CopyEmbeddedResourceToMockFilesystem(typeof(XmlImportHandlerTest), resourcePath, filePath,
            _fileSystem);
        var repository = new InMemoryOrderedTranslationRepository();
        _handler.Import(new XmlInputStrategy(filePath), repository);
        Assert.Empty(repository.Content);
    }

    [Fact]
    public void Test_Import_WithSingleKeyXml()
    {
        var filePath = CreateMockFilePath("./single_key_test", SingleKeyXml);
        var resourcePath = GetResourcePath(SingleKeyXml);
        TestUtility.CopyEmbeddedResourceToMockFilesystem(typeof(XmlImportHandlerTest), resourcePath, filePath,
            _fileSystem);
        var repository = new InMemoryOrderedTranslationRepository();
        _handler.Import(new XmlInputStrategy(filePath), repository);
        Assert.Equal(5, repository.Content.Keys.Count());
        Assert.Equal(5, repository.Content.Values.Count());
        foreach (var kvp in repository.Content)
        {
            Assert.Single(kvp.Value);
            Assert.NotNull(kvp.Value.First());
            Assert.True(OrderedTranslationItemId.Of("TEST_KEY_00")?.Equals(kvp.Value.First().ItemId));
            Assert.Equal(new TranslationItemContent { Key = "TEST_KEY_00", Value = "Test text for key TEST_KEY_00" },
                kvp.Value.First().Content);
        }
    }

    [Fact]
    public void Test_Import_WithMultiKeyXml()
    {
        var filePath = CreateMockFilePath("./multi_keys_test", MultiKeysXml);
        var resourcePath = GetResourcePath(MultiKeysXml);
        TestUtility.CopyEmbeddedResourceToMockFilesystem(typeof(XmlImportHandlerTest), resourcePath, filePath,
            _fileSystem);
        var repository = new InMemoryOrderedTranslationRepository();
        _handler.Import(new XmlInputStrategy(filePath), repository);
        Assert.Equal(5, repository.Content.Keys.Count());
        Assert.Equal(5, repository.Content.Values.Count());

        foreach (var kvp in repository.Content) Assert.Equal(2, kvp.Value.Count);
    }
}
