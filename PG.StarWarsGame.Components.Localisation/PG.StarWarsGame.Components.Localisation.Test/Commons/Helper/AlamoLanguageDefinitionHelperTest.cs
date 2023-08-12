// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Attributes;
using PG.StarWarsGame.Components.Localisation.Attributes;
using PG.StarWarsGame.Components.Localisation.Commons.Helper;
using PG.StarWarsGame.Components.Localisation.Languages;
using PG.Testing;

namespace PG.StarWarsGame.Components.Localisation.Test.Commons.Helper;

[TestClass]
public class AlamoLanguageDefinitionHelperTest : ServiceTestBase
{
    [TestMethod]
    public void Test_GetAllAlamoLanguageDefinitions__ReturnsAtLeastDefaults()
    {
        var definitions = GetServiceInstance<AlamoLanguageDefinitionHelper>()
            .GetAllRegisteredAlamoLanguageDefinitions();
        Assert.IsTrue(definitions.Count >= LocalisationTestConstants.RegisteredLanguageDefinitions.Count,
            "This function should always at least return the default language definitions.");
        var actualNumberOfLanguageDefinitionsMarkedAsOfficial = 0;
        var actualDefaultLanguageCount = 0;
        foreach (var alamoLanguageDefinition in definitions)
        {
            if (alamoLanguageDefinition.GetType()
                .GetAttributeValueOrDefault((OfficiallySupportedLanguageAttribute o) => o.IsOfficiallySupported))
            {
                actualNumberOfLanguageDefinitionsMarkedAsOfficial++;
            }

            if (alamoLanguageDefinition.GetType()
                .GetAttributeValueOrDefault((DefaultLanguageAttribute d) => d.IsDefaultLanguage))
            {
                actualDefaultLanguageCount++;
            }
        }

        Assert.AreEqual(LocalisationTestConstants.RegisteredLanguageDefinitions.Count,
            actualNumberOfLanguageDefinitionsMarkedAsOfficial,
            "Someone added a language definition marked as official. This should not happen. Please remove the OfficiallySupportedLanguageAttribute from the offending language.");
        Assert.AreEqual(1, actualDefaultLanguageCount,
            "Someone added a language definition marked as default. This should not happen. Please remove the DefaultAttribute from the offending language.");
    }

    [TestMethod]
    public void Test_GetDefaultAlamoLanguageDefinition__ReturnsCorrectLanguage()
    {
        var l = GetServiceInstance<AlamoLanguageDefinitionHelper>().GetDefaultAlamoLanguageDefinition();
        Assert.AreEqual(LocalisationTestConstants.DefaultLanguage, l.GetType());
        var actual =
            Activator.CreateInstance(LocalisationTestConstants.DefaultLanguage) as IAlamoLanguageDefinition;
        Assert.AreEqual(actual, l);
    }

    protected override Type GetServiceClass()
    {
        return typeof(AlamoLanguageDefinitionHelper);
    }
}