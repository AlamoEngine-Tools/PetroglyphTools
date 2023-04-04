// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Util;
using PG.Core.Test;
using System;

namespace PG.Commons.Test.Util
{
    [TestClass]
    [TestCategory(TestConstants.TEST_TYPE_UTILITY)]
    public class BooleanUtilityTest
    {
        [TestMethod]
        [DataRow(null, false)]
        [DataRow("", false)]
        [DataRow("Some String", false)]
        [DataRow("true", true)]
        [DataRow("yes", true)]
        [DataRow("false", false)]
        [DataRow("no", false)]
        [DataRow("\ttrue  ", true)]
        [DataRow(" yes ", true)]
        [DataRow("\t\n\tfalse\t\n", false)]
        [DataRow("no\n", false)]
        public void Parse_StringToBoolean(string s, bool expected)
        {
            Assert.AreEqual(expected, BooleanUtility.Parse(s));
        }

        [TestMethod]
        [DataRow(true, BooleanUtility.BoolParseType.YesNo, "Yes")]
        [DataRow(false, BooleanUtility.BoolParseType.YesNo, "No")]
        [DataRow(true, BooleanUtility.BoolParseType.TrueFalse, "True")]
        [DataRow(false, BooleanUtility.BoolParseType.TrueFalse, "False")]
        public void Parse_BooleanToString(bool b, BooleanUtility.BoolParseType t, string expected)
        {
            Assert.IsNotNull(expected);
            Assert.IsTrue(expected.Equals(BooleanUtility.Parse(b, t), StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
