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
using PG.Core.Test.Attributes;
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
        [TestMethod]
        [TestCategory(TestConstants.TEST_TYPE_HOLY)]
        [TestCategory(TestConstants.TEST_TYPE_API)]
        public void PackFilesAsMegArchive_Test__CreatedMegFileIsBinaryEquivalent()
        {
            IFileSystem fileSystem = GetFileSystemInternal();
            string targetDirectory = fileSystem.Path.Combine(MegTestConstants.GetBasePath(),
                nameof(PackFilesAsMegArchive_Test__CreatedMegFileIsBinaryEquivalent));
            const string testMegFileName = "test_meg_file.meg";
            IMegFileProcessService svc = GetServiceInstance(fileSystem);
            svc.PackFilesAsMegArchive(testMegFileName,
                new Dictionary<string, string>
                {
                    {
                        $"DATA/XML/{MegTestConstants.FILE_NAME_CAMPAIGNFILES.ToUpper()}",
                        MegTestConstants.GetCampaignFilesPath()
                    },
                    {
                        $"DATA/XML/{MegTestConstants.FILE_NAME_GAMEOBJECTFILES.ToUpper()}",
                        MegTestConstants.GetGameObjectFilesPath()
                    }
                },
                targetDirectory);
            byte[] expected = fileSystem.File.ReadAllBytes(MegTestConstants.GetMegFilePath());
            byte[] actual = fileSystem.File.ReadAllBytes(fileSystem.Path.Combine(targetDirectory, testMegFileName));
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
            IFileSystem fileSystem = GetFileSystemInternal();
            IMegFileProcessService svc = GetServiceInstance(fileSystem);
            svc.PackFilesAsMegArchive(megArchiveName,
                new Dictionary<string, string>
                {
                    {
                        $"DATA/XML/{MegTestConstants.FILE_NAME_CAMPAIGNFILES.ToUpper()}",
                        MegTestConstants.GetCampaignFilesPath()
                    },
                    {
                        $"DATA/XML/{MegTestConstants.FILE_NAME_GAMEOBJECTFILES.ToUpper()}",
                        MegTestConstants.GetGameObjectFilesPath()
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
            IFileSystem fileSystem = GetFileSystemInternal();
            IMegFileProcessService svc = GetServiceInstance(fileSystem);
            svc.PackFilesAsMegArchive(nameof(PackFilesAsMegArchive_Test__ThrowsArgumentException),
                new Dictionary<string, string>(),
                fileSystem.Path.Combine(MegTestConstants.GetBasePath(),
                    nameof(PackFilesAsMegArchive_Test__ThrowsArgumentException)));
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
            IFileSystem fileSystem = GetFileSystemInternal();
            IMegFileProcessService svc = GetServiceInstance(fileSystem);
            svc.Load(invalidValue);
        }

        [PlatformSpecificTestMethod(TestConstants.PLATFORM_OSX)]
        [TestCategory(TestConstants.TEST_TYPE_HOLY)]
        [TestCategory(TestConstants.TEST_TYPE_API)]
        [ExpectedException(typeof(FileNotFoundException))]
        [DataRow("test/data/path.meg")]
        [DataRow("test/data/")]
        public void Load_Test__GivenInvalidFilePathThrowsFileNotFoundException__OSX(string invalidPath)
        {
            Assert.IsTrue(StringUtility.HasText(invalidPath));
            invalidPath = "mnt/c/" + invalidPath;
            IFileSystem fileSystem = GetFileSystemInternal();
            IMegFileProcessService svc = GetServiceInstance(fileSystem);
            svc.Load(invalidPath);
        }

        [PlatformSpecificTestMethod(TestConstants.PLATFORM_LINUX)]
        [TestCategory(TestConstants.TEST_TYPE_HOLY)]
        [TestCategory(TestConstants.TEST_TYPE_API)]
        [ExpectedException(typeof(FileNotFoundException))]
        [DataRow("test/data/path.meg")]
        [DataRow("test/data/")]
        public void Load_Test__GivenInvalidFilePathThrowsFileNotFoundException_Linux(string invalidPath)
        {
            Assert.IsTrue(StringUtility.HasText(invalidPath));
            invalidPath = "mnt/c/" + invalidPath;
            IFileSystem fileSystem = GetFileSystemInternal();
            IMegFileProcessService svc = GetServiceInstance(fileSystem);
            svc.Load(invalidPath);
        }

        [PlatformSpecificTestMethod(TestConstants.PLATFORM_WINDOWS)]
        [TestCategory(TestConstants.TEST_TYPE_HOLY)]
        [TestCategory(TestConstants.TEST_TYPE_API)]
        [ExpectedException(typeof(FileNotFoundException))]
        [DataRow("test/data/path.meg")]
        [DataRow("test/data/")]
        public void Load_Test__GivenInvalidFilePathThrowsFileNotFoundException__Windows(string invalidPath)
        {
            Assert.IsTrue(StringUtility.HasText(invalidPath));
            invalidPath = "c:/" + invalidPath;
            invalidPath = invalidPath.Replace('/', '\\');
            IFileSystem fileSystem = GetFileSystemInternal();
            IMegFileProcessService svc = GetServiceInstance(fileSystem);
            svc.Load(invalidPath);
        }

        [PlatformSpecificTestMethod(TestConstants.PLATFORM_OSX)]
        [TestCategory(TestConstants.TEST_TYPE_HOLY)]
        [TestCategory(TestConstants.TEST_TYPE_API)]
        public void Load_Test__MegFileHolderIntegrity__OSX()
        {
            IFileSystem fileSystem = GetFileSystemInternal();
            IMegFileProcessService svc = GetServiceInstance(fileSystem);
            MegFileHolder megFileHolder = svc.Load(MegTestConstants.GetMegFilePath());
            Assert.IsTrue(StringUtility.HasText(megFileHolder.FileName));
            string expectedFileName = MegTestConstants.FILE_NAME_MEG_FILE.Replace(".meg", string.Empty);
            string expectedFilePath =
                MegTestConstants.GetMegFilePath().Replace("/" + MegTestConstants.FILE_NAME_MEG_FILE, string.Empty);
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
                        (uint) fileSystem.File.ReadAllBytes(MegTestConstants.GetGameObjectFilesPath()).Length;
                    Assert.AreEqual(expectedFileSize, megFileDataEntry.Size);
                }

                if (megFileDataEntry.RelativeFilePath.Equals(
                    expectedBasePath + MegTestConstants.FILE_NAME_CAMPAIGNFILES,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    uint expectedFileSize =
                        (uint) fileSystem.File.ReadAllBytes(MegTestConstants.GetCampaignFilesPath()).Length;
                    Assert.AreEqual(expectedFileSize, megFileDataEntry.Size);
                }
            }
        }

        [PlatformSpecificTestMethod(TestConstants.PLATFORM_LINUX)]
        [TestCategory(TestConstants.TEST_TYPE_HOLY)]
        [TestCategory(TestConstants.TEST_TYPE_API)]
        public void Load_Test__MegFileHolderIntegrity__Linux()
        {
            IFileSystem fileSystem = GetFileSystemInternal();
            IMegFileProcessService svc = GetServiceInstance(fileSystem);
            MegFileHolder megFileHolder = svc.Load(MegTestConstants.GetMegFilePath());
            Assert.IsTrue(StringUtility.HasText(megFileHolder.FileName));
            string expectedFileName = MegTestConstants.FILE_NAME_MEG_FILE.Replace(".meg", string.Empty);
            string expectedFilePath =
                MegTestConstants.GetMegFilePath().Replace("/" + MegTestConstants.FILE_NAME_MEG_FILE, string.Empty);
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
                        (uint) fileSystem.File.ReadAllBytes(MegTestConstants.GetGameObjectFilesPath()).Length;
                    Assert.AreEqual(expectedFileSize, megFileDataEntry.Size);
                }

                if (megFileDataEntry.RelativeFilePath.Equals(
                    expectedBasePath + MegTestConstants.FILE_NAME_CAMPAIGNFILES,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    uint expectedFileSize =
                        (uint) fileSystem.File.ReadAllBytes(MegTestConstants.GetCampaignFilesPath()).Length;
                    Assert.AreEqual(expectedFileSize, megFileDataEntry.Size);
                }
            }
        }

        [PlatformSpecificTestMethod(TestConstants.PLATFORM_WINDOWS)]
        [TestCategory(TestConstants.TEST_TYPE_HOLY)]
        [TestCategory(TestConstants.TEST_TYPE_API)]
        public void Load_Test__MegFileHolderIntegrity__Windows()
        {
            IFileSystem fileSystem = GetFileSystemInternal();
            IMegFileProcessService svc = GetServiceInstance(fileSystem);
            MegFileHolder megFileHolder = svc.Load(MegTestConstants.GetMegFilePath());
            Assert.IsTrue(StringUtility.HasText(megFileHolder.FileName));
            string expectedFileName = MegTestConstants.FILE_NAME_MEG_FILE.Replace(".meg", string.Empty);
            string expectedFilePath =
                MegTestConstants.GetMegFilePath().Replace("\\" + MegTestConstants.FILE_NAME_MEG_FILE, string.Empty);
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
                        (uint) fileSystem.File.ReadAllBytes(MegTestConstants.GetGameObjectFilesPath()).Length;
                    Assert.AreEqual(expectedFileSize, megFileDataEntry.Size);
                }

                if (megFileDataEntry.RelativeFilePath.Equals(
                    expectedBasePath + MegTestConstants.FILE_NAME_CAMPAIGNFILES,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    uint expectedFileSize =
                        (uint) fileSystem.File.ReadAllBytes(MegTestConstants.GetCampaignFilesPath()).Length;
                    Assert.AreEqual(expectedFileSize, megFileDataEntry.Size);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestConstants.TEST_TYPE_HOLY)]
        [TestCategory(TestConstants.TEST_TYPE_API)]
        public void UnpackMegFile_Test__UnpackedFilesAreBinaryEquivalent()
        {
            IFileSystem fileSystem = GetFileSystemInternal();
            string exportTestPath =
                fileSystem.Path.Combine(MegTestConstants.GetBasePath(),
                    nameof(UnpackMegFile_Test__UnpackedFilesAreBinaryEquivalent));
            IMegFileProcessService svc = GetServiceInstance(fileSystem);
            svc.UnpackMegFile(MegTestConstants.GetMegFilePath(), exportTestPath);
            Assert.IsTrue(fileSystem.Directory.Exists(exportTestPath));
            string fullExportPath = fileSystem.Path.Combine(exportTestPath, "DATA", "XML");
            Assert.IsTrue(fileSystem.Directory.Exists(fullExportPath));
            string[] files = fileSystem.Directory.GetFiles(fullExportPath);
            Assert.IsTrue(files.Length == 2);
            Assert.IsTrue(files.Contains(fileSystem.Path.Combine(fullExportPath,
                MegTestConstants.FILE_NAME_CAMPAIGNFILES.ToUpper())));
            Assert.IsTrue(files.Contains(fileSystem.Path.Combine(fullExportPath,
                MegTestConstants.FILE_NAME_GAMEOBJECTFILES.ToUpper())));
            byte[] expected = fileSystem.File.ReadAllBytes(MegTestConstants.GetGameObjectFilesPath());
            byte[] actual = fileSystem.File.ReadAllBytes(fileSystem.Path.Combine(fullExportPath,
                MegTestConstants.FILE_NAME_GAMEOBJECTFILES.ToUpper()));
            TestUtility.AssertAreBinaryEquivalent(expected, actual);
            expected = fileSystem.File.ReadAllBytes(MegTestConstants.GetGameObjectFilesPath());
            actual = fileSystem.File.ReadAllBytes(fileSystem.Path.Combine(fullExportPath,
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
            IFileSystem fileSystem = GetFileSystemInternal();
            IMegFileProcessService svc = GetServiceInstance(fileSystem);
            MegFileHolder megFileHolder = svc.Load(MegTestConstants.GetMegFilePath());
            svc.UnpackSingleFileFromMegFile(megFileHolder, targetDirectory, fileName);
        }

        [TestMethod]
        [TestCategory(TestConstants.TEST_TYPE_HOLY)]
        [TestCategory(TestConstants.TEST_TYPE_API)]
        [ExpectedException(typeof(FileNotContainedInArchiveException))]
        public void UnpackSingleFileFromMegFile_Test__ThrowsFileNotContainedInArchiveException()
        {
            IFileSystem fileSystem = GetFileSystemInternal();
            string exportTestPath =
                fileSystem.Path.Combine(MegTestConstants.GetBasePath(),
                    "UnpackSingleFileFromMegFile_Test__ThrowsFileNotContainedInArchiveException");
            IMegFileProcessService svc = GetServiceInstance(fileSystem);
            MegFileHolder megFileHolder = svc.Load(MegTestConstants.GetMegFilePath());
            svc.UnpackSingleFileFromMegFile(megFileHolder, exportTestPath, "I_DO_NO_EXIST.XML", false);
        }

        [TestMethod]
        [TestCategory(TestConstants.TEST_TYPE_HOLY)]
        [TestCategory(TestConstants.TEST_TYPE_API)]
        [ExpectedException(typeof(MultipleFilesWithMatchingNameInArchiveException))]
        public void UnpackSingleFileFromMegFile_Test__ThrowsMultipleFilesWithMatchingNameInArchiveException()
        {
            IFileSystem fileSystem = GetFileSystemInternal();
            string exportTestPath =
                fileSystem.Path.Combine(MegTestConstants.GetBasePath(),
                    "UnpackSingleFileFromMegFile_Test__ThrowsFileNotContainedInArchiveException");
            IMegFileProcessService svc = GetServiceInstance(fileSystem);
            MegFileHolder megFileHolder = svc.Load(MegTestConstants.GetMegFilePath());
            svc.UnpackSingleFileFromMegFile(megFileHolder, exportTestPath, "XML", false);
        }

        [TestMethod]
        [TestCategory(TestConstants.TEST_TYPE_HOLY)]
        [TestCategory(TestConstants.TEST_TYPE_API)]
        public void UnpackSingleFileFromMegFile_Test__DirectoryHierarchyPreservedUnpackedFileIsBinaryEquivalent()
        {
            IFileSystem fileSystem = GetFileSystemInternal();
            string exportTestPath =
                fileSystem.Path.Combine(MegTestConstants.GetBasePath(),
                    "UnpackSingleFileFromMegFile_Test__UnpackedFileIsBinaryEquivalent");
            string fullExportPath = fileSystem.Path.Combine(exportTestPath, "DATA", "XML");
            IMegFileProcessService svc = GetServiceInstance(fileSystem);
            MegFileHolder megFileHolder = svc.Load(MegTestConstants.GetMegFilePath());
            svc.UnpackSingleFileFromMegFile(megFileHolder, exportTestPath, MegTestConstants.FILE_NAME_GAMEOBJECTFILES);
            byte[] expected = fileSystem.File.ReadAllBytes(MegTestConstants.GetGameObjectFilesPath());
            byte[] actual = fileSystem.File.ReadAllBytes(fileSystem.Path.Combine(fullExportPath,
                MegTestConstants.FILE_NAME_GAMEOBJECTFILES.ToUpper()));
            TestUtility.AssertAreBinaryEquivalent(expected, actual);
        }

        [TestMethod]
        [TestCategory(TestConstants.TEST_TYPE_HOLY)]
        [TestCategory(TestConstants.TEST_TYPE_API)]
        public void UnpackSingleFileFromMegFile_Test__DirectoryHierarchyFlatUnpackedFileIsBinaryEquivalent()
        {
            IFileSystem fileSystem = GetFileSystemInternal();
            string exportTestPath =
                fileSystem.Path.Combine(MegTestConstants.GetBasePath(),
                    "UnpackSingleFileFromMegFile_Test__UnpackedFileIsBinaryEquivalent");
            IMegFileProcessService svc = GetServiceInstance(fileSystem);
            MegFileHolder megFileHolder = svc.Load(MegTestConstants.GetMegFilePath());
            svc.UnpackSingleFileFromMegFile(megFileHolder, exportTestPath, MegTestConstants.FILE_NAME_GAMEOBJECTFILES,
                false);
            byte[] expected = fileSystem.File.ReadAllBytes(MegTestConstants.GetGameObjectFilesPath());
            byte[] actual = fileSystem.File.ReadAllBytes(fileSystem.Path.Combine(exportTestPath,
                MegTestConstants.FILE_NAME_GAMEOBJECTFILES.ToUpper()));
            TestUtility.AssertAreBinaryEquivalent(expected, actual);
        }

        protected override MegFileProcessService GetServiceInstance(IFileSystem fileSystem)
        {
            return new MegFileProcessService(fileSystem);
        }

        protected override IFileSystem GetFileSystemInternal()
        {
            return new MockFileSystem(new Dictionary<string, MockFileData>
            {
                {
                    MegTestConstants.GetGameObjectFilesPath(),
                    new MockFileData(MegTestConstants.CONTENT_GAMEOBJECTFILES)
                },
                {
                    MegTestConstants.GetCampaignFilesPath(),
                    new MockFileData(MegTestConstants.CONTENT_CAMPAIGNFILES)
                },
                {
                    MegTestConstants.GetMegFilePath(),
                    new MockFileData(MegTestConstants.CONTENT_MEG_FILE)
                }
            });
        }
    }
}
