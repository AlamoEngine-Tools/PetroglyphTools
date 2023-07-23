using System.Xml.Linq;

namespace PG.StarWarsGame.Services;

public interface IXmlFileBaseService
{
    XDocument LoadFlat(string filePath);
    void StoreFlat(XDocument xDocument, string filePath);
}