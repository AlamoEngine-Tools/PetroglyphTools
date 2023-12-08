using System.IO;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Binary;

internal interface IMegVersionIdentifier
{
    MegFileVersion GetMegFileVersion(Stream data, out bool encrypted);
}