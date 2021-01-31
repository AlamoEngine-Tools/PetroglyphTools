using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Util;

namespace PG.Commons.Test.Util
{
    [TestClass]
    [TestCategory(TestUtility.TEST_TYPE_UTILITY)]
    public class StringUtilityTest
    {
        [TestMethod]
        [DataRow(null, false)]
        [DataRow("", false)]
        [DataRow(" \t\n", false)]
        [DataRow("1234", true)]
        public void HasText(string s, bool expected)
        {
            Assert.AreEqual(expected, StringUtility.HasText(s));
        }

        [TestMethod]
        [DataRow("", ',', new string[] { })]
        [DataRow("  \n,a,b  ,\tc", ',', new string[] {"a", "b", "c"})]
        public void SplitClean(string s, char separator, [NotNull] string[] expected)
        {
            List<string> l = StringUtility.SplitClean(s, separator);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], l[i]);
            }
        }

        [TestMethod]
        [DataRow("", ',', new string[] { })]
        [DataRow("  \n,a,b  ,\tc", ',', new string[] {"a", "b", "c"})]
        public void ParseSeparatedStringToList(string s, char separator, [NotNull] string[] expected)
        {
            List<string> l = StringUtility.ParseSeparatedStringToList(s, separator);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], l[i]);
            }
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("\r\n\t  \r\n")]
        [DataRow(" ")]
        [ExpectedException(typeof(ArgumentException))]
        public void StripFileExtension_Test__ThrowsArgumentException(string invalidFilePath)
        {
            StringUtility.StripFileExtension(invalidFilePath);
        }
        
        [TestMethod]
        [DataRow("test", "test.xml")]
        [DataRow("c:/tester/test", "c:/tester/test.xml")]
        [DataRow("c:\\tester\\test", "c:\\tester\\test.xml")]
        public void StripFileExtension_Test__ReturnsExpected(string expected, string input)
        {
            string actual = StringUtility.StripFileExtension(input);
            Assert.AreEqual(expected, actual);
        }
    }
}
