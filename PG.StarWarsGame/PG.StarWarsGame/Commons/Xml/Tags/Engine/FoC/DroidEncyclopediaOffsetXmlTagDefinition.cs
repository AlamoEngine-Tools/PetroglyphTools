using PG.Commons.Xml.Values;

namespace PG.StarWarsGame.Commons.Xml.Tags.Engine.FoC
{
    public sealed class DroidEncyclopediaOffsetXmlTagDefinition : AXmlTagDefinition
    {
        private const string KEY_ID = "Droid_Encyclopedia_Offset";
        private const XmlValueType PG_TYPE = XmlValueType.FloatVector2;
        private const XmlValueTypeInternal INTERNAL_TYPE = XmlValueTypeInternal.Undefined;
        private const bool IS_SINGLETON = true;

        public override string Id => KEY_ID;
        public override XmlValueType Type => PG_TYPE;
        public override XmlValueTypeInternal TypeInternal => INTERNAL_TYPE;
        public override bool IsSingleton => IS_SINGLETON;
    }
}