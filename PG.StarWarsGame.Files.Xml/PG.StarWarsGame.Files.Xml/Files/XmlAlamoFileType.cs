namespace PG.StarWarsGame.Files.Xml.Files;

public class XmlAlamoFileType : IAlamoFileType
{
    private const FileType FILE_TYPE = FileType.Text;
    private const string FILE_EXTENSION = "xml";

    public FileType Type => FILE_TYPE;
    public string FileExtension => FILE_EXTENSION;
}
