using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PG.StarWarsGame.Files.MEG.Binary.V1;

namespace PG.StarWarsGame.Files.MEG.Test.Binary;

[TestClass]
public class MegFileBinaryServiceV1IntegrationTest
{
    private IFileSystem _fileSystem;

    private MegFileBinaryServiceV1 _binaryService;
    private Mock<IServiceProvider> _serviceProviderMock;

    [TestInitialize]
    public void SetUp()
    {
        _fileSystem = new MockFileSystem();
        var sp = new Mock<IServiceProvider>();
        sp.Setup(s => s.GetService(typeof(IFileSystem))).Returns(_fileSystem);
        _serviceProviderMock = sp;

        _binaryService = new MegFileBinaryServiceV1(sp.Object);

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

    [TestMethod]
    public void Test__ReadBinary_EmptyMeg()
    {
    }
}