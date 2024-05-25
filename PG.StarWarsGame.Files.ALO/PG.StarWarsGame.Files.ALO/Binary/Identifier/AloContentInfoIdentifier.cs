using System.IO;
using PG.Commons.Binary;
using PG.StarWarsGame.Files.ALO.Files;
using PG.StarWarsGame.Files.ChunkFiles.Binary.Metadata;
using PG.StarWarsGame.Files.ChunkFiles.Binary.Reader;

namespace PG.StarWarsGame.Files.ALO.Binary.Identifier;

internal class AloContentInfoIdentifier : IAloContentInfoIdentifier
{
    public AloContentInfo GetContentInfo(Stream stream)
    {
        using var chunkReader = new ChunkReader(stream, true);

        var chunk = chunkReader.ReadChunk();

        switch ((ChunkType)chunk.Type)
        {
            case ChunkType.Skeleton:
            case ChunkType.Mesh:
            case ChunkType.Light:
                return FromModel(chunk.Size, chunkReader);
            case ChunkType.Connections:
                return FromConnection(chunkReader);
            case ChunkType.Particle:
                return new AloContentInfo(AloType.Particle, AloVersion.V1);
            case ChunkType.ParticleUaW:
                return new AloContentInfo(AloType.Particle, AloVersion.V2);
            default:
                throw new BinaryCorruptedException("Unable to get ALO content information.");
        }
    }

    private static AloContentInfo FromConnection(ChunkReader chunkReader)
    {
        var chunk = chunkReader.TryReadChunk();
        while (chunk.HasValue)
        {
            switch ((ChunkType)chunk.Value.Type)
            {
                case ChunkType.ProxyConnection:
                case ChunkType.ObjectConnection:
                case ChunkType.ConnectionCounts:
                    chunkReader.Skip(chunk.Value.Size);
                    break;
                case ChunkType.Dazzle:
                    return new AloContentInfo(AloType.Model, AloVersion.V2);
                default:
                    throw new BinaryCorruptedException("Invalid ALO model.");
            }
            chunk = chunkReader.TryReadChunk();
        }

        return new AloContentInfo(AloType.Model, AloVersion.V1);
    }

    private static AloContentInfo FromModel(int size, ChunkReader chunkReader)
    {
        chunkReader.Skip(size);
        var chunk = chunkReader.TryReadChunk();
        if (chunk is null) 
            throw new BinaryCorruptedException("Unable to get ALO content information.");
        switch ((ChunkType)chunk.Value.Type)
        {
            case ChunkType.Connections:
                return FromConnection(chunkReader);
            case ChunkType.Skeleton:
            case ChunkType.Mesh:
            case ChunkType.Light:
                return FromModel(chunk.Value.Size, chunkReader);
            default:
                throw new BinaryCorruptedException("Invalid ALO model.");
        }
    }
}