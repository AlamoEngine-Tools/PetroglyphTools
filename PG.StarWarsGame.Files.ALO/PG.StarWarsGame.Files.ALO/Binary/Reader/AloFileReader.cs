using System.IO;
using PG.StarWarsGame.Files.ALO.Data;
using PG.StarWarsGame.Files.ALO.Services;
using PG.StarWarsGame.Files.ChunkFiles.Binary.Reader;

namespace PG.StarWarsGame.Files.ALO.Binary.Reader;

internal abstract class AloFileReader<T>(AloLoadOptions loadOptions, Stream stream)
    : ChunkFileReaderBase<T>(stream), IAloFileReader<T>  where T : IAloDataContent
{
    protected AloLoadOptions LoadOptions { get; } = loadOptions;
}