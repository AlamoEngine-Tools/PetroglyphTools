using JetBrains.Annotations;
using PG.Commons.Data.Holder;

namespace PG.Commons.Binary.File.Builder
{
    public interface IBinaryFileBuilder<out TFileToBuild, in TFileHolder>
        where TFileHolder : IFileHolder
        where TFileToBuild : IBinaryFile
    {
        [NotNull] TFileToBuild FromBytes([NotNull] byte[] byteStream);
        [NotNull] TFileToBuild FromHolder([NotNull] TFileHolder buildAttribute);
    }
}
