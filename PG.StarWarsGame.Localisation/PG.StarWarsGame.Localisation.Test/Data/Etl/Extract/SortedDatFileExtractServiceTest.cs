// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Test;
using PG.StarWarsGame.Files.DAT.Holder;
using PG.StarWarsGame.Files.DAT.Services;
using PG.StarWarsGame.Localisation.Data.Etl.Extract;

namespace PG.StarWarsGame.Localisation.Test.Data.Etl.Extract
{
    [TestClass]
    [TestCategory(TestUtility.TEST_TYPE_ETL)]
    public class SortedDatFileExtractServiceTest
    {
        private static SortedDatFileHolder s_sortedDatFileHolder;
        private const string FILE_NAME = "mastertextfile_english.dat";

        [TestInitialize]
        public void TestInitialize()
        {
            s_sortedDatFileHolder = new SortedDatFileHolder("test", "test");
            s_sortedDatFileHolder.Content.Add(new Tuple<string, string>("KEY_00", "Value 00"));
            s_sortedDatFileHolder.Content.Add(new Tuple<string, string>("KEY_01", "Value 01"));
            s_sortedDatFileHolder.Content.Add(new Tuple<string, string>("KEY_02", "Value 02"));
            s_sortedDatFileHolder.Content.Add(new Tuple<string, string>("KEY_03", "Value 03"));
            s_sortedDatFileHolder.Content.Add(new Tuple<string, string>("KEY_04", "Value 04"));
            s_sortedDatFileHolder.Content.Add(new Tuple<string, string>("KEY_05", "Value 05"));
        }

        private class MockSortedDatFileProcessService : ISortedDatFileProcessService
        {
            public SortedDatFileHolder LoadFromFile(string filePath)
            {
                return s_sortedDatFileHolder;
            }

            public void SaveToFile(SortedDatFileHolder sortedDatFileHolder)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_Test__ThrowsArgumentNullException()
        {
            SortedDatFileExtractService _ = new SortedDatFileExtractService(null, null, "test");
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("\r\n\t  \r\n")]
        [DataRow(" ")]
        [ExpectedException(typeof(ArgumentException))]
        public void Ctor_Test__ThrowsArgumentException(string stage0FilePath)
        {
            SortedDatFileExtractService _ =
                new SortedDatFileExtractService(null, new MockSortedDatFileProcessService(), stage0FilePath);
        }

        [TestMethod]
        public void Extract_Test()
        {
            SortedDatFileExtractService svc =
                new SortedDatFileExtractService(null, new MockSortedDatFileProcessService(), $"../test/{FILE_NAME}");
            svc.Extract();
            Assert.AreEqual(s_sortedDatFileHolder.Content.Count, svc.Stage1Beans.Count);
            foreach ((string key, string value) in s_sortedDatFileHolder.Content)
            {
                bool isContained = false;
                foreach (SortedTranslationStage1Bean bean in svc.Stage1Beans)
                {
                    if (key.Equals(bean.Key, StringComparison.InvariantCultureIgnoreCase) &&
                        value.Equals(bean.Value, StringComparison.InvariantCultureIgnoreCase) &&
                        FILE_NAME.Equals(bean.OriginFileName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        isContained = true;
                    }
                }

                Assert.IsTrue(isContained);
            }
        }
    }
}
