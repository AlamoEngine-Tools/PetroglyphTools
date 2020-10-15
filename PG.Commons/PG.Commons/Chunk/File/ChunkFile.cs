using System;
using System.Collections.Generic;

namespace PG.Commons.Chunk.File
{
    public abstract class ChunkFile : IChunkFile
    {
        public List<Chunk> Roots { get; }

        public byte[] ToBytes()
        {
            throw new NotImplementedException();
        }
    }
}