using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Services;

namespace PG.Commons.Test.Services;

[TestClass]
public class ChecksumServiceTest
{

    // Test Values are extracted from the game's debug logs or produced by Mike's MEG Tool
    [TestMethod]
    [DataRow("Tatooine", -256176565)]
    [DataRow("Corulag", 539193933)]
    [DataRow("TEXT_GUI_DIALOG_TOOLTIP_IDC_MAIN_MENU_SINGLE_PLAYER_GAMES", 72402613)]
    [DataRow("XML\\ABCDEF", 4133962033)]
    [DataRow("", 0)]
    // This test method accepts long so that we can interpret expectedChecksum as either uint or int.
    public void Test_Correct_Checksums(string value, long expectedChecksum)
    {
        var crc = ChecksumService.Instance.GetChecksum(value, Encoding.ASCII);
        Assert.AreEqual((int)expectedChecksum, (int)crc);
        Assert.AreEqual((uint)expectedChecksum, (uint)crc);
    }

    [TestMethod]
    public void Test_Encoding_Ambiguity()
    {
        var crc1 = ChecksumService.Instance.GetChecksum("Ä", Encoding.ASCII);
        var crc2 = ChecksumService.Instance.GetChecksum("ü", Encoding.ASCII);
        Assert.AreEqual(crc1, crc2);
    }

    [TestMethod]
    public void Test_Encoding_NoAmbiguity()
    {
        var crc1 = ChecksumService.Instance.GetChecksum("Ä", Encoding.Unicode);
        var crc2 = ChecksumService.Instance.GetChecksum("ü", Encoding.Unicode);
        Assert.AreNotEqual(crc1, crc2);
    }
}