namespace PG.Commons.Chunk.File
{
    public class DataChunk : BigChunk
    {
        public override ChunkDataKind DataKind => ChunkDataKind.Data;

        public byte[] Data { get; }
    }
}