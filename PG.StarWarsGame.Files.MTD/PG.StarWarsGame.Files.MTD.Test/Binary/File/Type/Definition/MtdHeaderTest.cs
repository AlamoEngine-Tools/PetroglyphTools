// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Test;
using PG.StarWarsGame.Files.MTD.Binary.File.Type.Definition;
using System;

namespace PG.StarWarsGame.Files.MTD.Test.Binary.File.Type.Definition
{
    [TestClass]
    public class MtdHeaderTest
    {
        private const int EXPECTED_SIZE = sizeof(uint);

        [TestMethod]
        [DataRow(0u)]
        [DataRow(12345u)]
        [DataRow(uint.MaxValue)]
        [Ignore]
        public void ToBytes_Test__AreBinaryEquivalent(uint input)
        {
            MtdHeader header = new MtdHeader(input);
            byte[] actual = header.ToBytes();
            TestUtility.AssertAreBinaryEquivalent(BitConverter.GetBytes(input), actual);
        }

        [TestMethod]
        public void Size_Test_AsExpected()
        {
            Assert.AreEqual(EXPECTED_SIZE, new MtdHeader().Size);
        }
    }
}
