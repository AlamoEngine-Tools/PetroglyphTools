using System.Collections.Generic;

namespace PG.Commons.Chunk.File
{
    public class ChunksHolder : BigChunk
    {
        public override ChunkDataKind DataKind => ChunkDataKind.Chunks;

        public List<Chunk> Chunks { get; }
    }
}