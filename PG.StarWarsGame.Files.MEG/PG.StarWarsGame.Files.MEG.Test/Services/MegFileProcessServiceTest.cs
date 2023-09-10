// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Services;
using PG.Testing;

namespace PG.StarWarsGame.Files.MEG.Test.Services;

[TestClass]
public class MegFileProcessServiceTest : ServiceTestBase
{
    //[TestMethod]
    //public void PackFilesAsMegArchive_Test__CreatedMegFileIsBinaryEquivalent()
    //{
    //    var services = GetServiceProviderInternal();
    //    var targetDirectory = services.GetRequiredService<IFileSystem>().Path.Combine(
    //        MegTestConstants.GetBasePath(),
    //        nameof(PackFilesAsMegArchive_Test__CreatedMegFileIsBinaryEquivalent));
    //    const string testMegFileName = "test_meg_file.meg";
    //    IMegFileProcessService svc = GetServiceInstance(services);
    //    svc.PackFilesAsMegArchive(testMegFileName,
    //        new Dictionary<string, string>
    //        {
    //            {
    //                $"DATA/XML/{MegTestConstants.FILE_NAME_CAMPAIGNFILES.ToUpper()}",
    //                MegTestConstants.GetCampaignFilesPath()
    //            },
    //            {
    //                $"DATA/XML/{MegTestConstants.FILE_NAME_GAMEOBJECTFILES.ToUpper()}",
    //                MegTestConstants.GetGameObjectFilesPath()
    //            }
    //        },
    //        targetDirectory);
    //    var expected = services.GetRequiredService<IFileSystem>().File
    //        .ReadAllBytes(MegTestConstants.GetMegFilePath());
    //    var actual = services.GetRequiredService<IFileSystem>().File
    //        .ReadAllBytes(services.GetRequiredService<IFileSystem>().Path
    //            .Combine(targetDirectory, testMegFileName));
    //    TestUtility.AssertAreBinaryEquivalent(expected, actual);
    //}

    //[TestMethod]
    //[DataRow(null, null)]
    //[DataRow("", null)]
    //[DataRow("    ", null)]
    //[DataRow("test", null)]
    //[DataRow("test", "")]
    //[DataRow("test", "    ")]
    //[ExpectedException(typeof(ArgumentException))]
    //public void PackFilesAsMegArchive_Test__ThrowsArgumentExceptionForStringTypes(string megArchiveName,
    //    string targetDirectory)
    //{
    //    IMegFileProcessService svc = GetServiceInstance();
    //    svc.PackFilesAsMegArchive(megArchiveName,
    //        new Dictionary<string, string>
    //        {
    //            {
    //                $"DATA/XML/{MegTestConstants.FILE_NAME_CAMPAIGNFILES.ToUpper()}",
    //                MegTestConstants.GetCampaignFilesPath()
    //            },
    //            {
    //                $"DATA/XML/{MegTestConstants.FILE_NAME_GAMEOBJECTFILES.ToUpper()}",
    //                MegTestConstants.GetGameObjectFilesPath()
    //            }
    //        },
    //        targetDirectory);
    //}

    //[TestMethod]
    //[ExpectedException(typeof(ArgumentException))]
    //public void PackFilesAsMegArchive_Test__ThrowsArgumentException()
    //{
    //    var services = GetServiceProviderInternal();
    //    IMegFileProcessService svc = GetServiceInstance(services);
    //    svc.PackFilesAsMegArchive(nameof(PackFilesAsMegArchive_Test__ThrowsArgumentException),
    //        new Dictionary<string, string>(),
    //        services.GetRequiredService<IFileSystem>().Path.Combine(MegTestConstants.GetBasePath(),
    //            nameof(PackFilesAsMegArchive_Test__ThrowsArgumentException)));
    //}

    //[TestMethod]
    //[ExpectedException(typeof(ArgumentNullException))]
    //[DataRow(null)]
    //[DataRow("")]
    //[DataRow("\r\n")]
    //[DataRow("\t    \r\n")]
    //public void Load_Test__GivenInvalidFilePathThrowsArgumentNullException(string invalidValue)
    //{
    //    Assert.IsFalse(StringUtility.HasText(invalidValue));
    //    var services = GetServiceProviderInternal();
    //    IMegFileProcessService svc = GetServiceInstance(services);
    //    svc.Load(invalidValue);
    //}

    //[PlatformSpecificTestMethod(TestConstants.PLATFORM_LINUX)]
    //[ExpectedException(typeof(FileNotFoundException))]
    //[DataRow("test/data/path.meg")]
    //[DataRow("test/data/")]
    //public void Load_Test__GivenInvalidFilePathThrowsFileNotFoundException_Linux(string invalidPath)
    //{
    //    Assert.IsTrue(StringUtility.HasText(invalidPath));
    //    invalidPath = "mnt/c/" + invalidPath;
    //    var services = GetServiceProviderInternal();
    //    IMegFileProcessService svc = GetServiceInstance();
    //    svc.Load(invalidPath);
    //}

    //[PlatformSpecificTestMethod(TestConstants.PLATFORM_WINDOWS)]
    //[ExpectedException(typeof(FileNotFoundException))]
    //[DataRow("test/data/path.meg")]
    //[DataRow("test/data/")]
    //public void Load_Test__GivenInvalidFilePathThrowsFileNotFoundException__Windows(string invalidPath)
    //{
    //    Assert.IsTrue(StringUtility.HasText(invalidPath));
    //    invalidPath = "c:/" + invalidPath;
    //    invalidPath = invalidPath.Replace('/', '\\');
    //    IMegFileProcessService svc = GetServiceInstance();
    //    svc.Load(invalidPath);
    //}

    //[PlatformSpecificTestMethod(TestConstants.PLATFORM_LINUX)]
    //public void Load_Test__MegFileHolderIntegrity__Linux()
    //{
    //    var services = GetServiceProviderInternal();
    //    IMegFileProcessService svc = GetServiceInstance(services);
    //    var megFileHolder = svc.Load(MegTestConstants.GetMegFilePath());
    //    Assert.IsTrue(StringUtility.HasText(megFileHolder.FileName));
    //    var expectedFileName = MegTestConstants.FILE_NAME_MEG_FILE.Replace(".meg", string.Empty);
    //    var expectedFilePath =
    //        MegTestConstants.GetMegFilePath().Replace("/" + MegTestConstants.FILE_NAME_MEG_FILE, string.Empty);
    //    Assert.IsTrue(expectedFileName.Equals(megFileHolder.FileName, StringComparison.InvariantCultureIgnoreCase));
    //    Assert.IsTrue(expectedFilePath.Equals(megFileHolder.FilePath, StringComparison.InvariantCultureIgnoreCase));
    //    Assert.AreEqual(2, megFileHolder.Content.Count);
    //    const string expectedBasePath = "DATA/XML/";
    //    foreach (var megFileDataEntry in megFileHolder.Content)
    //    {
    //        if (megFileDataEntry.RelativeFilePath.Equals(
    //                expectedBasePath + MegTestConstants.FILE_NAME_GAMEOBJECTFILES,
    //                StringComparison.InvariantCultureIgnoreCase))
    //        {
    //            var expectedFileSize =
    //                (uint) services.GetRequiredService<IFileSystem>().File
    //                    .ReadAllBytes(MegTestConstants.GetGameObjectFilesPath()).Length;
    //            Assert.AreEqual(expectedFileSize, megFileDataEntry.Size);
    //        }

    //        if (megFileDataEntry.RelativeFilePath.Equals(
    //                expectedBasePath + MegTestConstants.FILE_NAME_CAMPAIGNFILES,
    //                StringComparison.InvariantCultureIgnoreCase))
    //        {
    //            var expectedFileSize =
    //                (uint) services.GetRequiredService<IFileSystem>().File
    //                    .ReadAllBytes(MegTestConstants.GetCampaignFilesPath()).Length;
    //            Assert.AreEqual(expectedFileSize, megFileDataEntry.Size);
    //        }
    //    }
    //}

    //[PlatformSpecificTestMethod(TestConstants.PLATFORM_WINDOWS)]
    //public void Load_Test__MegFileHolderIntegrity__Windows()
    //{
    //    var services = GetServiceProviderInternal();
    //    IMegFileProcessService svc = GetServiceInstance(services);
    //    var megFileHolder = svc.Load(MegTestConstants.GetMegFilePath());
    //    Assert.IsTrue(StringUtility.HasText(megFileHolder.FileName));
    //    var expectedFileName = MegTestConstants.FILE_NAME_MEG_FILE.Replace(".meg", string.Empty);
    //    var expectedFilePath =
    //        MegTestConstants.GetMegFilePath().Replace("\\" + MegTestConstants.FILE_NAME_MEG_FILE, string.Empty);
    //    Assert.IsTrue(expectedFileName.Equals(megFileHolder.FileName, StringComparison.InvariantCultureIgnoreCase));
    //    Assert.IsTrue(expectedFilePath.Equals(megFileHolder.FilePath, StringComparison.InvariantCultureIgnoreCase));
    //    Assert.AreEqual(2, megFileHolder.Content.Count);
    //    const string expectedBasePath = "DATA/XML/";
    //    foreach (var megFileDataEntry in megFileHolder.Content)
    //    {
    //        if (megFileDataEntry.RelativeFilePath.Equals(
    //                expectedBasePath + MegTestConstants.FILE_NAME_GAMEOBJECTFILES,
    //                StringComparison.InvariantCultureIgnoreCase))
    //        {
    //            var expectedFileSize =
    //                (uint) services.GetRequiredService<IFileSystem>().File
    //                    .ReadAllBytes(MegTestConstants.GetGameObjectFilesPath()).Length;
    //            Assert.AreEqual(expectedFileSize, megFileDataEntry.Size);
    //        }

    //        if (megFileDataEntry.RelativeFilePath.Equals(
    //                expectedBasePath + MegTestConstants.FILE_NAME_CAMPAIGNFILES,
    //                StringComparison.InvariantCultureIgnoreCase))
    //        {
    //            var expectedFileSize =
    //                (uint) services.GetRequiredService<IFileSystem>().File
    //                    .ReadAllBytes(MegTestConstants.GetCampaignFilesPath()).Length;
    //            Assert.AreEqual(expectedFileSize, megFileDataEntry.Size);
    //        }
    //    }
    //}

    //[TestMethod]
    //public void UnpackMegFile_Test__UnpackedFilesAreBinaryEquivalent()
    //{
    //    var services = GetServiceProviderInternal();
    //    var exportTestPath =
    //        services.GetRequiredService<IFileSystem>().Path.Combine(MegTestConstants.GetBasePath(),
    //            nameof(UnpackMegFile_Test__UnpackedFilesAreBinaryEquivalent));
    //    IMegFileProcessService svc = GetServiceInstance(services);
    //    svc.UnpackMegFile(MegTestConstants.GetMegFilePath(), exportTestPath);
    //    Assert.IsTrue(services.GetRequiredService<IFileSystem>().Directory.Exists(exportTestPath));
    //    var fullExportPath =
    //        services.GetRequiredService<IFileSystem>().Path.Combine(exportTestPath, "DATA", "XML");
    //    Assert.IsTrue(services.GetRequiredService<IFileSystem>().Directory.Exists(fullExportPath));
    //    var files = services.GetRequiredService<IFileSystem>().Directory.GetFiles(fullExportPath);
    //    Assert.IsTrue(files.Length == 2);
    //    Assert.IsTrue(files.Contains(services.GetRequiredService<IFileSystem>().Path.Combine(fullExportPath,
    //        MegTestConstants.FILE_NAME_CAMPAIGNFILES.ToUpper())));
    //    Assert.IsTrue(files.Contains(services.GetRequiredService<IFileSystem>().Path.Combine(fullExportPath,
    //        MegTestConstants.FILE_NAME_GAMEOBJECTFILES.ToUpper())));
    //    var expected = services.GetRequiredService<IFileSystem>().File
    //        .ReadAllBytes(MegTestConstants.GetGameObjectFilesPath());
    //    var actual = services.GetRequiredService<IFileSystem>().File.ReadAllBytes(services
    //        .GetRequiredService<IFileSystem>().Path.Combine(fullExportPath,
    //            MegTestConstants.FILE_NAME_GAMEOBJECTFILES.ToUpper()));
    //    TestUtility.AssertAreBinaryEquivalent(expected, actual);
    //    expected = services.GetRequiredService<IFileSystem>().File
    //        .ReadAllBytes(MegTestConstants.GetGameObjectFilesPath());
    //    actual = services.GetRequiredService<IFileSystem>().File.ReadAllBytes(services
    //        .GetRequiredService<IFileSystem>().Path.Combine(fullExportPath,
    //            MegTestConstants.FILE_NAME_GAMEOBJECTFILES.ToUpper()));
    //    TestUtility.AssertAreBinaryEquivalent(expected, actual);
    //}

    //[TestMethod]
    //[DataRow("", MegTestConstants.FILE_NAME_GAMEOBJECTFILES)]
    //[DataRow(@"c:\mod\data\xml\gameobjectfiles.xml", "")]
    //[ExpectedException(typeof(ArgumentException))]
    //public void UnpackSingleFileFromMegFile_Test__ThrowsArgumentException(string targetDirectory, string fileName)
    //{
    //    IMegFileProcessService svc = GetServiceInstance();
    //    var megFileHolder = svc.Load(MegTestConstants.GetMegFilePath());
    //    svc.UnpackSingleFileFromMegFile(megFileHolder, targetDirectory, fileName);
    //}

    //[TestMethod]
    //[ExpectedException(typeof(FileNotInMegException))]
    //public void UnpackSingleFileFromMegFile_Test__ThrowsFileNotContainedInArchiveException()
    //{
    //    var services = GetServiceProviderInternal();
    //    var exportTestPath =
    //        services.GetRequiredService<IFileSystem>().Path.Combine(MegTestConstants.GetBasePath(),
    //            "UnpackSingleFileFromMegFile_Test__ThrowsFileNotContainedInArchiveException");
    //    IMegFileProcessService svc = GetServiceInstance(services);
    //    var megFileHolder = svc.Load(MegTestConstants.GetMegFilePath());
    //    svc.UnpackSingleFileFromMegFile(megFileHolder, exportTestPath, "I_DO_NO_EXIST.XML", false);
    //}

    //[TestMethod]
    //[ExpectedException(typeof(MultipleFilesWithMatchingNameInArchiveException))]
    //public void UnpackSingleFileFromMegFile_Test__ThrowsMultipleFilesWithMatchingNameInArchiveException()
    //{
    //    var services = GetServiceProviderInternal();
    //    var exportTestPath =
    //        services.GetRequiredService<IFileSystem>().Path.Combine(MegTestConstants.GetBasePath(),
    //            "UnpackSingleFileFromMegFile_Test__ThrowsFileNotContainedInArchiveException");
    //    IMegFileProcessService svc = GetServiceInstance(services);
    //    var megFileHolder = svc.Load(MegTestConstants.GetMegFilePath());
    //    svc.UnpackSingleFileFromMegFile(megFileHolder, exportTestPath, "XML", false);
    //}

    //[TestMethod]
    //public void UnpackSingleFileFromMegFile_Test__DirectoryHierarchyPreservedUnpackedFileIsBinaryEquivalent()
    //{
    //    var services = GetServiceProviderInternal();
    //    var exportTestPath =
    //        services.GetRequiredService<IFileSystem>().Path.Combine(MegTestConstants.GetBasePath(),
    //            "UnpackSingleFileFromMegFile_Test__UnpackedFileIsBinaryEquivalent");
    //    var fullExportPath =
    //        services.GetRequiredService<IFileSystem>().Path.Combine(exportTestPath, "DATA", "XML");
    //    IMegFileProcessService svc = GetServiceInstance(services);
    //    var megFileHolder = svc.Load(MegTestConstants.GetMegFilePath());
    //    svc.UnpackSingleFileFromMegFile(megFileHolder, exportTestPath, MegTestConstants.FILE_NAME_GAMEOBJECTFILES);
    //    var expected = services.GetRequiredService<IFileSystem>().File
    //        .ReadAllBytes(MegTestConstants.GetGameObjectFilesPath());
    //    var actual = services.GetRequiredService<IFileSystem>().File.ReadAllBytes(services
    //        .GetRequiredService<IFileSystem>().Path.Combine(fullExportPath,
    //            MegTestConstants.FILE_NAME_GAMEOBJECTFILES.ToUpper()));
    //    TestUtility.AssertAreBinaryEquivalent(expected, actual);
    //}

    //[TestMethod]
    //public void UnpackSingleFileFromMegFile_Test__DirectoryHierarchyFlatUnpackedFileIsBinaryEquivalent()
    //{
    //    var services = GetServiceProviderInternal();
    //    var exportTestPath = services.GetRequiredService<IFileSystem>().Path.Combine(
    //        MegTestConstants.GetBasePath(),
    //        "UnpackSingleFileFromMegFile_Test__UnpackedFileIsBinaryEquivalent");
    //    IMegFileProcessService svc = GetServiceInstance(services);
    //    var megFileHolder = svc.Load(MegTestConstants.GetMegFilePath());
    //    svc.UnpackSingleFileFromMegFile(megFileHolder, exportTestPath, MegTestConstants.FILE_NAME_GAMEOBJECTFILES,
    //        false);
    //    var expected = services.GetRequiredService<IFileSystem>().File
    //        .ReadAllBytes(MegTestConstants.GetGameObjectFilesPath());
    //    var actual = services.GetRequiredService<IFileSystem>().File.ReadAllBytes(services
    //        .GetRequiredService<IFileSystem>().Path.Combine(exportTestPath,
    //            MegTestConstants.FILE_NAME_GAMEOBJECTFILES.ToUpper()));
    //    TestUtility.AssertAreBinaryEquivalent(expected, actual);
    //}


    //public class MegFileServiceMockFileSystem : MockFileSystem
    //{
    //    public MegFileServiceMockFileSystem() : base(new Dictionary<string, MockFileData>
    //    {
    //        {
    //            MegTestConstants.GetGameObjectFilesPath(),
    //            new MockFileData(MegTestConstants.CONTENT_GAMEOBJECTFILES)
    //        },
    //        {
    //            MegTestConstants.GetCampaignFilesPath(),
    //            new MockFileData(MegTestConstants.CONTENT_CAMPAIGNFILES)
    //        },
    //        {
    //            MegTestConstants.GetMegFilePath(),
    //            new MockFileData(MegTestConstants.CONTENT_MEG_FILE_V1)
    //        }
    //    })
    //    {
    //    }
    //}

    //protected override IServiceProvider GetServiceProviderInternal()
    //{
    //    var serviceProvider = new ServiceCollection()
    //        .AddSingleton<IFileSystem, MegFileServiceMockFileSystem>()
    //        .AddSingleton<ILoggerFactory, NullLoggerFactory>().BuildServiceProvider();
    //    return serviceProvider;
    //}
    protected override Type GetServiceClass()
    {
        return typeof(MegFileService);
    }
}