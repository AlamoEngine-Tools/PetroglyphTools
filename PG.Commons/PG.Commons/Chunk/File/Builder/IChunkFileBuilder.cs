using PG.Commons.Binary.File.Builder;
using PG.Commons.Data.Holder;

namespace PG.Commons.Chunk.File.Builder
{
    public interface IChunkFileBuilder<out TFileToBuild, in TFileHolder> : IBinaryFileBuilder<TFileToBuild, TFileHolder>
        where TFileToBuild : IChunkFile
        where TFileHolder : IFileHolder
    {
    }
}