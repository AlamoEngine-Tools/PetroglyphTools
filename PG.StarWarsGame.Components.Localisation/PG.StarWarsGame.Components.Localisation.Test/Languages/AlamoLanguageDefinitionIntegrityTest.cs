// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Attributes;
using PG.Commons.Extensibility;
using PG.StarWarsGame.Components.Localisation.Attributes;
using PG.StarWarsGame.Components.Localisation.Commons.Helper;
using Testably.Abstractions.Testing;
using Xunit;

namespace PG.StarWarsGame.Components.Localisation.Test.Languages;

public class AlamoLanguageDefinitionIntegrityTest
{
    private readonly IAlamoLanguageDefinitionHelper _helper;

    public AlamoLanguageDefinitionIntegrityTest()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(_ => new MockFileSystem());
        sc.CollectPgServiceContributions();
        var sp = sc.BuildServiceProvider();
        _helper = sp.GetRequiredService<IAlamoLanguageDefinitionHelper>();
    }

    [Fact]
    public void Test_CorrectNumberOfLanguages()
    {
        var l = _helper.GetAllRegisteredAlamoLanguageDefinitions();
        Assert.Equal(LocalisationTestConstants.RegisteredLanguageDefinitions.Count, l.Count);
    }

    [Fact]
    public void Test_CorrectLanguagesRegistered()
    {
        var l = _helper.GetAllRegisteredAlamoLanguageDefinitions();
        foreach (var alamoLanguageDefinition in l)
        {
            Assert.Contains(alamoLanguageDefinition.GetType(), LocalisationTestConstants.RegisteredLanguageDefinitions);
            Assert.True(alamoLanguageDefinition.GetType().GetAttributeValueOrDefault((OfficiallySupportedLanguageAttribute o) => o.IsOfficiallySupported));
        }
    }

    [Fact]
    public void Test_DefaultLanguageIsDefined()
    {
        var l = _helper.GetAllRegisteredAlamoLanguageDefinitions();
        var isDefaultDefined = false;
        foreach (var alamoLanguageDefinition in l)
        {
            if (alamoLanguageDefinition.GetType().GetAttributeValueOrDefault((DefaultLanguageAttribute d) => d.IsDefaultLanguage))
            {
                isDefaultDefined = true;
            }
        }

        Assert.True(isDefaultDefined, "No default language is defined. This should not happen. EnglishAlamoLanguageDefinition should have the DefaultLanguage attribute set.");
    }

    [Fact]
    public void Test_DefaultLanguageIsCorrect()
    {
        var l = _helper.GetDefaultAlamoLanguageDefinition();
        Assert.Equal(LocalisationTestConstants.DefaultLanguage, l.GetType());
    }
}