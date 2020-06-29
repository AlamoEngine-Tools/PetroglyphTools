using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace PG.Commons.Util
{
    public static class StringUtility
    {
        /// <summary>
        /// Tests whether a given string contains any non-whitespace characters.<br/>
        /// Can be used to run quick checks on form fields.
        /// </summary>
        /// <remarks>This method does <b>not</b> copy the string.</remarks>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool HasText(string s)
        {
            return !IsNullEmptyOrWhiteSpace(s);
        }
        /// <summary>
        /// Tests whether a provided string contains any non-whitespace characters starting from the
        /// provided offset until the end of the string.
        /// </summary>
        /// <example>
        /// IsNullEmptyOrWhiteSpace("test\t\t) == false
        /// IsNullEmptyOrWhiteSpace("test\t\t, 5) == true
        /// </example>
        /// <remarks>This method does <b>not</b> copy the string.</remarks>
        /// <param name="s"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static bool IsNullEmptyOrWhiteSpace(string s, int offset = 0)
        {
            return IsNullEmptyOrWhiteSpace(s, offset, int.MaxValue);
        }

        /// <summary>
        /// Tests whether a provided string contains any non-whitespace characters starting from the
        /// provided offset and checking the next <code>length</code> characters.
        /// </summary>
        /// <remarks>This method does <b>not</b> copy the string.</remarks>
        /// <param name="s"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static bool IsNullEmptyOrWhiteSpace(string s, int offset, int length)
        {
            if (null == s)
            {
                return true;
            }

            length = Math.Min(s.Length, length);
            for (int i = offset; i < length; ++i)
            {
                if (!char.IsWhiteSpace(s[i]))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Splits a string by the given separator, and cleans the contained substrings
        /// by dropping whitespace-only elements and trimming the others.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        [NotNull]
        public static List<string> SplitClean(string s, char separator)
        {
            return SplitStringInternal(s, separator);
        }

        [NotNull]
        public static List<string> ParseSeparatedStringToList(string s, char separator, bool shouldClean = true)
        {
            return SplitStringInternal(s, separator, shouldClean);
        }
        
        [NotNull]
        private static List<string> SplitStringInternal(string s, char separator, bool shouldClean = true)
        {
            List<string> list = new List<string>();
            if (IsNullEmptyOrWhiteSpace(s))
            {
                return list;
            }

            Debug.Assert(s != null, nameof(s) + " != null");
            string[] split = s.Split(separator);
            list.AddRange(from str in split where !IsNullEmptyOrWhiteSpace(str) select shouldClean ? str.Trim() : str);
            return list;
        }
    }
}
