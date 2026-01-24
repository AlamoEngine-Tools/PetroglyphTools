using PG.StarWarsGame.Files.DAT.Files;

namespace PG.StarWarsGame.Files.DAT.Test.Services.Builder;

public abstract class PetroglyphStarWarsGameDatBuilder : DatBuilderBaseTest
{
    protected override bool FileInfoIsAlwaysValid => false;

    protected override DatFileInformation CreateFileInfo(bool valid, string path)
    {
        return new DatFileInformation
        {
            FilePath = valid ? path : "FileÖÄÜ.dat"
        };
    }
}