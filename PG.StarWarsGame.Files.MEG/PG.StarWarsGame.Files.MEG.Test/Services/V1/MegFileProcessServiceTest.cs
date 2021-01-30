// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Test;
using PG.Commons.Util;
using PG.StarWarsGame.Files.MEG.Holder;
using PG.StarWarsGame.Files.MEG.Holder.V1;
using PG.StarWarsGame.Files.MEG.Services;
using PG.StarWarsGame.Files.MEG.Services.V1;

namespace PG.StarWarsGame.Files.MEG.Test.Services.V1
{
    [TestClass]
    public class MegFileProcessServiceTest
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
        [TestCategory(TestUtility.TEST_TYPE_HOLY)]
        [TestCategory(TestUtility.TEST_TYPE_API)]
        [ExpectedException(typeof(NotImplementedException))]
        public void PackFilesAsMegArchive_Test__()
        {
            IMegFileProcessService svc = new MegFileProcessService(m_fileSystem);
            svc.PackFilesAsMegArchive("", "", new List<string>(), "");
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_HOLY)]
        [TestCategory(TestUtility.TEST_TYPE_API)]
        [ExpectedException(typeof(ArgumentNullException))]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("\r\n")]
        [DataRow("\t    \r\n")]
        public void Load_Test__GivenInvalidFilePathThrowsArgumentNullException(string invalidValue)
        {
            IMegFileProcessService svc = new MegFileProcessService(m_fileSystem);
            svc.Load(invalidValue);
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_HOLY)]
        [TestCategory(TestUtility.TEST_TYPE_API)]
        [ExpectedException(typeof(FileNotFoundException))]
        [DataRow("test/data/path.meg")]
        [DataRow("test/data/")]
        public void Load_Test__GivenInvalidFilePathThrowsFileNotFoundException(string invalidPath)
        {
            Assert.IsTrue(StringUtility.HasText(invalidPath));
            if (TestUtility.IsWindows())
            {
                invalidPath = "c:/" + invalidPath;
                invalidPath = invalidPath.Replace('/', '\\');
            }
            else
            {
                invalidPath = "mnt/c/" + invalidPath;
            }

            IMegFileProcessService svc = new MegFileProcessService(m_fileSystem);
            svc.Load(invalidPath);
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_HOLY)]
        [TestCategory(TestUtility.TEST_TYPE_API)]
        public void Load_Test__MegFileHolderIntegrity()
        {
            IMegFileProcessService svc = new MegFileProcessService(m_fileSystem);
            MegFileHolder megFileHolder = svc.Load(TestConstants.FILE_PATH_MEG_FILE);
            Assert.IsTrue(StringUtility.HasText(megFileHolder.FileName));
            string expectedFileName = TestConstants.FILE_NAME_MEG_FILE.Replace(".meg", string.Empty);
            string expectedFilePath =
                TestConstants.FILE_PATH_MEG_FILE.Replace(
                    (TestUtility.IsWindows() ? "\\" : "/") + TestConstants.FILE_NAME_MEG_FILE, string.Empty);
            Assert.IsTrue(expectedFileName.Equals(megFileHolder.FileName, StringComparison.InvariantCultureIgnoreCase));
            Assert.IsTrue(expectedFilePath.Equals(megFileHolder.FilePath, StringComparison.InvariantCultureIgnoreCase));
            Assert.AreEqual(2, megFileHolder.Content.Count);
            const string expectedBasePath = "DATA/XML/";
            foreach (MegFileDataEntry megFileDataEntry in megFileHolder.Content)
            {
                if (megFileDataEntry.RelativeFilePath.Equals(
                    expectedBasePath + TestConstants.FILE_NAME_GAMEOBJECTFILES,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    uint expectedFileSize =
                        (uint) m_fileSystem.File.ReadAllBytes(TestConstants.FILE_PATH_GAMEOBJECTFILES).Length;
                    Assert.AreEqual(expectedFileSize, megFileDataEntry.Size);
                }

                if (megFileDataEntry.RelativeFilePath.Equals(
                    expectedBasePath + TestConstants.FILE_NAME_CAMPAIGNFILES,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    uint expectedFileSize =
                        (uint) m_fileSystem.File.ReadAllBytes(TestConstants.FILE_PATH_CAMPAIGNFILES).Length;
                    Assert.AreEqual(expectedFileSize, megFileDataEntry.Size);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestUtility.TEST_TYPE_HOLY)]
        [TestCategory(TestUtility.TEST_TYPE_API)]
        public void UnpackMegFile_Test__UnpackedFilesAreBinaryEquivalent()
        {
            string exportTestPath =
                m_fileSystem.Path.Combine(TestConstants.BASE_PATH, "UnpackMegFile_Test__FromFilePath");
            IMegFileProcessService svc = new MegFileProcessService(m_fileSystem);
            svc.UnpackMegFile(TestConstants.FILE_PATH_MEG_FILE, exportTestPath);
            Assert.IsTrue(m_fileSystem.Directory.Exists(exportTestPath));
            string fullExportPath = m_fileSystem.Path.Combine(exportTestPath, "DATA", "XML");
            Assert.IsTrue(m_fileSystem.Directory.Exists(fullExportPath));
            string[] files = m_fileSystem.Directory.GetFiles(fullExportPath);
            Assert.IsTrue(files.Length == 2);
            Assert.IsTrue(files.Contains(m_fileSystem.Path.Combine(fullExportPath,
                TestConstants.FILE_NAME_CAMPAIGNFILES.ToUpper())));
            Assert.IsTrue(files.Contains(m_fileSystem.Path.Combine(fullExportPath,
                TestConstants.FILE_NAME_GAMEOBJECTFILES.ToUpper())));
            byte[] expected = m_fileSystem.File.ReadAllBytes(TestConstants.FILE_PATH_GAMEOBJECTFILES);
            byte[] actual = m_fileSystem.File.ReadAllBytes(m_fileSystem.Path.Combine(fullExportPath,
                TestConstants.FILE_NAME_GAMEOBJECTFILES.ToUpper()));
            AssertAreBinaryEquivalent(expected, actual);
            expected = m_fileSystem.File.ReadAllBytes(TestConstants.FILE_PATH_GAMEOBJECTFILES);
            actual = m_fileSystem.File.ReadAllBytes(m_fileSystem.Path.Combine(fullExportPath,
                TestConstants.FILE_NAME_GAMEOBJECTFILES.ToUpper()));
            AssertAreBinaryEquivalent(expected, actual);
        }

        private static void AssertAreBinaryEquivalent(IReadOnlyList<byte> expected, IReadOnlyList<byte> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }
    }
}
