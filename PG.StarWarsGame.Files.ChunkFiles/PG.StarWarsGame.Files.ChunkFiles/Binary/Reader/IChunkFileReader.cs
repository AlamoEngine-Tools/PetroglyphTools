using System;
using PG.StarWarsGame.Files.ChunkFiles.Data;

namespace PG.StarWarsGame.Files.ChunkFiles.Binary.Reader;

public interface IChunkFileReader : IDisposable
{
    IChunkData Read();
}

public interface IChunkFileReader<out T> : IChunkFileReader where T : IChunkData
{
    new T Read();
}