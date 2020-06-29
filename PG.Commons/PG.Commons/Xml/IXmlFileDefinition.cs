using PG.Commons.Xml.Tags;

namespace PG.Commons.Xml
{
    public interface IXmlFileDefinition
    {
        bool Contains<T>() where T : IXmlTagDefinition;
        T GetByClass<T>() where T : IXmlTagDefinition;
    }
}
