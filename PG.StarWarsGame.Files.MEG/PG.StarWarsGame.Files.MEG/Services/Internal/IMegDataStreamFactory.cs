using System.IO;

namespace PG.StarWarsGame.Files.MEG.Services;

internal interface IMegDataStreamFactory
{
    Stream CreateDataStream(string path, uint offset, uint size);
}