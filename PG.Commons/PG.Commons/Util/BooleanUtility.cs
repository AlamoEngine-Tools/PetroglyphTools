using System;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace PG.Commons.Util
{
    /// <summary>
    /// A convenience class to handle Petroglyph's boolean types.
    /// </summary>
    public static class BooleanUtility
    {
        public enum BoolParseType
        {
            YesNo = 0,
            TrueFalse = 1,
        }
        [NotNull] private const string PETROGLYPH_TRUE = "True";
        [NotNull] private const string PETROGLYPH_YES = "Yes";
        [NotNull] private const string PETROGLYPH_FALSE = "False";
        [NotNull] private const string PETROGLYPH_NO = "No";
        [NotNull] private static readonly string[] PETROGLYPH_TRUE_STRING_TYPES = {PETROGLYPH_YES, PETROGLYPH_TRUE};
        [NotNull] private static readonly string[] PETROGLYPH_FALSE_STRING_TYPES = {PETROGLYPH_NO, PETROGLYPH_FALSE};

        /// <summary>Parses the specified Petroglyph boolean found in Petroglyph games.</summary>
        /// <param name="s">The string as read from the xml value.</param>
        /// <param name="defaultReturn">If the provided string value cannot be parsed, this default will be returned instead.</param>
        /// <returns></returns>
        public static bool Parse(string s, bool defaultReturn = false)
        {
            if (!StringUtility.HasText(s))
            {
                return defaultReturn;
            }
            Debug.Assert(s != null, nameof(s) + " != null");
            return PETROGLYPH_TRUE_STRING_TYPES.Contains(s.Trim(),
                StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>Parses the specified boolean and turns it into a Petroglyph boolean type as used in the xml files for Petroglyph games.</summary>
        /// <param name="boolean">The boolean to parse.</param>
        /// <param name="petroglyphBoolType">The type the boolean should be parsed as.
        /// Defaults to the <see cref="BoolParseType.YesNo"/> type.</param>
        /// <returns>Returns a string that can be used as boolean for xml attributes in Petroglyph games.</returns>
        public static string Parse(bool boolean, BoolParseType petroglyphBoolType = BoolParseType.YesNo)
        {
            return boolean
                    ? PETROGLYPH_TRUE_STRING_TYPES[(int) petroglyphBoolType]
                    : PETROGLYPH_FALSE_STRING_TYPES[(int) petroglyphBoolType];
        }
    }
}
