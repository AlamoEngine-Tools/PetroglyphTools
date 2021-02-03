using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PG.Commons.Test
{
    public static class TestUtility
    {
        public const string TEST_TYPE_API = "Public API Test";
        public const string TEST_TYPE_HOLY = "Holy Test";
        public const string TEST_TYPE_BUILDER = "Builder Test";
        public const string TEST_TYPE_UTILITY = "Utility Test";

        public static bool IsWindows()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }

        public static bool IsUnix()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        }
        
        public static void AssertAreBinaryEquivalent(IReadOnlyList<byte> expected, IReadOnlyList<byte> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }
    }
}
