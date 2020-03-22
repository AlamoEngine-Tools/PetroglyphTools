namespace PG.Commons.Xml.Values
{
    /// <summary>
    /// An enumeration that tries to replicate the type index of Petroglyph's database map.
    /// </summary>
    public enum XmlValueType
    {
        Boolean = 0,
        /// <summary>
        /// Unknown Value Type - Assumed <c>int</c>
        /// </summary>
        IntType5 = 5,
        /// <summary>
        /// Unknown Value Type - Assumed <c>uint</c>
        /// </summary>
        IntType6 = 6,
        Float = 8,
        /// <summary>
        /// Generally used as multiplier, so double might make more sense than a <c>FloatMultiplier</c>.
        /// </summary>
        Double = 9,
        DynamicEnumValue = 14,
        /// <summary>
        /// Technically only used with integers in the xml, but from this context it makes more sense to be a float.
        /// </summary>
        FloatVector2 = 15,
        FloatVector3 = 16,
        FloatVector4 = 17,
        IntList = 18,
        FloatList = 19,
        RGBA = 22,
        /// <summary>
        /// A reference to another object, presumably by name.
        /// </summary>
        NameReference = 23,
        /// <summary>
        /// A list of references to other objects, presumably by name.
        /// </summary>
        NameReferenceList = 27,
        /// <summary>
        /// A reference to another object, presumably by type.
        /// </summary>
        TypeReference = 29,
        /// <summary>
        /// A list of references to other objects, presumably by type.
        /// </summary>
        TypeReferenceList = 42,
        FloatTupleList = 47,
        IntFloatTupleList = 48,
        ShipNameTextFileList = 54,
        DamageToArmorMod = 62,
        LocalisationToTextureMap = 64,
        HardPointTypeToTextureMap = 65,
        CategoryToIntegerMap = 72,
    }
}
