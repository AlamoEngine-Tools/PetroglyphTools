using PG.StarWarsGame.Files.ALO.Data;
using PG.StarWarsGame.Files.ChunkFiles.Binary.Reader;

namespace PG.StarWarsGame.Files.ALO.Binary.Reader;

internal interface IAloFileReader<out T> : IChunkFileReader<T> where T : IAloDataContent;