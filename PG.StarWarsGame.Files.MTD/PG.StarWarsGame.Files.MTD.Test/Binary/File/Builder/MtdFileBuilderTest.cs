// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MTD.Binary.File.Builder;
using PG.StarWarsGame.Files.MTD.Binary.File.Type.Definition;

namespace PG.StarWarsGame.Files.MTD.Test.Binary.File.Builder
{
    [TestClass]
    public class MtdFileBuilderTest
    {
        [TestMethod]
        [DataRow(null)]
        [DataRow(new byte[]
        {
            0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
        })]
        [DataRow(new byte[]
        {
            0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0,
        })]
        [ExpectedException(typeof(ArgumentException))]
        public void FromBytes_Test__ThrowsArgumentException(byte[] byteStream)
        {
            MtdFileBuilder builder = new MtdFileBuilder();
            MtdFile _ = builder.FromBytes(byteStream);
        }

        [TestMethod]
        public void FromBytes_Test__AsExpected()
        {
            MtdFileBuilder builder = new MtdFileBuilder();
            MtdFile file = builder.FromBytes(TestConstants.MtdFileTestConstants.METADATA_TO_BYTES);
        }
    }
}
