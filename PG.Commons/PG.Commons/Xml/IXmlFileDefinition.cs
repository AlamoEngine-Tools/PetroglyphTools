using PG.Commons.Xml.Tags;

namespace PG.Commons.Xml
{
    public interface IXmlFileDefinition
    {
        bool ContainsProperty<T>() where T : IXmlTagDefinition;
        T GetPropertyByClass<T>() where T : IXmlTagDefinition;
    }
}
