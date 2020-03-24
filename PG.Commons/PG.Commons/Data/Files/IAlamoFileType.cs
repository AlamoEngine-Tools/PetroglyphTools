namespace PG.Commons.Data.Files
{
    public interface IAlamoFileType
    {
        FileType Type { get; }
        string FileExtension { get; }
    }
}
