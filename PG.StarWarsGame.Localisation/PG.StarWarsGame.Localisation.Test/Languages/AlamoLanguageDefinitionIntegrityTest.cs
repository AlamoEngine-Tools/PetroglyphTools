// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Linq;
using System.Reflection;
using PG.StarWarsGame.Localisation.Languages;
using PG.StarWarsGame.Localisation.Languages.Attributes;
using Xunit;

namespace PG.StarWarsGame.Localisation.Test.Languages;

public class AlamoLanguageDefinitionIntegrityTest
{
    private static readonly Assembly LocalisationAssembly =
        typeof(IAlamoLanguageDefinition).Assembly;

    [Fact]
    public void ExactlyElevenConcreteLanguagesAreDefined()
    {
        var count = LocalisationAssembly.GetTypes()
            .Count(t => t is { IsClass: true, IsAbstract: false }
                        && typeof(IAlamoLanguageDefinition).IsAssignableFrom(t));
        Assert.Equal(11, count);
    }

    [Fact]
    public void ExactlyOneDefaultLanguageIsDefined()
    {
        var defaults = LocalisationAssembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false }
                        && typeof(IAlamoLanguageDefinition).IsAssignableFrom(t)
                        && t.IsDefined(typeof(DefaultLanguageAttribute), false))
            .ToList();
        Assert.Single(defaults);
    }

    [Fact]
    public void DefaultLanguageIsEnglish()
    {
        var defaultType = LocalisationAssembly.GetTypes()
            .Single(t => t is { IsClass: true, IsAbstract: false }
                         && typeof(IAlamoLanguageDefinition).IsAssignableFrom(t)
                         && t.IsDefined(typeof(DefaultLanguageAttribute), false));
        var instance = (IAlamoLanguageDefinition)System.Activator.CreateInstance(defaultType)!;
        Assert.Equal("ENGLISH", instance.LanguageIdentifier);
    }

    [Fact]
    public void AllConcreteLanguagesAreOfficiallySupported()
    {
        var langs = LocalisationAssembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false }
                        && typeof(IAlamoLanguageDefinition).IsAssignableFrom(t))
            .Select(t => (IAlamoLanguageDefinition)System.Activator.CreateInstance(t)!)
            .ToList();

        Assert.All(langs, l => Assert.True(l.IsOfficiallySupported));
    }

    [Fact]
    public void AllConcreteLanguagesHaveUniqueIdentifiers()
    {
        var identifiers = LocalisationAssembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false }
                        && typeof(IAlamoLanguageDefinition).IsAssignableFrom(t))
            .Select(t => ((IAlamoLanguageDefinition)System.Activator.CreateInstance(t)!).LanguageIdentifier)
            .ToList();

        Assert.Equal(identifiers.Count, identifiers.Distinct().Count());
    }
}
