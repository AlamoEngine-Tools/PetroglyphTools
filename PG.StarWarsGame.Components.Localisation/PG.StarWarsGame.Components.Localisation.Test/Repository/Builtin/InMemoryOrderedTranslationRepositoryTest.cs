// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Extensibility;
using PG.StarWarsGame.Components.Localisation.Languages.BuiltIn;
using PG.StarWarsGame.Components.Localisation.Repository.Builtin;
using PG.StarWarsGame.Components.Localisation.Repository.Content;
using PG.StarWarsGame.Components.Localisation.Services;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Components.Localisation.Test.Repository.Builtin;

public class InMemoryOrderedTranslationRepositoryTest
{
    private readonly MockFileSystem _fileSystem = new();
    private readonly IServiceProvider _serviceProvider;

    public InMemoryOrderedTranslationRepositoryTest()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(_fileSystem);
        sc.CollectPgServiceContributions();
        _serviceProvider = sc.BuildServiceProvider();
    }

    [Fact]
    public void Test_AddLanguage_AllPresent()
    {
        var repository = new InMemoryOrderedTranslationRepository();
        var service = _serviceProvider.GetService<IAlamoLanguageSupportService>();
        Assert.NotNull(service);
        foreach (var l in service.GetRegisteredLanguages()) Assert.True(repository.AddLanguage(l));
        Assert.NotEmpty(repository.Content);
        Assert.Equal(service.GetRegisteredLanguages().Count, repository.Content.Count);
    }

    [Fact]
    public void Test_RemoveLanguage_AllPresentMinusOne()
    {
        var repository = new InMemoryOrderedTranslationRepository();
        var service = _serviceProvider.GetService<IAlamoLanguageSupportService>();
        Assert.NotNull(service);
        foreach (var l in service.GetRegisteredLanguages()) Assert.True(repository.AddLanguage(l));
        Assert.NotEmpty(repository.Content);
        Assert.Equal(service.GetRegisteredLanguages().Count, repository.Content.Count);

        repository.RemoveLanguage(service.GetDefaultLanguageDefinition());
        Assert.NotEmpty(repository.Content);
        Assert.Equal(service.GetRegisteredLanguages().Count - 1, repository.Content.Count);
    }

    [Fact]
    public void Test_AddOrUpdateTranslationItem_Add()
    {
        var repository = new InMemoryOrderedTranslationRepository();
        var service = _serviceProvider.GetService<IAlamoLanguageSupportService>();
        Assert.NotNull(service);
        foreach (var l in service.GetRegisteredLanguages()) Assert.True(repository.AddLanguage(l));
        Assert.NotEmpty(repository.Content);
        Assert.Equal(service.GetRegisteredLanguages().Count, repository.Content.Count);

        repository.AddOrUpdateTranslationItem(new EnglishAlamoLanguageDefinition(),
            OrderedTranslationItem.Of(new TranslationItemContent { Key = "TEST_00", Value = "Test translation" }));

        Assert.Single(repository.Content[new EnglishAlamoLanguageDefinition()]);
        foreach (var l in service.GetRegisteredLanguages())
        {
            if (l.Equals(new EnglishAlamoLanguageDefinition())) continue;
            Assert.Empty(repository[l]);
        }

        Assert.NotNull(repository.GetTranslationItem(new EnglishAlamoLanguageDefinition(),
            OrderedTranslationItemId.Of("TEST_00")!));
    }
}
