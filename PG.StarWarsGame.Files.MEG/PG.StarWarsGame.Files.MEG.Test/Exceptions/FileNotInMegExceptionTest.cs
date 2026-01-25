using AnakinRaW.CommonUtilities.Testing.Extensions;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Test.Data.Entries;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Exceptions;

public class FileNotInMegExceptionTest : CommonMegTestBase
{
    [Fact]
    public void Ctor()
    {
        FileSystem.File.Create("a.meg");
        FileSystem.File.Create("b.meg");

        var megFileA = new MegFile(new MegArchive([]), new MegFileInformation("a.meg", MegFileVersion.V1),
            ServiceProvider);
        var entry = MegDataEntryTest.CreateEntry("path");

        var e = new FileNotInMegException(new MegDataEntryLocationReference(megFileA, entry));
        Assert.Exception(e, message: $"The file \"{entry.FilePath}\" is not contained in the MEG archive \"{megFileA.FilePath}\"");
    }
}