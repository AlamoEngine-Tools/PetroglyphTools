// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Util;
using PG.Core.Test;

namespace PG.Commons.Test.Util
{
    [TestClass]
    [TestCategory(TestConstants.TEST_TYPE_UTILITY)]
    public class HtmlUtilityTest
    {
        [TestMethod]
        [DataRow(null, false)]
        [DataRow("", false)]
        [DataRow("\t  \n", false)]
        [DataRow("\t https://www.youtube.com/watch?v=wwLW6CjswxM\n", false)]
        [DataRow(" https://github.com/AlamoEngine-Tools", false)]
        [DataRow("https://www.youtube.com/watch?v=wwLW6CjswxM", true)]
        [DataRow("https://github.com/AlamoEngine-Tools", true)]
        public void IsValidUri(string uri, bool expected)
        {
            Assert.AreEqual(expected, HtmlUtility.IsValidUri(uri));
        }
    }
}
