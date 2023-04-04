// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Core.Attributes;
using PG.Core.Localisation;
using PG.Core.Localisation.Attributes;
using PG.Core.Test;
using PG.StarWarsGame.Localisation.Util;
using System;
using System.Collections.Generic;

namespace PG.StarWarsGame.Localisation.Test.Util
{
    [TestClass]
    [TestCategory(TestConstants.TEST_TYPE_UTILITY)]
    public class LocalisationUtilityTest
    {
        [TestMethod]
        public void Test_GetAllAlamoLanguageDefinitions__ReturnsAtLeastDefaults()
        {
            IList<IAlamoLanguageDefinition> l = LocalisationUtility.GetAllAlamoLanguageDefinitions();
            Assert.IsTrue(l.Count >= LocalisationTestConstants.REGISTERED_LANGUAGE_DEFINITIONS.Count,
                "This function should always at least return the default language definitions.");
            int actualNumberOfLanguageDefinitionsMarkedAsOfficial = 0;
            int actualDefaultLanguageCount = 0;
            foreach (IAlamoLanguageDefinition alamoLanguageDefinition in l)
            {
                if (alamoLanguageDefinition.GetType()
                    .GetAttributeValueOrDefault((OfficiallySupportedLanguageAttribute o) => o.IsOfficiallySupported))
                {
                    actualNumberOfLanguageDefinitionsMarkedAsOfficial++;
                }

                if (alamoLanguageDefinition.GetType()
                    .GetAttributeValueOrDefault((DefaultAttribute d) => d.IsDefault))
                {
                    actualDefaultLanguageCount++;
                }
            }
            Assert.AreEqual(LocalisationTestConstants.REGISTERED_LANGUAGE_DEFINITIONS.Count, actualNumberOfLanguageDefinitionsMarkedAsOfficial,
                "Someone added a language definition marked as official. This should not happen. Please remove the OfficiallySupportedLanguageAttribute from the offending language.");
            Assert.AreEqual(1, actualDefaultLanguageCount,
                "Someone added a language definition marked as default. This should not happen. Please remove the DefaultAttribute from the offending language.");

        }

        [TestMethod]
        public void Test_GetDefaultAlamoLanguageDefinition__ReturnsCorrectLanguage()
        {
            IAlamoLanguageDefinition l = LocalisationUtility.GetDefaultAlamoLanguageDefinition();
            Assert.AreEqual(LocalisationTestConstants.DEFAULT_LANGUAGE, l.GetType());
            IAlamoLanguageDefinition actual = Activator.CreateInstance(LocalisationTestConstants.DEFAULT_LANGUAGE) as IAlamoLanguageDefinition;
            Assert.AreEqual(actual, l);
        }
    }
}
