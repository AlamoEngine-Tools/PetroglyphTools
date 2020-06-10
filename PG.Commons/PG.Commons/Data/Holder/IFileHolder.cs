using PG.Commons.Data.Files;

namespace PG.Commons.Data.Holder
{
    public interface IFileHolder
    {
        string FilePath { get; }
        string FileName { get; }
    }

    public interface IFileHolder<TContent, out TAlamoFileType>  : IFileHolder where TAlamoFileType : IAlamoFileType
    {
        TAlamoFileType FileType { get; }
        TContent Content { get; set; }
    }
}
