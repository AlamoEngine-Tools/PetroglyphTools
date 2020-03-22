using PG.Commons.Xml.Tags;
using PG.Commons.Xml.Values;

namespace PG.StarWarsGame.Commons.Xml.Tags
{
    public abstract class AXmlTagDefinition : IXmlTagDefinition
    {
        public string Description => LocalizableTexts.AXmlTagDefinition_Description_Default;
        public abstract string Id { get; }
        public abstract XmlValueType Type { get; }
        public abstract XmlValueTypeInternal TypeInternal { get; }
        public abstract bool IsSingleton { get; }
    }
}
