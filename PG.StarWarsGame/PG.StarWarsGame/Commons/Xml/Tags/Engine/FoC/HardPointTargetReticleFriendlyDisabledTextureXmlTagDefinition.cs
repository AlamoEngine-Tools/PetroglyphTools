using PG.Commons.Xml.Values;

namespace PG.StarWarsGame.Commons.Xml.Tags.Engine.FoC
{
    public sealed class HardPointTargetReticleFriendlyDisabledTextureXmlTagDefinition : AXmlTagDefinition
    {
        private const string KEY_ID = "HardPoint_Target_Reticle_Friendly_Disabled_Texture";
        private const XmlValueType PG_TYPE = XmlValueType.HardPointTypeToTextureMap;
        private const XmlValueTypeInternal INTERNAL_TYPE = XmlValueTypeInternal.Undefined;
        private const bool IS_SINGLETON = false;

        public override string Id => KEY_ID;
        public override XmlValueType Type => PG_TYPE;
        public override XmlValueTypeInternal TypeInternal => INTERNAL_TYPE;
        public override bool IsSingleton => IS_SINGLETON;
    }
}