using System.IO;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.Entries;

namespace PG.StarWarsGame.Files.MEG.Services;

internal interface IMegDataStreamFactory
{
    Stream GetDataStream(MegDataEntryOriginInfo originInfo);

    Stream GetDataStream(MegFileDataEntryReference dateReference);
}