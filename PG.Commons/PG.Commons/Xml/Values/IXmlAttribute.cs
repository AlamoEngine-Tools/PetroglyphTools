using PG.Commons.Xml.Tags;

namespace PG.Commons.Xml.Values
{
    public interface IXmlAttribute<T>
    {
        IXmlTagDefinition Definition { get; }
        T GetValue();
        void SetValue(T value);
    }
}
