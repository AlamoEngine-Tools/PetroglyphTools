// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Test;
using PG.Core.Localisation;
using PG.StarWarsGame.Localisation.Languages;
using PG.StarWarsGame.Localisation.Util;

namespace PG.StarWarsGame.Localisation.Test.Util
{
    [TestClass]
    [TestCategory(TestUtility.TEST_TYPE_UTILITY)]
    public class LocalisationUtilityTest
    {
        private class TestAlamoLanguageDefinition : IAlamoLanguageDefinition, IEquatable<IAlamoLanguageDefinition>
        {
            public bool Equals(IAlamoLanguageDefinition other)
            {
                if (other == null)
                {
                    return false;
                }

                return Culture.Equals(other.Culture) && LanguageIdentifier.Equals(other.LanguageIdentifier,
                    StringComparison.InvariantCultureIgnoreCase);
            }

            public TestAlamoLanguageDefinition(string languageIdentifier, CultureInfo culture)
            {
                LanguageIdentifier = languageIdentifier;
                Culture = culture;
            }

            public string LanguageIdentifier { get; }
            public CultureInfo Culture { get; }
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("\r\n\t  \r\n")]
        [DataRow(" ")]
        public void TryGuessAlamoLanguageDefinitionByIdentifier_Test__IfIdentifierInvalidReturnsFalse(string identifier)
        {
            Assert.IsFalse(
                LocalisationUtility.TryGuessAlamoLanguageDefinitionByIdentifier(identifier,
                    out IAlamoLanguageDefinition _));
        }

        [TestMethod]
        public void TryGuessAlamoLanguageDefinitionByIdentifier_Test__GuessBuiltinCorrectly()
        {
            foreach (OfficialLanguage officialLanguage in Enum.GetValues(typeof(OfficialLanguage)))
            {
                TestAlamoLanguageDefinition expected = new TestAlamoLanguageDefinition(
                    officialLanguage.ToAlamoLanguageIdentifierString(),
                    officialLanguage.ToCultureInfo());
                Assert.IsTrue(LocalisationUtility.TryGuessAlamoLanguageDefinitionByIdentifier(
                    officialLanguage.ToString().ToLower(), out IAlamoLanguageDefinition actual));
                Assert.IsTrue(expected.Equals(actual));
            }
        }

        [TestMethod]
        public void TryGuessAlamoLanguageDefinitionByIdentifier_Test__GuessFromUnknown()
        {
            Assert.IsFalse(
                LocalisationUtility.TryGuessAlamoLanguageDefinitionByIdentifier("Unknown",
                    out IAlamoLanguageDefinition actual));
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual is EnglishAlamoLanguageDefinition);
            TestAlamoLanguageDefinition fallback = new TestAlamoLanguageDefinition("test", CultureInfo.CurrentCulture);
            Assert.IsFalse(
                LocalisationUtility.TryGuessAlamoLanguageDefinitionByIdentifier("Unknown",
                    out actual, fallback));
            Assert.IsNotNull(actual);
            Assert.IsTrue(fallback.Equals(actual));
        }

        [TestMethod]
        public void IsOfficialLanguage_Test__AsExpectedForBuiltinTypes()
        {
            Type[] builtins =
            {
                typeof(ChineseAlamoLanguageDefinition),
                typeof(EnglishAlamoLanguageDefinition),
                typeof(FrenchAlamoLanguageDefinition),
                typeof(GermanAlamoLanguageDefinition),
                typeof(GermanAlamoLanguageDefinition),
                typeof(ItalianAlamoLanguageDefinition),
                typeof(JapaneseAlamoLanguageDefinition),
                typeof(KoreanAlamoLanguageDefinition),
                typeof(PolishAlamoLanguageDefinition),
                typeof(RussianAlamoLanguageDefinition),
                typeof(SpanishAlamoLanguageDefinition),
                typeof(ThaiAlamoLanguageDefinition)
            };
            foreach (Type builtin in builtins)
            {
                Assert.IsTrue(
                    LocalisationUtility.IsOfficiallySupportedLanguage(
                        Activator.CreateInstance(builtin) as IAlamoLanguageDefinition));
            }

            Assert.IsFalse(
                LocalisationUtility.IsOfficiallySupportedLanguage(
                    new TestAlamoLanguageDefinition("test", CultureInfo.CurrentCulture)));
        }
    }
}
