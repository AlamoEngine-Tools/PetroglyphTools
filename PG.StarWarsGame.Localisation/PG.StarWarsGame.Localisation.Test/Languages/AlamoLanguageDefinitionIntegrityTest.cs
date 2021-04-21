// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Core.Attributes;
using PG.Core.Localisation;
using PG.Core.Localisation.Attributes;
using PG.Core.Test;
using PG.StarWarsGame.Localisation.Util;

namespace PG.StarWarsGame.Localisation.Test.Languages
{
    [TestClass]
    [TestCategory(TestConstants.TEST_TYPE_INTEGRITY)]
    [TestCategory(TestConstants.TEST_TYPE_HOLY)]
    public class AlamoLanguageDefinitionIntegrityTest
    {
        [TestMethod]
        public void Test_CorrectNumberOfLanguages()
        {
            IList<IAlamoLanguageDefinition> l = LocalisationUtility.GetAllAlamoLanguageDefinitions();
            Assert.AreEqual(LocalisationTestConstants.REGISTERED_LANGUAGE_DEFINITIONS.Count, l.Count, "An official language definition has been added or removed. This should never happen - if there is a good reason for this, please update LocalisationTestConstants.REGISTERED_LANGUAGE_DEFINITIONS accordingly.");
        }

        [TestMethod]
        public void Test_CorrectLanguagesRegistered()
        {
            IList<IAlamoLanguageDefinition> l = LocalisationUtility.GetAllAlamoLanguageDefinitions();
            foreach (IAlamoLanguageDefinition alamoLanguageDefinition in l)
            {
                Assert.IsTrue(
                    LocalisationTestConstants.REGISTERED_LANGUAGE_DEFINITIONS.Contains(
                        alamoLanguageDefinition.GetType()));
                Assert.IsTrue(alamoLanguageDefinition.GetType().GetAttributeValueOrDefault((OfficiallySupportedLanguageAttribute o) => o.IsOfficiallySupported));
            }
        }

        [TestMethod]
        public void Test_DefaultLanguageIsDefined()
        {
            IList<IAlamoLanguageDefinition> l = LocalisationUtility.GetAllAlamoLanguageDefinitions();
            bool isDefaultDefined = false;
            foreach (IAlamoLanguageDefinition alamoLanguageDefinition in l)
            {
                if (alamoLanguageDefinition.GetType().GetAttributeValueOrDefault((DefaultAttribute d) => d.IsDefault))
                {
                    isDefaultDefined = true;
                }
            }
            Assert.IsTrue(isDefaultDefined, "No default language is defined. This should not happen. EnglishAlamoLanguageDefinition should have the Default attribute set.");
        }

        [TestMethod]
        public void Test_DefaultLanguageIsCorrect()
        {
            IAlamoLanguageDefinition l = LocalisationUtility.GetDefaultAlamoLanguageDefinition();
            Assert.AreEqual(LocalisationTestConstants.DEFAULT_LANGUAGE, l.GetType(), "The default language is not EnglishAlamoLanguageDefinition. This should never happen. Please ensure EnglishAlamoLanguageDefinition has the Default attribute set.");
        }
    }
}
