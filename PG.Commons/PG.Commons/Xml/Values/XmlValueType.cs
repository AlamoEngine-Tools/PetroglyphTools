// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.Commons.Xml.Values
{
    /// <summary>
    /// An enumeration that tries to replicate the type index of Petroglyph's database map.
    /// </summary>
    public enum XmlValueType
    {
        /// <summary>
        /// A basic boolean flag, parsed from either Yes/No or true/false.
        /// </summary>
        Boolean = 0,
        /// <summary>
        /// Unknown Value Type - Assumed <c>int</c>
        /// </summary>
        IntType5 = 5,
        /// <summary>
        /// Unknown Value Type - Assumed <c>uint</c>
        /// </summary>
        IntType6 = 6,
        /// <summary>
        /// Basic floatpoint number.
        /// </summary>
        Float = 8,
        /// <summary>
        /// Generally used as multiplier, so double might make more sense than a <c>FloatMultiplier</c>.
        /// </summary>
        Double = 9,
        /// <summary>
        /// A bitmask enum value. The name defined by the xml tag of the enum definition, the value indicating the bit flags.  
        /// </summary>
        DynamicEnumValue = 14,
        /// <summary>
        /// A floatpoint 2D vector.
        /// Technically only used with integers in the xml, but from this context it makes more sense to be a float.
        /// </summary>
        FloatVector2 = 15,
        /// <summary>
        /// A floatpoint 3D vector.
        /// </summary>
        FloatVector3 = 16,
        /// <summary>
        /// A floatpoint 4D vector.
        /// </summary>
        FloatVector4 = 17,
        /// <summary>
        /// A list of integers.
        /// </summary>
        IntList = 18,
        /// <summary>
        /// A list of floatpoint numbers.
        /// </summary>
        FloatList = 19,
        /// <summary>
        /// A 4-tuple representing an RGBA value. 
        /// </summary>
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
        /// <summary>
        /// A list of floatpoint number tuples.
        /// </summary>
        FloatTupleList = 47,
        /// <summary>
        /// A list consisting of Tuple&lt;int, float&gt; tuples.
        /// </summary>
        IntFloatTupleList = 48,
        /// <summary>
        /// A map of game object type to a relative file path that stores possible names for the game object type.
        /// </summary>
        ShipNameTextFileList = 54,
        /// <summary>
        /// A damage to armour modification definition, consisting of Damage type, armour type and multiplier. 
        /// </summary>
        DamageToArmorMod = 62,
        /// <summary>
        /// Maps an available language to a texture (e.g. localised splash screens).
        /// </summary>
        LocalisationToTextureMap = 64,
        /// <summary>
        /// Maps a hardpoint type to a texture.
        /// </summary>
        HardPointTypeToTextureMap = 65,
        /// <summary>
        /// Maps a category to an integer i.e. command ranking.
        /// </summary>
        CategoryToIntegerMap = 72,
    }
}
