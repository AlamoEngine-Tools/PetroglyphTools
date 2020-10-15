using System;
using PG.Commons.Chunk.File.Builder;
using PG.StarWarsGame.Files.TED.Holder;

namespace PG.StarWarsGame.Files.TED.Chunk
{
    internal sealed class MapBuilder : IChunkFileBuilder<IMapChunkFile, TedFileHolder>
    {
        public IMapChunkFile FromBytes(byte[] byteStream)
        {
            throw new NotImplementedException();
        }

        public IMapChunkFile FromHolder(TedFileHolder holder)
        {
            throw new NotImplementedException();
        }
    }
}
