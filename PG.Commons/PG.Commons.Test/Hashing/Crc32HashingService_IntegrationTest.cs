using System;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using AnakinRaW.CommonUtilities.Hashing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.Commons.Hashing;
using Testably.Abstractions.Testing;

namespace PG.Commons.Test.Hashing;

[TestClass]
public class Crc32HashingService_IntegrationTest
{

    private Crc32HashingService _crc32HashingService = null!;

    [TestInitialize]
    public void Setup()
    {
        var sc = new ServiceCollection();
        sc.AddSingleton<IFileSystem>(sp => new MockFileSystem());
        sc.AddSingleton<IHashingService>(sp => new HashingService(sp));
        sc.AddSingleton<IHashAlgorithmProvider>(new Crc32HashingProvider());
        _crc32HashingService = new Crc32HashingService(sc.BuildServiceProvider());
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
    public void Test_GetChecksum_KnowsPG_Values(string value, long expectedChecksum)
    {
        var crc = _crc32HashingService.GetCrc32(value, Encoding.ASCII);
        Assert.AreEqual((int)expectedChecksum, (int)crc);
        Assert.AreEqual((uint)expectedChecksum, (uint)crc);


        var bytes = Encoding.ASCII.GetBytes(value);
        crc = _crc32HashingService.GetCrc32(new ReadOnlySpan<byte>(bytes));
        Assert.AreEqual((int)expectedChecksum, (int)crc);
        Assert.AreEqual((uint)expectedChecksum, (uint)crc);


        crc = _crc32HashingService.GetCrc32(new MemoryStream(bytes));
        Assert.AreEqual((int)expectedChecksum, (int)crc);
        Assert.AreEqual((uint)expectedChecksum, (uint)crc);

    }

    [TestMethod]
    public void Test_GetChecksum_String_Encoding_Ambiguity()
    {
        var crc1 = _crc32HashingService.GetCrc32("Ä", Encoding.ASCII);
        var crc2 = _crc32HashingService.GetCrc32("ü", Encoding.ASCII);
        Assert.AreEqual(crc1, crc2);
    }

    [TestMethod]
    public void Test_GetChecksum_String_Encoding_NoAmbiguity()
    {
        var crc1 = _crc32HashingService.GetCrc32("Ä", Encoding.Unicode);
        var crc2 = _crc32HashingService.GetCrc32("ü", Encoding.Unicode);
        Assert.AreNotEqual(crc1, crc2);
    }
}