using PG.Commons.Data.Files;

namespace PG.Commons.Data.Holder
{
    public interface IFileHolder<out T> where T : IAlamoFileType
    {
        string FilePath { get; }
        string FileName { get; }
        T FileType { get; }
    }
}
