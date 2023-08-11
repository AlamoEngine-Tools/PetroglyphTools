// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Attributes;
using PG.StarWarsGame.Localisation.Attributes;
using PG.StarWarsGame.Localisation.Commons.Helper;
using PG.Testing;

namespace PG.StarWarsGame.Localisation.Test.Languages;

[TestClass]
[TestCategory(TestConstants.TestCategories.HOLY)]
public class AlamoLanguageDefinitionIntegrityTest
{
    private IAlamoLanguageDefinitionHelper GetHelper()
    {
        return new AlamoLanguageDefinitionHelper(TestConstants.Services);
    }

    [TestMethod]
    public void Test_CorrectNumberOfLanguages()
    {
        var l = GetHelper().GetAllRegisteredAlamoLanguageDefinitions();
        Assert.AreEqual(LocalisationTestConstants.RegisteredLanguageDefinitions.Count, l.Count,
            "An official language definition has been added or removed. This should never happen - if there is a good reason for this, please update LocalisationTestConstants.REGISTERED_LANGUAGE_DEFINITIONS accordingly.");
    }

    [TestMethod]
    public void Test_CorrectLanguagesRegistered()
    {
        var l = GetHelper().GetAllRegisteredAlamoLanguageDefinitions();
        foreach (var alamoLanguageDefinition in l)
        {
            Assert.IsTrue(
                LocalisationTestConstants.RegisteredLanguageDefinitions.Contains(
                    alamoLanguageDefinition.GetType()));
            Assert.IsTrue(alamoLanguageDefinition.GetType()
                .GetAttributeValueOrDefault((OfficiallySupportedLanguageAttribute o) => o.IsOfficiallySupported));
        }
    }

    [TestMethod]
    public void Test_DefaultLanguageIsDefined()
    {
        var l = GetHelper().GetAllRegisteredAlamoLanguageDefinitions();
        var isDefaultDefined = false;
        foreach (var alamoLanguageDefinition in l)
        {
            if (alamoLanguageDefinition.GetType()
                .GetAttributeValueOrDefault((DefaultLanguageAttribute d) => d.IsDefaultLanguage))
            {
                isDefaultDefined = true;
            }
        }

        Assert.IsTrue(isDefaultDefined,
            "No default language is defined. This should not happen. EnglishAlamoLanguageDefinition should have the DefaultLanguage attribute set.");
    }

    [TestMethod]
    public void Test_DefaultLanguageIsCorrect()
    {
        var l = GetHelper().GetDefaultAlamoLanguageDefinition();
        Assert.AreEqual(LocalisationTestConstants.DefaultLanguage, l.GetType(),
            "The default language is not EnglishAlamoLanguageDefinition. This should never happen. Please ensure EnglishAlamoLanguageDefinition has the Default attribute set.");
    }
}