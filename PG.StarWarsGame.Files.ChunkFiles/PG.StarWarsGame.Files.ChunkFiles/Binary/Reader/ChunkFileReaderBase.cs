using System.IO;
using AnakinRaW.CommonUtilities;
using PG.StarWarsGame.Files.ChunkFiles.Data;

namespace PG.StarWarsGame.Files.ChunkFiles.Binary.Reader;

public abstract class ChunkFileReaderBase<T>(Stream stream) : DisposableObject, IChunkFileReader<T> where T : IChunkData
{
    protected readonly ChunkReader ChunkReader = new(stream);

    public abstract T Read();

    IChunkData IChunkFileReader.Read()
    {
        return Read();
    }

    protected override void DisposeManagedResources()
    {
        base.DisposeManagedResources();
        ChunkReader.Dispose();
    }
}