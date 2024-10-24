// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Extensibility;
using PG.StarWarsGame.Components.Localisation.Attributes;
using PG.StarWarsGame.Components.Localisation.Languages;
using PG.StarWarsGame.Components.Localisation.Languages.BuiltIn;
using PG.StarWarsGame.Components.Localisation.Services;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Components.Localisation.Test.Services;

public class AlamoLanguageSupportServiceTest
{
    private readonly HashSet<Type> _builtinTypes = new()
    {
        typeof(ChineseAlamoLanguageDefinition),
        typeof(EnglishAlamoLanguageDefinition),
        typeof(FrenchAlamoLanguageDefinition),
        typeof(GermanAlamoLanguageDefinition),
        typeof(ItalianAlamoLanguageDefinition),
        typeof(JapaneseAlamoLanguageDefinition),
        typeof(KoreanAlamoLanguageDefinition),
        typeof(PolishAlamoLanguageDefinition),
        typeof(RussianAlamoLanguageDefinition),
        typeof(SpanishAlamoLanguageDefinition),
        typeof(ThaiAlamoLanguageDefinition)
    };

    private readonly MockFileSystem _fileSystem = new();
    private readonly IAlamoLanguageSupportService _service;

    public AlamoLanguageSupportServiceTest()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(_fileSystem);
        sc.CollectPgServiceContributions();
        _service = sc.BuildServiceProvider().GetRequiredService<IAlamoLanguageSupportService>();
    }

    [Fact]
    public void Test_DefaultLanguageDefinitionDefinedExactlyOnce()
    {
        var languageDefinitions = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assemblyTypes => assemblyTypes.GetTypes())
            .Where(assemblyType => typeof(IAlamoLanguageDefinition).IsAssignableFrom(assemblyType) &&
                                   assemblyType is { IsClass: true, IsAbstract: false })
            .Where(t => t.GetCustomAttribute(typeof(DefaultLanguageAttribute)) != null)
            .ToList();
        Assert.Single(languageDefinitions);
        Assert.NotNull(languageDefinitions[0]);
    }

    [Fact]
    public void Test_GetDefaultLanguageDefinition_ReturnsEnglish()
    {
        var def = _service.GetDefaultLanguageDefinition();
        Assert.NotNull(def);
        Assert.Equal(def, new EnglishAlamoLanguageDefinition());
    }

    [Theory]
    [InlineData(null, typeof(EnglishAlamoLanguageDefinition))]
    [InlineData(typeof(EnglishAlamoLanguageDefinition), typeof(EnglishAlamoLanguageDefinition))]
    [InlineData(typeof(ChineseAlamoLanguageDefinition), typeof(ChineseAlamoLanguageDefinition))]
    public void Test_GetFallbackLanguageDefinition_ReturnsDesired(Type? definition, Type expected)
    {
        var exp = (IAlamoLanguageDefinition)Activator.CreateInstance(expected)!;
        if (definition != null)
            _service.WithOverrideFallbackLanguage((IAlamoLanguageDefinition)Activator.CreateInstance(definition)!);
        else
            _service.WithOverrideFallbackLanguage(new EnglishAlamoLanguageDefinition());
        Assert.Equal(exp, _service.GetFallbackLanguageDefinition());
    }

    [Fact]
    public void Test_IsBuiltInLanguageDefinition_ReturnsTrue()
    {
        foreach (var b in _builtinTypes)
            Assert.True(_service.IsBuiltInLanguageDefinition((IAlamoLanguageDefinition)Activator.CreateInstance(b)!));
    }

    [Fact]
    public void Test_IsBuiltInLanguageDefinition_ReturnsFalse()
    {
        Assert.False(_service.IsBuiltInLanguageDefinition(new TestAlamoLanguageDefinition()));
    }

    [Fact]
    public void Test_AllPresentLanguageIdentifiersAreValid()
    {
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assemblyTypes => assemblyTypes.GetTypes())
            .Where(assemblyType => typeof(IAlamoLanguageDefinition).IsAssignableFrom(assemblyType) &&
                                   assemblyType is { IsClass: true, IsAbstract: false })
            .ToList();
        foreach (var type in types) Assert.True(typeof(AlamoLanguageDefinitionBase).IsAssignableFrom(type));
    }

    [Fact]
    public void Test_CreateLanguageIdentifierMapping_AllPresent()
    {
        var defs = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assemblyTypes => assemblyTypes.GetTypes())
            .Where(assemblyType => typeof(IAlamoLanguageDefinition).IsAssignableFrom(assemblyType) &&
                                   assemblyType is { IsClass: true, IsAbstract: false })
            .Select<Type, IAlamoLanguageDefinition>(t => (IAlamoLanguageDefinition)Activator.CreateInstance(t))
            .ToList();
        var map = _service.CreateLanguageIdentifierMapping();
        Assert.Equal(defs.Count, map.Count);
        foreach (var def in defs)
        {
            Assert.True(typeof(AlamoLanguageDefinitionBase).IsAssignableFrom(def.GetType()));
            Assert.NotNull(map[def.LanguageIdentifier]);
            Assert.Equal(def, map[def.LanguageIdentifier]);
        }
    }

    private class TestAlamoLanguageDefinition : AlamoLanguageDefinitionBase
    {
        protected override string ConfiguredLanguageIdentifier => "TEST";
        protected override CultureInfo ConfiguredCulture => CultureInfo.CurrentCulture;
    }
}
