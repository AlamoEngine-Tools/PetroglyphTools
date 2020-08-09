using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PG.StarWarsGame.Files.MEG.Test.Binary.File.Builder
{
    public class MegFileBuilderTest
    {
        private IFileSystem m_fileSystem;

        [TestInitialize]
        public void SetUp()
        {
            m_fileSystem = new MockFileSystem();
        }
    }
}