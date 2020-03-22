using PG.Commons.Xml.Values;

namespace PG.Commons.Xml.Tags
{
    public interface IXmlTagDefinition
    {
        string Description { get; }
        string Id { get; }
        XmlValueType Type { get; }
        XmlValueTypeInternal TypeInternal { get; }
        bool IsSingleton { get; }
    }
}
