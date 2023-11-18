using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Files;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Test.Files;

[TestClass]
public class MegAlamoFileTypeTest
{
    [TestMethod]
    public void Test_Ctor()
    {
        var fileType = new MegAlamoFileType();
        Assert.AreEqual("meg", fileType.FileExtension);
        Assert.AreEqual(FileType.Binary, fileType.Type);
    }
}