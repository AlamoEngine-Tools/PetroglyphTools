using System.Collections.Generic;
using PG.Commons.Binary.File;

namespace PG.Commons.Chunk.File
{
    public interface IChunkFile : IBinaryFile
    {
        public List<Chunk> Roots { get; }
    }
}