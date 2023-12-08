using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Hashing;

namespace PG.Commons.Test.Hashing;

[TestClass]
public class ChecksumServiceTest
{
    [TestMethod]
    public void Test_GetChecksum_Throws()
    {
        var checksumService = new ChecksumService();
        Assert.ThrowsException<ArgumentNullException>(() => checksumService.GetChecksum(null!, Encoding.ASCII));
        Assert.ThrowsException<ArgumentNullException>(() => checksumService.GetChecksum("", null));
    }


    // Test Values are extracted from the game's debug logs or produced by Mike's MEG Tool
    [TestMethod]
    [DataRow("Tatooine", -256176565)]
    [DataRow("Corulag", 539193933)]
    [DataRow("TEXT_GUI_DIALOG_TOOLTIP_IDC_MAIN_MENU_SINGLE_PLAYER_GAMES", 72402613)]
    [DataRow("XML\\ABCDEF", 4133962033)]
    [DataRow("", 0)]
    [DataRow("Gcum8qzFZ1xYTTzY1N9avqWRM1q3qk22sfANKZykb8YGIb3fm1znkEPytzwk3qbzhwkIOlTVsC3dGde0vMxqcS6lF2wQi3rI7oMaJMdVdwkvYSf2zNU2qUBDXF1wGgHhhqeMvE6X479nKlsPFjjKG2AHRYqfOQttbZ8AW4n7p5wkrP2ScL08KGLls8vuZ8TXigsiySnBMqOX9SThW7IAApIn60NGiFNZtsC4FrhUXkLhuvDLtO9NyvdVMNLf5Z7iacGWSGrcTI0w9PrUBxFETwy3M2klGlvUM2P3kpvZzigYpPgo7wkicm7IclUbVjrBv2nLtzFDzXDtg2nP5RTXkPLtFEoeTMK1Y9nH6U9omcikyW92lyfUgh60fYWXIPsPwiCmS9K3jBcOz9V07T5HARtJmQFU9Z6nTwRRszzDqHs9KbWVV9cnCJ7bdPX2T3bS5O0lHnlVGhWYhiS5Il3lBJSyzuRLZ3N6OhO8uJ2gdGImN1hg928JRE132s0tOWGx", 1080801695)]
    // This test method accepts long so that we can interpret expectedChecksum as either uint or int.
    public void Test_GetChecksum(string value, long expectedChecksum)
    {
        var crc = new ChecksumService().GetChecksum(value, Encoding.ASCII);
        Assert.AreEqual((int)expectedChecksum, (int)crc);
        Assert.AreEqual((uint)expectedChecksum, (uint)crc);
    }

    [TestMethod]
    public void Test_GetChecksum_Encoding_Ambiguity()
    {
        var checksumService = new ChecksumService();
        var crc1 = checksumService.GetChecksum("Ä", Encoding.ASCII);
        var crc2 = checksumService.GetChecksum("ü", Encoding.ASCII);
        Assert.AreEqual(crc1, crc2);
    }

    [TestMethod]
    public void Test_GetChecksum_Encoding_NoAmbiguity()
    {
        var checksumService = new ChecksumService();
        var crc1 = checksumService.GetChecksum("Ä", Encoding.Unicode);
        var crc2 = checksumService.GetChecksum("ü", Encoding.Unicode);
        Assert.AreNotEqual(crc1, crc2);
    }
}