// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Core.Data.Key;
using System;
using System.Collections.Generic;

namespace PG.Core.Test.Data.Key
{
    [TestClass]
    [TestCategory(TestConstants.TEST_TYPE_HOLY)]
    public class AbstractKeyTest
    {
        private class IntegerTestKey : AbstractIntegerKey
        {
            public IntegerTestKey(int key) : base(key)
            {
            }
        }

        private class StringTestKey : AbstractStringKey
        {
            public StringTestKey(string key) : base(key)
            {
            }
        }

        private static IEnumerable<Tuple<IKey, IKey, bool>> GetEqualsKeyTestMap()
        {
            List<Tuple<IKey, IKey, bool>> l = new()
            {
                new Tuple<IKey, IKey, bool>(new IntegerTestKey(0), new IntegerTestKey(0), true),
                new Tuple<IKey, IKey, bool>(new IntegerTestKey(1), new IntegerTestKey(0), false),
                new Tuple<IKey, IKey, bool>(new IntegerTestKey(0), new IntegerTestKey(1), false),
                new Tuple<IKey, IKey, bool>(new IntegerTestKey(0), new StringTestKey("0"), false),
                new Tuple<IKey, IKey, bool>(new StringTestKey("0"), new IntegerTestKey(0), false),
                new Tuple<IKey, IKey, bool>(new StringTestKey("ABC"), new StringTestKey("ABC"), true),
                new Tuple<IKey, IKey, bool>(new StringTestKey("abC"), new StringTestKey("aBC"), false)
            };
            return l;
        }

        [TestMethod]
        public void Test_Equals__AsExpected()
        {
            foreach ((IKey key0, IKey key1, bool expectedComparison) in GetEqualsKeyTestMap())
            {
                Assert.IsNotNull(key0);
                Assert.IsNotNull(key1);
                Assert.AreEqual(expectedComparison, key0.Equals(key1));
                Assert.AreEqual(expectedComparison, key0.Equals(key1));
                Assert.AreEqual(expectedComparison, key1.Equals(key0));
            }
        }
    }
}
