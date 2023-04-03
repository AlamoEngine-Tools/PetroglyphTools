// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
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
            IServiceProvider services = GetServiceProviderInternal();
            string targetDirectory = services.GetRequiredService<IFileSystem>().Path.Combine(
                MegTestConstants.GetBasePath(),
                nameof(PackFilesAsMegArchive_Test__CreatedMegFileIsBinaryEquivalent));
            const string testMegFileName = "test_meg_file.meg";
            IMegFileProcessService svc = GetServiceInstance(services);
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
            byte[] expected = services.GetRequiredService<IFileSystem>().File
                .ReadAllBytes(MegTestConstants.GetMegFilePath());
            byte[] actual = services.GetRequiredService<IFileSystem>().File
                .ReadAllBytes(services.GetRequiredService<IFileSystem>().Path
                    .Combine(targetDirectory, testMegFileName));
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
            IServiceProvider services = GetServiceProviderInternal();
            IMegFileProcessService svc = GetServiceInstance(services);
            svc.PackFilesAsMegArchive(nameof(PackFilesAsMegArchive_Test__ThrowsArgumentException),
                new Dictionary<string, string>(),
                services.GetRequiredService<IFileSystem>().Path.Combine(MegTestConstants.GetBasePath(),
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
            IServiceProvider services = GetServiceProviderInternal();
            IMegFileProcessService svc = GetServiceInstance(services);
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
            IServiceProvider services = GetServiceProviderInternal();
            IMegFileProcessService svc = GetServiceInstance();
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
            IServiceProvider services = GetServiceProviderInternal();
            IMegFileProcessService svc = GetServiceInstance();
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
            IMegFileProcessService svc = GetServiceInstance();
            svc.Load(invalidPath);
        }

        [PlatformSpecificTestMethod(TestConstants.PLATFORM_OSX)]
        [TestCategory(TestConstants.TEST_TYPE_HOLY)]
        [TestCategory(TestConstants.TEST_TYPE_API)]
        public void Load_Test__MegFileHolderIntegrity__OSX()
        {
            IServiceProvider services = GetServiceProviderInternal();
            IMegFileProcessService svc = GetServiceInstance(services);
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
                        (uint) services.GetRequiredService<IFileSystem>().File
                            .ReadAllBytes(MegTestConstants.GetGameObjectFilesPath()).Length;
                    Assert.AreEqual(expectedFileSize, megFileDataEntry.Size);
                }

                if (megFileDataEntry.RelativeFilePath.Equals(
                    expectedBasePath + MegTestConstants.FILE_NAME_CAMPAIGNFILES,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    uint expectedFileSize =
                        (uint) services.GetRequiredService<IFileSystem>().File
                            .ReadAllBytes(MegTestConstants.GetCampaignFilesPath()).Length;
                    Assert.AreEqual(expectedFileSize, megFileDataEntry.Size);
                }
            }
        }

        [PlatformSpecificTestMethod(TestConstants.PLATFORM_LINUX)]
        [TestCategory(TestConstants.TEST_TYPE_HOLY)]
        [TestCategory(TestConstants.TEST_TYPE_API)]
        public void Load_Test__MegFileHolderIntegrity__Linux()
        {
            IServiceProvider services = GetServiceProviderInternal();
            IMegFileProcessService svc = GetServiceInstance(services);
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
                        (uint) services.GetRequiredService<IFileSystem>().File
                            .ReadAllBytes(MegTestConstants.GetGameObjectFilesPath()).Length;
                    Assert.AreEqual(expectedFileSize, megFileDataEntry.Size);
                }

                if (megFileDataEntry.RelativeFilePath.Equals(
                    expectedBasePath + MegTestConstants.FILE_NAME_CAMPAIGNFILES,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    uint expectedFileSize =
                        (uint) services.GetRequiredService<IFileSystem>().File
                            .ReadAllBytes(MegTestConstants.GetCampaignFilesPath()).Length;
                    Assert.AreEqual(expectedFileSize, megFileDataEntry.Size);
                }
            }
        }

        [PlatformSpecificTestMethod(TestConstants.PLATFORM_WINDOWS)]
        [TestCategory(TestConstants.TEST_TYPE_HOLY)]
        [TestCategory(TestConstants.TEST_TYPE_API)]
        public void Load_Test__MegFileHolderIntegrity__Windows()
        {
            IServiceProvider services = GetServiceProviderInternal();
            IMegFileProcessService svc = GetServiceInstance(services);
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
                        (uint) services.GetRequiredService<IFileSystem>().File
                            .ReadAllBytes(MegTestConstants.GetGameObjectFilesPath()).Length;
                    Assert.AreEqual(expectedFileSize, megFileDataEntry.Size);
                }

                if (megFileDataEntry.RelativeFilePath.Equals(
                    expectedBasePath + MegTestConstants.FILE_NAME_CAMPAIGNFILES,
                    StringComparison.InvariantCultureIgnoreCase))
                {
                    uint expectedFileSize =
                        (uint) services.GetRequiredService<IFileSystem>().File
                            .ReadAllBytes(MegTestConstants.GetCampaignFilesPath()).Length;
                    Assert.AreEqual(expectedFileSize, megFileDataEntry.Size);
                }
            }
        }

        [TestMethod]
        [TestCategory(TestConstants.TEST_TYPE_HOLY)]
        [TestCategory(TestConstants.TEST_TYPE_API)]
        public void UnpackMegFile_Test__UnpackedFilesAreBinaryEquivalent()
        {
            IServiceProvider services = GetServiceProviderInternal();
            string exportTestPath =
                services.GetRequiredService<IFileSystem>().Path.Combine(MegTestConstants.GetBasePath(),
                    nameof(UnpackMegFile_Test__UnpackedFilesAreBinaryEquivalent));
            IMegFileProcessService svc = GetServiceInstance(services);
            svc.UnpackMegFile(MegTestConstants.GetMegFilePath(), exportTestPath);
            Assert.IsTrue(services.GetRequiredService<IFileSystem>().Directory.Exists(exportTestPath));
            string fullExportPath =
                services.GetRequiredService<IFileSystem>().Path.Combine(exportTestPath, "DATA", "XML");
            Assert.IsTrue(services.GetRequiredService<IFileSystem>().Directory.Exists(fullExportPath));
            string[] files = services.GetRequiredService<IFileSystem>().Directory.GetFiles(fullExportPath);
            Assert.IsTrue(files.Length == 2);
            Assert.IsTrue(files.Contains(services.GetRequiredService<IFileSystem>().Path.Combine(fullExportPath,
                MegTestConstants.FILE_NAME_CAMPAIGNFILES.ToUpper())));
            Assert.IsTrue(files.Contains(services.GetRequiredService<IFileSystem>().Path.Combine(fullExportPath,
                MegTestConstants.FILE_NAME_GAMEOBJECTFILES.ToUpper())));
            byte[] expected = services.GetRequiredService<IFileSystem>().File
                .ReadAllBytes(MegTestConstants.GetGameObjectFilesPath());
            byte[] actual = services.GetRequiredService<IFileSystem>().File.ReadAllBytes(services
                .GetRequiredService<IFileSystem>().Path.Combine(fullExportPath,
                    MegTestConstants.FILE_NAME_GAMEOBJECTFILES.ToUpper()));
            TestUtility.AssertAreBinaryEquivalent(expected, actual);
            expected = services.GetRequiredService<IFileSystem>().File
                .ReadAllBytes(MegTestConstants.GetGameObjectFilesPath());
            actual = services.GetRequiredService<IFileSystem>().File.ReadAllBytes(services
                .GetRequiredService<IFileSystem>().Path.Combine(fullExportPath,
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
            MegFileHolder megFileHolder = svc.Load(MegTestConstants.GetMegFilePath());
            svc.UnpackSingleFileFromMegFile(megFileHolder, targetDirectory, fileName);
        }

        [TestMethod]
        [TestCategory(TestConstants.TEST_TYPE_HOLY)]
        [TestCategory(TestConstants.TEST_TYPE_API)]
        [ExpectedException(typeof(FileNotContainedInArchiveException))]
        public void UnpackSingleFileFromMegFile_Test__ThrowsFileNotContainedInArchiveException()
        {
            IServiceProvider services = GetServiceProviderInternal();
            string exportTestPath =
                services.GetRequiredService<IFileSystem>().Path.Combine(MegTestConstants.GetBasePath(),
                    "UnpackSingleFileFromMegFile_Test__ThrowsFileNotContainedInArchiveException");
            IMegFileProcessService svc = GetServiceInstance(services);
            MegFileHolder megFileHolder = svc.Load(MegTestConstants.GetMegFilePath());
            svc.UnpackSingleFileFromMegFile(megFileHolder, exportTestPath, "I_DO_NO_EXIST.XML", false);
        }

        [TestMethod]
        [TestCategory(TestConstants.TEST_TYPE_HOLY)]
        [TestCategory(TestConstants.TEST_TYPE_API)]
        [ExpectedException(typeof(MultipleFilesWithMatchingNameInArchiveException))]
        public void UnpackSingleFileFromMegFile_Test__ThrowsMultipleFilesWithMatchingNameInArchiveException()
        {
            IServiceProvider services = GetServiceProviderInternal();
            string exportTestPath =
                services.GetRequiredService<IFileSystem>().Path.Combine(MegTestConstants.GetBasePath(),
                    "UnpackSingleFileFromMegFile_Test__ThrowsFileNotContainedInArchiveException");
            IMegFileProcessService svc = GetServiceInstance(services);
            MegFileHolder megFileHolder = svc.Load(MegTestConstants.GetMegFilePath());
            svc.UnpackSingleFileFromMegFile(megFileHolder, exportTestPath, "XML", false);
        }

        [TestMethod]
        [TestCategory(TestConstants.TEST_TYPE_HOLY)]
        [TestCategory(TestConstants.TEST_TYPE_API)]
        public void UnpackSingleFileFromMegFile_Test__DirectoryHierarchyPreservedUnpackedFileIsBinaryEquivalent()
        {
            IServiceProvider services = GetServiceProviderInternal();
            string exportTestPath =
                services.GetRequiredService<IFileSystem>().Path.Combine(MegTestConstants.GetBasePath(),
                    "UnpackSingleFileFromMegFile_Test__UnpackedFileIsBinaryEquivalent");
            string fullExportPath =
                services.GetRequiredService<IFileSystem>().Path.Combine(exportTestPath, "DATA", "XML");
            IMegFileProcessService svc = GetServiceInstance(services);
            MegFileHolder megFileHolder = svc.Load(MegTestConstants.GetMegFilePath());
            svc.UnpackSingleFileFromMegFile(megFileHolder, exportTestPath, MegTestConstants.FILE_NAME_GAMEOBJECTFILES);
            byte[] expected = services.GetRequiredService<IFileSystem>().File
                .ReadAllBytes(MegTestConstants.GetGameObjectFilesPath());
            byte[] actual = services.GetRequiredService<IFileSystem>().File.ReadAllBytes(services
                .GetRequiredService<IFileSystem>().Path.Combine(fullExportPath,
                    MegTestConstants.FILE_NAME_GAMEOBJECTFILES.ToUpper()));
            TestUtility.AssertAreBinaryEquivalent(expected, actual);
        }

        [TestMethod]
        [TestCategory(TestConstants.TEST_TYPE_HOLY)]
        [TestCategory(TestConstants.TEST_TYPE_API)]
        public void UnpackSingleFileFromMegFile_Test__DirectoryHierarchyFlatUnpackedFileIsBinaryEquivalent()
        {
            IServiceProvider services = GetServiceProviderInternal();
            string exportTestPath = services.GetRequiredService<IFileSystem>().Path.Combine(
                MegTestConstants.GetBasePath(),
                "UnpackSingleFileFromMegFile_Test__UnpackedFileIsBinaryEquivalent");
            IMegFileProcessService svc = GetServiceInstance(services);
            MegFileHolder megFileHolder = svc.Load(MegTestConstants.GetMegFilePath());
            svc.UnpackSingleFileFromMegFile(megFileHolder, exportTestPath, MegTestConstants.FILE_NAME_GAMEOBJECTFILES,
                false);
            byte[] expected = services.GetRequiredService<IFileSystem>().File
                .ReadAllBytes(MegTestConstants.GetGameObjectFilesPath());
            byte[] actual = services.GetRequiredService<IFileSystem>().File.ReadAllBytes(services
                .GetRequiredService<IFileSystem>().Path.Combine(exportTestPath,
                    MegTestConstants.FILE_NAME_GAMEOBJECTFILES.ToUpper()));
            TestUtility.AssertAreBinaryEquivalent(expected, actual);
        }


        public class MegFileServiceMockFileSystem : MockFileSystem
        {
            public MegFileServiceMockFileSystem() : base(new Dictionary<string, MockFileData>
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
            })
            {
            }
        }

        protected override IServiceProvider GetServiceProviderInternal()
        {
            ServiceProvider serviceProvider = new ServiceCollection()
                .AddSingleton<IFileSystem, MegFileServiceMockFileSystem>()
                .AddSingleton<ILoggerFactory, NullLoggerFactory>().BuildServiceProvider();
            return serviceProvider;
        }
    }
}
