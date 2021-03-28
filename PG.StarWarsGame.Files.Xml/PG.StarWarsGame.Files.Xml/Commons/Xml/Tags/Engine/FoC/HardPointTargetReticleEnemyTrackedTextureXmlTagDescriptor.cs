using PG.Commons.Xml.Values;

namespace PG.StarWarsGame.Files.Xml.Commons.Xml.Tags.Engine.FoC
{
    public sealed class HardPointTargetReticleEnemyTrackedTextureXmlTagDescriptor : AXmlTagDescriptor
    {
        private const string KEY_ID = "HardPoint_Target_Reticle_Enemy_Tracked_Texture";
        private const XmlValueType PG_TYPE = XmlValueType.HardPointTypeToTextureMap;
        private const XmlValueTypeInternal INTERNAL_TYPE = XmlValueTypeInternal.Undefined;
        private const bool IS_SINGLETON = false;

        public override string Id => KEY_ID;
        public override XmlValueType Type => PG_TYPE;
        public override XmlValueTypeInternal TypeInternal => INTERNAL_TYPE;
        public override bool IsSingleton => IS_SINGLETON;
    }
}
