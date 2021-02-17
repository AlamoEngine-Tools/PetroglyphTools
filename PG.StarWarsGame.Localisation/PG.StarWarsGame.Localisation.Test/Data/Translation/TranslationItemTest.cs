// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Localisation.Data.Translation;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.Test.Data.Translation
{
    [TestClass]
    public class TranslationItemTest
    {
        [TestMethod]
        public void Ctor_Test__AfterCreationIsVersion1()
        {
            const string value01 = "My new value!";
            TranslationItem translationItem =
                new TranslationItem("My_Key", new GermanAlamoLanguageDefinition(), value01);
            Assert.AreEqual(1, translationItem.Version);
            Assert.IsTrue(string.Compare(value01, translationItem.Value, false,
                translationItem.Language.Culture) == 0);
        }

        [TestMethod]
        public void Version_Test__VersionUpdatesAsExpected()
        {
            const string value01 = "My new value!";
            const string value02 = "A different value!";
            TranslationItem translationItem =
                new TranslationItem("My_Key", new GermanAlamoLanguageDefinition(), value01);
            Assert.AreEqual(1, translationItem.Version);
            translationItem.Value = value01;
            Assert.AreEqual(1, translationItem.Version);
            translationItem.Value = value02;
            Assert.AreEqual(2, translationItem.Version);
            translationItem.Value = null;
            Assert.AreEqual(3, translationItem.Version);
            Assert.IsTrue(string.Compare(value01, translationItem.GetValueByVersion(1), false,
                translationItem.Language.Culture) == 0);
            Assert.IsTrue(string.Compare(value02, translationItem[2], false,
                translationItem.Language.Culture) == 0);
            Assert.IsFalse(string.Compare(value01, translationItem[3], false,
                translationItem.Language.Culture) == 0);
            Assert.IsFalse(string.Compare(value02, translationItem[3], false,
                translationItem.Language.Culture) == 0);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(0)]
        [DataRow(2)]
        [ExpectedException(typeof(ArgumentException))]
        public void GetValueByVersion_Test__ThrowsArgumentException(long v)
        {
            TranslationItem t = new TranslationItem(nameof(TranslationItem),
                new GermanAlamoLanguageDefinition(),
                nameof(GetValueByVersion_Test__ThrowsArgumentException));
            string _ = t.GetValueByVersion(v);
        }

        [TestMethod]
        public void IsTodoItem_Test__IsUpdatedAsExpected()
        {   const string key = "key";
            Tuple<string, bool>[] vars = {
                new Tuple<string, bool>(null, true)
                , new Tuple<string, bool>("value", false)
                , new Tuple<string, bool>("value2", false)
                , new Tuple<string, bool>(null, true)
                , new Tuple<string, bool>(null, true)
            };
            TranslationItem item = new TranslationItem(key, new GermanAlamoLanguageDefinition());
            Assert.IsTrue(item.IsTodoItem);
            long initialVersion = item.Version;
            foreach ((string value, bool expected) in vars)
            {
                item.Value = value;
                initialVersion++;
                Assert.AreEqual(initialVersion, item.Version);
                Assert.AreEqual(expected, item.IsTodoItem);
            }
        }

        [TestMethod]
        public void TryGetValueByVersion_Test__GetValueAsExpected()
        {
            const string key = "key";
            string[] values = {"My new value!", "A different value!", "A third value!"};
            TranslationItem item = new TranslationItem(key, new GermanAlamoLanguageDefinition());

            foreach (string t in values)
            {
                item.Value = t;
            }

            for (int i = 1; i <= item.Version; i++)
            {
                Assert.IsTrue(item.TryGetValueByVersion(i, out string v));
                Assert.IsNotNull(v);
            }

            Assert.IsFalse(item.TryGetValueByVersion(item.Version + 1, out string _));
        }
    }
}
