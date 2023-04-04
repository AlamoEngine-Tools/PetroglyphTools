// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Test;
using PG.StarWarsGame.Files.MEG.Binary.File.Type.Definition.V1;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.File.Type.Definition.V1
{
    [TestClass]
    public class MegFileNameTableRecordTest
    {
        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("    ")]
        [DataRow("  \n\t  ")]
        [DataRow("  \r\n  \n  ")]
        [ExpectedException(typeof(ArgumentException))]
        public void Ctor_Test__ThrowsArgumentException(string fileName)
        {
            MegFileNameTableRecord _ = new MegFileNameTableRecord(fileName);
        }

        [TestMethod]
        [ExpectedException(typeof(OverflowException))]
        public void Ctor_Test__ThrowsOverflowException()
        {
            string fn = TestUtility.GetRandomStringOfLength(ushort.MaxValue + 5);
            MegFileNameTableRecord _ = new MegFileNameTableRecord(fn);
        }
    }
}
