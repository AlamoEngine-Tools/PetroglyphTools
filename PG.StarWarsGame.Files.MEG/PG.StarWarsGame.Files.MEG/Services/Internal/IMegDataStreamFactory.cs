using System.IO;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.Entries;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Services;

internal interface IMegDataStreamFactory
{
    Stream GetDataStream(MegDataEntryOriginInfo originInfo);

    Stream GetDataStream(IMegFile megFile, MegDataEntry dataEntry);
}