namespace PG.StarWarsGame.Files.ChunkFiles.Binary.Metadata;

public readonly struct ChunkMetadata
{
    public readonly int Type;
    public readonly int Size;

    private ChunkMetadata(int type, int size, bool isContainer, bool isMiniChunk)
    {
        Type = type;
        Size = size;
        IsMiniChunk = isMiniChunk;
        IsContainer = isContainer;
    }

    public bool IsContainer { get; }

    public bool IsMiniChunk { get; }

    public static ChunkMetadata FromContainer(int type, int size)
    {
        return new ChunkMetadata(type, size, true, false);
    }

    public static ChunkMetadata FromData(int type, int size, bool isMini = false)
    {
        return new ChunkMetadata(type, size, false, isMini);
    }
}