// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PG.StarWarsGame.Files.MEG.Test.Binary;

[TestClass]
public class MegFileBuilderTest
{
    private IFileSystem _fileSystem;

    [TestInitialize]
    public void SetUp()
    {
        _fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
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

    //[TestMethod]
    //public void FromHolder_Test__FileHeaderIsConsistent()
    //{
    //    var megFileHolder = new MegFileHolder(MegTestConstants.GetBasePath(), "test");
    //    megFileHolder.Content.Add(new MegFileDataEntry("data/xml/campaignfiles.xml",
    //        MegTestConstants.GetCampaignFilesPath()));
    //    megFileHolder.Content.Add(new MegFileDataEntry("data/xml/gameobjectfiles.xml",
    //        MegTestConstants.GetGameObjectFilesPath()));
    //    var builder = new MegFileBinaryServiceV1(m_fileSystem);
    //    var megFile = builder.FromHolder(megFileHolder);
    //    Assert.IsNotNull(megFile);
    //    Assert.IsNotNull(megFile.Header);
    //    Assert.IsNotNull(megFile.FileContentTable);
    //    Assert.IsNotNull(megFile.FileNameTable);
    //    Assert.AreEqual(2u, megFile.Header.NumFiles);
    //    Assert.AreEqual(2u, megFile.Header.NumFileNames);
    //    Assert.AreEqual(40, megFile.FileContentTable.Size);
    //    Assert.AreEqual(58, megFile.FileNameTable.Size);
    //    Assert.IsTrue(megFile.FileNameTable[0].FileName
    //        .Equals("data/xml/gameobjectfiles.xml", StringComparison.InvariantCultureIgnoreCase));
    //    Assert.IsTrue(megFile.FileNameTable[1].FileName.Equals("data/xml/campaignfiles.xml",
    //        StringComparison.InvariantCultureIgnoreCase));
    //}

    //[TestMethod]
    //public void FromHolder_Test__FileHeaderIsBinaryEquivalent()
    //{
    //    var megFileHolder = new MegFileHolder(MegTestConstants.GetBasePath(),
    //        "FromHolder_Test__FileHeaderIsBinaryEquivalent");
    //    megFileHolder.Content.Add(new MegFileDataEntry("data/xml/campaignfiles.xml",
    //        MegTestConstants.GetCampaignFilesPath()));
    //    megFileHolder.Content.Add(new MegFileDataEntry("data/xml/gameobjectfiles.xml",
    //        MegTestConstants.GetGameObjectFilesPath()));
    //    var builder = new MegFileBinaryServiceV1(m_fileSystem);
    //    var megFile = builder.FromHolder(megFileHolder);
    //    Assert.IsNotNull(megFile);
    //    m_fileSystem.File.WriteAllBytes(
    //        m_fileSystem.Path.Combine(MegTestConstants.GetBasePath(),
    //            megFileHolder.FullyQualifiedName), megFile.ToBytes());
    //    var readMegFile = m_fileSystem.File.ReadAllBytes(m_fileSystem.Path.Combine(MegTestConstants.GetBasePath(),
    //        megFileHolder.FullyQualifiedName));
    //    Assert.AreEqual(MegTestConstants.CONTENT_MEG_FILE_HEADER.Length, readMegFile.Length);
    //    for (var i = 0; i < MegTestConstants.CONTENT_MEG_FILE_HEADER.Length; i++)
    //    {
    //        Assert.AreEqual(MegTestConstants.CONTENT_MEG_FILE_HEADER[i], readMegFile[i]);
    //    }
    //}
}
