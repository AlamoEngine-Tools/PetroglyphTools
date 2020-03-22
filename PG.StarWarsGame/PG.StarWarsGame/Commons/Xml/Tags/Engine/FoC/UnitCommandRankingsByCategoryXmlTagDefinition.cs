using PG.Commons.Xml.Values;

namespace PG.StarWarsGame.Commons.Xml.Tags.Engine.FoC
{
    public sealed class UnitCommandRankingsByCategoryXmlTagDefinition : AXmlTagDefinition
    {
        private const string KEY_ID = "Unit_Command_Rankings_By_Category";
        private const XmlValueType PG_TYPE = XmlValueType.NameReferenceList;
        private const XmlValueTypeInternal INTERNAL_TYPE = XmlValueTypeInternal.Undefined;
        private const bool IS_SINGLETON = true;

        public override string Id => KEY_ID;
        public override XmlValueType Type => PG_TYPE;
        public override XmlValueTypeInternal TypeInternal => INTERNAL_TYPE;
        public override bool IsSingleton => IS_SINGLETON;
    }
}