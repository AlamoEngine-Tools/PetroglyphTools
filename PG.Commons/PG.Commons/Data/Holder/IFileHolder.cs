using PG.Commons.Data.Files;

namespace PG.Commons.Data.Holder
{
    public interface IFileHolder<TContent, out TAlamoFileType> where TAlamoFileType : IAlamoFileType
    {
        string FilePath { get; }
        string FileName { get; }
        TAlamoFileType FileType { get; }
        TContent Content { get; set; }
    }
}
