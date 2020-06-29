using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Util;

namespace PG.Commons.Test.Util
{
    [TestClass]
    [TestCategory(TestUtility.TEST_TYPE_UTILITY)]
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
