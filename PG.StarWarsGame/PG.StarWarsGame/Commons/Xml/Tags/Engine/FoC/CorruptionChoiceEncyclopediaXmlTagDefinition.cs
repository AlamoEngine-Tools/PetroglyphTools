using PG.Commons.Xml.Values;

namespace PG.StarWarsGame.Commons.Xml.Tags.Engine.FoC
{
    public sealed class CorruptionChoiceEncyclopediaXmlTagDefinition : AXmlTagDefinition
    {
        private const string KEY_ID = "Corruption_Choice_Encyclopedia";
        private const XmlValueType PG_TYPE = XmlValueType.TypeReferenceList;
        private const XmlValueTypeInternal INTERNAL_TYPE = XmlValueTypeInternal.Undefined;
        private const bool IS_SINGLETON = true;

        public override string Id => KEY_ID;
        public override XmlValueType Type => PG_TYPE;
        public override XmlValueTypeInternal TypeInternal => INTERNAL_TYPE;
        public override bool IsSingleton => IS_SINGLETON;
    }
}