// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Test;
using PG.StarWarsGame.Files.MEG.Binary.File.Builder.V1;
using PG.StarWarsGame.Files.MEG.Binary.File.Type.Definition.V1;
using PG.StarWarsGame.Files.MEG.Holder;
using PG.StarWarsGame.Files.MEG.Holder.V1;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.File.Builder
{
    [TestClass]
    public class MegFileBuilderTest
    {
        private IFileSystem m_fileSystem;

        [TestInitialize]
        public void SetUp()
        {
            m_fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    TestConstants.FILE_PATH_GAMEOBJECTFILES,
                    new MockFileData(TestConstants.CONTENT_GAMEOBJECTFILES)
                },
                {
                    TestConstants.FILE_PATH_CAMPAIGNFILES,
                    new MockFileData(TestConstants.CONTENT_CAMPAIGNFILES)
                },
                {
                    TestConstants.FILE_PATH_MEG_FILE,
                    new MockFileData(TestConstants.CONTENT_MEG_FILE)
                }
            });
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_BUILDER)]
        public void FromHolder_Test__FileHeaderIsConsistent()
        {
            MegFileHolder megFileHolder = new MegFileHolder(TestConstants.BASE_PATH, "test");
            megFileHolder.Content.Add(new MegFileDataEntry("data/xml/campaignfiles.xml",
                TestConstants.FILE_PATH_CAMPAIGNFILES));
            megFileHolder.Content.Add(new MegFileDataEntry("data/xml/gameobjectfiles.xml",
                TestConstants.FILE_PATH_GAMEOBJECTFILES));
            MegFileBuilder builder = new MegFileBuilder(m_fileSystem);
            MegFile megFile = builder.FromHolder(megFileHolder);
            Assert.IsNotNull(megFile);
            Assert.IsNotNull(megFile.Header);
            Assert.IsNotNull(megFile.FileContentTable);
            Assert.IsNotNull(megFile.FileNameTable);
            Assert.AreEqual(2u, megFile.Header.NumFiles);
            Assert.AreEqual(2u, megFile.Header.NumFileNames);
            Assert.AreEqual(40, megFile.FileContentTable.Size);
            Assert.AreEqual(58, megFile.FileNameTable.Size);
            Assert.IsTrue(megFile.FileNameTable[0].FileName
                .Equals("data/xml/gameobjectfiles.xml", StringComparison.InvariantCultureIgnoreCase));
            Assert.IsTrue(megFile.FileNameTable[1].FileName.Equals("data/xml/campaignfiles.xml",
                StringComparison.InvariantCultureIgnoreCase));
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_BUILDER)]
        public void FromHolder_Test__FileHeaderIsBinaryEquivalent()
        {
            MegFileHolder megFileHolder =
                new MegFileHolder(TestConstants.BASE_PATH, "FromHolder_Test__FileHeaderIsBinaryEquivalent");
            megFileHolder.Content.Add(new MegFileDataEntry("data/xml/campaignfiles.xml",
                TestConstants.FILE_PATH_CAMPAIGNFILES));
            megFileHolder.Content.Add(new MegFileDataEntry("data/xml/gameobjectfiles.xml",
                TestConstants.FILE_PATH_GAMEOBJECTFILES));
            MegFileBuilder builder = new MegFileBuilder(m_fileSystem);
            MegFile megFile = builder.FromHolder(megFileHolder);
            Assert.IsNotNull(megFile);
            m_fileSystem.File.WriteAllBytes(
                m_fileSystem.Path.Combine(TestConstants.BASE_PATH,
                    megFileHolder.FullyQualifiedName), megFile.ToBytes());
            byte[] readMegFile = m_fileSystem.File.ReadAllBytes(m_fileSystem.Path.Combine(TestConstants.BASE_PATH,
                megFileHolder.FullyQualifiedName));
            Assert.AreEqual(TestConstants.CONTENT_MEG_FILE_HEADER.Length, readMegFile.Length);
            for (int i = 0; i < TestConstants.CONTENT_MEG_FILE_HEADER.Length; i++)
            {
                Assert.AreEqual(TestConstants.CONTENT_MEG_FILE_HEADER[i], readMegFile[i]);
            }
        }
    }
}
