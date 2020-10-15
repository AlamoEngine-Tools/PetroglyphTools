namespace PG.Commons.Chunk.File
{
    public class MiniChunk : Chunk
    {
        public override ChunkDataKind DataKind => ChunkDataKind.Data;

        public MiniChunkHeader Header { get; }

    }
}