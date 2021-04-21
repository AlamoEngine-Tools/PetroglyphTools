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
using PG.Core.Test;
using PG.Core.Test.Services;
using PG.StarWarsGame.Files.MEG.Commons.Exceptions;
using PG.StarWarsGame.Files.MEG.Holder;
using PG.StarWarsGame.Files.MEG.Holder.V1;
using PG.StarWarsGame.Files.MEG.Services;
using PG.StarWarsGame.Files.MEG.Services.V1;

namespace PG.StarWarsGame.Files.MEG.Test.Services.V1
{
    [TestClass]
    public class MegFileProcessServiceTest : AbstractServiceTest<MegFileProcessService>
    {
        private IFileSystem m_fileSystem;
        
        public override MegFileProcessService GetServiceInstance()
        {
            return new MegFileProcessService(m_fileSystem);
        }

        [TestInitialize]
        public void SetUp()
        {
            m_fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    MegTestConstants.FILE_PATH_GAMEOBJECTFILES,
                    new MockFileData(MegTestConstants.CONTENT_GAMEOBJECTFILES)
                },
                {
                    MegTestConstants.FILE_PATH_CAMPAIGNFILES,
                    new MockFileData(MegTestConstants.CONTENT_CAMPAIGNFILES)
                },
                {
                    MegTestConstants.FILE_PATH_MEG_FILE,
                    new MockFileData(MegTestConstants.CONTENT_MEG_FILE)
                }
            });
        }

        [TestMethod]
        [TestCategory(TestConstants.TEST_TYPE_HOLY)]
        [TestCategory(TestConstants.TEST_TYPE_API)]
        public void PackFilesAsMegArchive_Test__CreatedMegFileIsBinaryEquivalent()
        {
            string targetDirectory = m_fileSystem.Path.Combine(MegTestConstants.BASE_PATH,
                nameof(PackFilesAsMegArchive_Test__CreatedMegFileIsBinaryEquivalent));
            const string testMegFileName = "test_meg_file.meg";
            IMegFileProcessService svc = GetServiceInstance();
            svc.PackFilesAsMegArchive(testMegFileName,
                new Dictionary<string, string>
                {
                    {
                        $"DATA/XML/{MegTestConstants.FILE_NAME_CAMPAIGNFILES.ToUpper()}",
                        MegTestConstants.FILE_PATH_CAMPAIGNFILES
                    },
                    {
                        $"DATA/XML/{MegTestConstants.FILE_NAME_GAMEOBJECTFILES.ToUpper()}",
                        MegTestConstants.FILE_PATH_GAMEOBJECTFILES
                    }
                },
                targetDirectory);
            byte[] expected = m_fileSystem.File.ReadAllBytes(MegTestConstants.FILE_PATH_MEG_FILE);
            byte[] actual = m_fileSystem.File.ReadAllBytes(m_fileSystem.Path.Combine(targetDirectory, testMegFileName));
            TestUtility.AssertAreBinaryEquivalent(expected, actual);
        }

        [TestMethod]
        [TestCategory(TestConstants.TEST_TYPE_HOLY)]
        [TestCategory(TestConstants.TEST_TYPE_API)]
        [DataRow(null, null)]
        [DataRow("", null)]
        [DataRow("    ", null)]
        [DataRow("test", null)]
        [DataRow("test", "")]
        [DataRow("test", "    ")]
        [ExpectedException(typeof(ArgumentException))]
        public void PackFilesAsMegArchive_Test__ThrowsArgumentExceptionForStringTypes(string megArchiveName,
            string targetDirectory)
        {
            IMegFileProcessService svc = GetServiceInstance();
            svc.PackFilesAsMegArchive(megArchiveName,
                new Dictionary<string, string>
                {
                    {
                        $"DATA/XML/{MegTestConstants.FILE_NAME_CAMPAIGNFILES.ToUpper()}",
                        MegTestConstants.FILE_PATH_CAMPAIGNFILES
                    },
                    {
                        $"DATA/XML/{MegTestConstants.FILE_NAME_GAMEOBJECTFILES.ToUpper()}",
                        MegTestConstants.FILE_PATH_GAMEOBJECTFILES
                    }
                },
                targetDirectory);
        }

        [TestMethod]
        [TestCategory(TestConstants.TEST_TYPE_HOLY)]
        [TestCategory(TestConstants.TEST_TYPE_API)]
        [ExpectedException(typeof(ArgumentException))]
        public void PackFilesAsMegArchive_Test__ThrowsArgumentException()
        {
            IMegFileProcessService svc = GetServiceInstance();
            svc.PackFilesAsMegArchive(nameof(PackFilesAsMegArchive_Test__ThrowsArgumentException),
                new Dictionary<string, string>(),
                m_fileSystem.Path.Combine(MegTestConstants.BASE_PATH, nameof(PackFilesAsMegArchive_Test__ThrowsArgumentException)));
        }

        [TestMethod]
        [TestCategory(TestConstants.TEST_TYPE_HOLY)]
        [TestCategory(TestConstants.TEST_TYPE_API)]
        [ExpectedException(typeof(ArgumentNullException))]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("\r\n")]
        [DataRow("\t    \r\n")]
        public void Load_Test__GivenInvalidFilePathThrowsArgumentNullException(string invalidValue)
        {
            Assert.IsFalse(StringUtility.HasText(invalidValue));
            IMegFileProcessService svc = GetServiceInstance();
            svc.Load(invalidValue);
        }

        [TestMethod]
        [TestCategory(TestConstants.TEST_TYPE_HOLY)]
        [TestCategory(TestConstants.TEST_TYPE_API)]
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

            IMegFileProcessService svc = GetServiceInstance();
            svc.Load(invalidPath);
        }

        [TestMethod]
        [TestCategory(TestConstants.TEST_TYPE_HOLY)]
        [TestCategory(TestConstants.TEST_TYPE_API)]
        public void Load_Test__MegFileHolderIntegrity()
        {
            IMegFileProcessService svc = GetServiceInstance();
            MegFileHolder megFileHolder = svc.Load(MegTestConstants.FILE_PATH_MEG_FILE);
            Assert.IsTrue(StringUtility.HasText(megFileHolder.FileName));
            string expectedFileName = MegTestConstants.FILE_NAME_MEG_FILE.Replace(".meg", string.Empty);
            string expectedFilePath =
                MegTestConstants.FILE_PATH_MEG_FILE.Replace(
                    (TestUtility.IsWindows() ? "\\" : "/") + MegTestConstants.FILE_NAME_MEG_FILE, string.Empty);
            Assert.IsTrue(expectedFileName.Equals(megFileHolder.FileName, StringComparison.InvariantCultureIgnoreCase));
            Assert.IsTrue(expectedFilePath.Equals(megFileHolder.FilePath, StringComparison.InvariantCultureIgnoreCase));
            Assert.AreEqual(2, megFileHolder.Content.Count);
            const string expectedBasePath = "DATA/XML/";
            foreach (MegFileDataEntry megFileDataEntry in megFileHolder.Content)
            {
                if (megFileDataEntry.RelativeFilePath.Equals(
                    expectedBasePath + MegTestConstants.FILE_NAME_GAMEOBJECTFILES,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    uint expectedFileSize =
                        (uint) m_fileSystem.File.ReadAllBytes(MegTestConstants.FILE_PATH_GAMEOBJECTFILES).Length;
                    Assert.AreEqual(expectedFileSize, megFileDataEntry.Size);
                }

                if (megFileDataEntry.RelativeFilePath.Equals(
                    expectedBasePath + MegTestConstants.FILE_NAME_CAMPAIGNFILES,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    uint expectedFileSize =
                        (uint) m_fileSystem.File.ReadAllBytes(MegTestConstants.FILE_PATH_CAMPAIGNFILES).Length;
                    Assert.AreEqual(expectedFileSize, megFileDataEntry.Size);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestConstants.TEST_TYPE_HOLY)]
        [TestCategory(TestConstants.TEST_TYPE_API)]
        public void UnpackMegFile_Test__UnpackedFilesAreBinaryEquivalent()
        {
            string exportTestPath =
                m_fileSystem.Path.Combine(MegTestConstants.BASE_PATH,
                    nameof(UnpackMegFile_Test__UnpackedFilesAreBinaryEquivalent));
            IMegFileProcessService svc = GetServiceInstance();
            svc.UnpackMegFile(MegTestConstants.FILE_PATH_MEG_FILE, exportTestPath);
            Assert.IsTrue(m_fileSystem.Directory.Exists(exportTestPath));
            string fullExportPath = m_fileSystem.Path.Combine(exportTestPath, "DATA", "XML");
            Assert.IsTrue(m_fileSystem.Directory.Exists(fullExportPath));
            string[] files = m_fileSystem.Directory.GetFiles(fullExportPath);
            Assert.IsTrue(files.Length == 2);
            Assert.IsTrue(files.Contains(m_fileSystem.Path.Combine(fullExportPath,
                MegTestConstants.FILE_NAME_CAMPAIGNFILES.ToUpper())));
            Assert.IsTrue(files.Contains(m_fileSystem.Path.Combine(fullExportPath,
                MegTestConstants.FILE_NAME_GAMEOBJECTFILES.ToUpper())));
            byte[] expected = m_fileSystem.File.ReadAllBytes(MegTestConstants.FILE_PATH_GAMEOBJECTFILES);
            byte[] actual = m_fileSystem.File.ReadAllBytes(m_fileSystem.Path.Combine(fullExportPath,
                MegTestConstants.FILE_NAME_GAMEOBJECTFILES.ToUpper()));
            TestUtility.AssertAreBinaryEquivalent(expected, actual);
            expected = m_fileSystem.File.ReadAllBytes(MegTestConstants.FILE_PATH_GAMEOBJECTFILES);
            actual = m_fileSystem.File.ReadAllBytes(m_fileSystem.Path.Combine(fullExportPath,
                MegTestConstants.FILE_NAME_GAMEOBJECTFILES.ToUpper()));
            TestUtility.AssertAreBinaryEquivalent(expected, actual);
        }

        [TestMethod]
        [TestCategory(TestConstants.TEST_TYPE_HOLY)]
        [TestCategory(TestConstants.TEST_TYPE_API)]
        [DataRow("", MegTestConstants.FILE_NAME_GAMEOBJECTFILES)]
        [DataRow(@"c:\mod\data\xml\gameobjectfiles.xml", "")]
        [ExpectedException(typeof(ArgumentException))]
        public void UnpackSingleFileFromMegFile_Test__ThrowsArgumentException(string targetDirectory, string fileName)
        {
            IMegFileProcessService svc = GetServiceInstance();
            MegFileHolder megFileHolder = svc.Load(MegTestConstants.FILE_PATH_MEG_FILE);
            svc.UnpackSingleFileFromMegFile(megFileHolder, targetDirectory, fileName);
        }

        [TestMethod]
        [TestCategory(TestConstants.TEST_TYPE_HOLY)]
        [TestCategory(TestConstants.TEST_TYPE_API)]
        [ExpectedException(typeof(FileNotContainedInArchiveException))]
        public void UnpackSingleFileFromMegFile_Test__ThrowsFileNotContainedInArchiveException()
        {
            string exportTestPath =
                m_fileSystem.Path.Combine(MegTestConstants.BASE_PATH,
                    "UnpackSingleFileFromMegFile_Test__ThrowsFileNotContainedInArchiveException");
            IMegFileProcessService svc = GetServiceInstance();
            MegFileHolder megFileHolder = svc.Load(MegTestConstants.FILE_PATH_MEG_FILE);
            svc.UnpackSingleFileFromMegFile(megFileHolder, exportTestPath, "I_DO_NO_EXIST.XML", false);
        }
        
        [TestMethod]
        [TestCategory(TestConstants.TEST_TYPE_HOLY)]
        [TestCategory(TestConstants.TEST_TYPE_API)]
        [ExpectedException(typeof(MultipleFilesWithMatchingNameInArchiveException))]
        public void UnpackSingleFileFromMegFile_Test__ThrowsMultipleFilesWithMatchingNameInArchiveException()
        {
            string exportTestPath =
                m_fileSystem.Path.Combine(MegTestConstants.BASE_PATH,
                    "UnpackSingleFileFromMegFile_Test__ThrowsFileNotContainedInArchiveException");
            IMegFileProcessService svc = GetServiceInstance();
            MegFileHolder megFileHolder = svc.Load(MegTestConstants.FILE_PATH_MEG_FILE);
            svc.UnpackSingleFileFromMegFile(megFileHolder, exportTestPath, "XML", false);
        }

        [TestMethod]
        [TestCategory(TestConstants.TEST_TYPE_HOLY)]
        [TestCategory(TestConstants.TEST_TYPE_API)]
        public void UnpackSingleFileFromMegFile_Test__DirectoryHierarchyPreservedUnpackedFileIsBinaryEquivalent()
        {
            string exportTestPath =
                m_fileSystem.Path.Combine(MegTestConstants.BASE_PATH,
                    "UnpackSingleFileFromMegFile_Test__UnpackedFileIsBinaryEquivalent");
            string fullExportPath = m_fileSystem.Path.Combine(exportTestPath, "DATA", "XML");
            IMegFileProcessService svc = GetServiceInstance();
            MegFileHolder megFileHolder = svc.Load(MegTestConstants.FILE_PATH_MEG_FILE);
            svc.UnpackSingleFileFromMegFile(megFileHolder, exportTestPath, MegTestConstants.FILE_NAME_GAMEOBJECTFILES);
            byte[] expected = m_fileSystem.File.ReadAllBytes(MegTestConstants.FILE_PATH_GAMEOBJECTFILES);
            byte[] actual = m_fileSystem.File.ReadAllBytes(m_fileSystem.Path.Combine(fullExportPath,
                MegTestConstants.FILE_NAME_GAMEOBJECTFILES.ToUpper()));
            TestUtility.AssertAreBinaryEquivalent(expected, actual);
        }

        [TestMethod]
        [TestCategory(TestConstants.TEST_TYPE_HOLY)]
        [TestCategory(TestConstants.TEST_TYPE_API)]
        public void UnpackSingleFileFromMegFile_Test__DirectoryHierarchyFlatUnpackedFileIsBinaryEquivalent()
        {
            string exportTestPath =
                m_fileSystem.Path.Combine(MegTestConstants.BASE_PATH,
                    "UnpackSingleFileFromMegFile_Test__UnpackedFileIsBinaryEquivalent");
            IMegFileProcessService svc = GetServiceInstance();
            MegFileHolder megFileHolder = svc.Load(MegTestConstants.FILE_PATH_MEG_FILE);
            svc.UnpackSingleFileFromMegFile(megFileHolder, exportTestPath, MegTestConstants.FILE_NAME_GAMEOBJECTFILES,
                false);
            byte[] expected = m_fileSystem.File.ReadAllBytes(MegTestConstants.FILE_PATH_GAMEOBJECTFILES);
            byte[] actual = m_fileSystem.File.ReadAllBytes(m_fileSystem.Path.Combine(exportTestPath,
                MegTestConstants.FILE_NAME_GAMEOBJECTFILES.ToUpper()));
            TestUtility.AssertAreBinaryEquivalent(expected, actual);
        }
    }
}
