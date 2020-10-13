using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
                }
            });
        }
    }
}