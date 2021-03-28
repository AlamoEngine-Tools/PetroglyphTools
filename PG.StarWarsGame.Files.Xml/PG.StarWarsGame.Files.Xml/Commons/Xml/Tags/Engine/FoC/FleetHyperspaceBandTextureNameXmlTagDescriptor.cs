using PG.Commons.Xml.Values;

namespace PG.StarWarsGame.Files.Xml.Commons.Xml.Tags.Engine.FoC
{
    public sealed class FleetHyperspaceBandTextureNameXmlTagDescriptor : AXmlTagDescriptor
    {
        private const string KEY_ID = "Fleet_Hyperspace_Band_Texture_Name";
        private const XmlValueType PG_TYPE = XmlValueType.NameReference;
        private const XmlValueTypeInternal INTERNAL_TYPE = XmlValueTypeInternal.Undefined;
        private const bool IS_SINGLETON = true;

        public override string Id => KEY_ID;
        public override XmlValueType Type => PG_TYPE;
        public override XmlValueTypeInternal TypeInternal => INTERNAL_TYPE;
        public override bool IsSingleton => IS_SINGLETON;
    }
}
