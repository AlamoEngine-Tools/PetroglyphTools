using System.IO;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;

namespace PG.StarWarsGame.Files.MEG.Services;

internal interface IMegDataStreamFactory
{
    Stream GetDataStream(MegDataEntryOriginInfo originInfo);

    Stream GetDataStream(MegDataEntryLocationReference locationReference);
}