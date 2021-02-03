// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Test;
using PG.StarWarsGame.Files.DAT.Binary.File.Builder;
using PG.StarWarsGame.Files.DAT.Binary.File.Type.Definition;
using PG.StarWarsGame.Files.DAT.Holder;

namespace PG.StarWarsGame.Files.DAT.Test.Binary.File.Builder
{
    [TestClass]
    public class SortedDatFileBuilderTest
    {
        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_HOLY)]
        [TestCategory(TestUtility.TEST_TYPE_BUILDER)]
        public void FromHolder()
        {
            List<Tuple<string, string>> translations = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("KEY_00", "VALUE"),
                new Tuple<string, string>("KEY_01", "VALUE"),
                new Tuple<string, string>("KEY_02", "VALUE"),
                new Tuple<string, string>("KEY_03", "VALUE"),
                new Tuple<string, string>("KEY_04", "VALUE")
            };
            SortedDatFileHolder sortedDatFileHolder = new SortedDatFileHolder("", "") {Content = translations};
            SortedDatFileBuilder builder = new SortedDatFileBuilder();
            DatFile file = builder.FromHolder(sortedDatFileHolder);
            Assert.IsNotNull(file);
            Assert.AreEqual((uint) translations.Count, file.KeyValuePairCount);
            List<string> expectedOrder = new List<string>
            {
                "KEY_01",
                "KEY_04",
                "KEY_00",
                "KEY_02",
                "KEY_03",
            };
            for (int i = 0; i < expectedOrder.Count; i++)
            {
                Debug.Assert(file.Keys != null, "file.Keys != null");
                Debug.Assert(file.Keys[i] != null, $"file.Keys[{i}] != null");
                Assert.AreEqual(expectedOrder[i], file.Keys[i].Key);
            }
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_HOLY)]
        [TestCategory(TestUtility.TEST_TYPE_BUILDER)]
        public void FromBytes()
        {
            SortedDatFileHolder sortedDatFileHolder = new SortedDatFileHolder("", "")
            {
                Content = new List<Tuple<string, string>>
                {
                    new Tuple<string, string>("KEY_00", "VALUE"),
                    new Tuple<string, string>("KEY_01", "VALUE"),
                    new Tuple<string, string>("KEY_02", "VALUE"),
                    new Tuple<string, string>("KEY_03", "VALUE"),
                    new Tuple<string, string>("KEY_04", "VALUE")
                }
            };
            SortedDatFileBuilder builder = new SortedDatFileBuilder();
            DatFile file = builder.FromHolder(sortedDatFileHolder);
            byte[] bytes = file.ToBytes();
            DatFile fileFromBytes = builder.FromBytes(bytes);
            Assert.AreEqual(file, fileFromBytes);
        }
    }
}
