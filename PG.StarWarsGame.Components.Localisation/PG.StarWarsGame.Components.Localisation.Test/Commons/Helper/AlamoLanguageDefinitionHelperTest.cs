// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Attributes;
using PG.StarWarsGame.Components.Localisation.Attributes;
using PG.StarWarsGame.Components.Localisation.Commons.Helper;
using PG.StarWarsGame.Components.Localisation.Languages;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Components.Localisation.Test.Commons.Helper;

public class AlamoLanguageDefinitionHelperTest
{
    private readonly AlamoLanguageDefinitionHelper _helper;

    public AlamoLanguageDefinitionHelperTest()
    {
        var fs = new MockFileSystem();
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(_ => fs);
        _helper = new AlamoLanguageDefinitionHelper(sc.BuildServiceProvider());
    }

    [Fact]
    public void Test_GetAllAlamoLanguageDefinitions__ReturnsAtLeastDefaults()
    {
        var definitions = _helper
            .GetAllRegisteredAlamoLanguageDefinitions();
        Assert.True(definitions.Count >= LocalisationTestConstants.RegisteredLanguageDefinitions.Count,
            "This function should always at least return the default language definitions.");

        var actualNumberOfLanguageDefinitionsMarkedAsOfficial = 0;
        var actualDefaultLanguageCount = 0;
        foreach (var alamoLanguageDefinition in definitions)
        {
            if (alamoLanguageDefinition.GetType().GetAttributeValueOrDefault((OfficiallySupportedLanguageAttribute o) => o.IsOfficiallySupported))
            {
                actualNumberOfLanguageDefinitionsMarkedAsOfficial++;
            }

            if (alamoLanguageDefinition.GetType().GetAttributeValueOrDefault((DefaultLanguageAttribute d) => d.IsDefaultLanguage))
            {
                actualDefaultLanguageCount++;
            }
        }

        Assert.Equal(LocalisationTestConstants.RegisteredLanguageDefinitions.Count, actualNumberOfLanguageDefinitionsMarkedAsOfficial);
        Assert.Equal(1, actualDefaultLanguageCount);
    }

    [Fact]
    public void Test_GetDefaultAlamoLanguageDefinition__ReturnsCorrectLanguage()
    {
        var defaultLanguage = _helper.GetDefaultAlamoLanguageDefinition();
        Assert.Equal(LocalisationTestConstants.DefaultLanguage, defaultLanguage.GetType());
        var actual =
            Activator.CreateInstance(LocalisationTestConstants.DefaultLanguage) as IAlamoLanguageDefinition;
        Assert.Equal(actual, defaultLanguage);
    }
}