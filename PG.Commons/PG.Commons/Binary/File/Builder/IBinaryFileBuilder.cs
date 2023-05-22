using PG.Commons.Data.Holder;

namespace PG.Commons.Binary.File.Builder
{
    public interface IBinaryFileBuilder<out TFileToBuild, in TFileHolder>
        where TFileHolder : notnull, IFileHolder
        where TFileToBuild : notnull, IBinaryFile
    { 
        TFileToBuild FromBytes(byte[] byteStream);
        TFileToBuild FromHolder(TFileHolder holder);
    }
}
