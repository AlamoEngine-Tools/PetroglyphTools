using System.IO;
using PG.StarWarsGame.Files.ALO.Binary.Reader;
using PG.StarWarsGame.Files.ALO.Data;
using PG.StarWarsGame.Files.ALO.Files;
using PG.StarWarsGame.Files.ALO.Services;

namespace PG.StarWarsGame.Files.ALO.Binary;

internal interface IAloFileReaderFactory
{
    IAloFileReader<IAloDataContent> GetReader(AloContentInfo contentInfo, Stream dataStream, AloLoadOptions loadOptions);
}