// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace PG.Core.Localisation
{
    /// <summary>
    /// All officially supported languages of the Alamo Engine as confirmed by Petroglyph.
    /// </summary>
    public enum OfficialLanguage
    {
        /// <summary>
        /// The language definition for the Chinese language.
        /// </summary>
        Chinese,

        /// <summary>
        /// The language definition for the English (US) language.
        /// This language is the default game and development language of all Alamo Engine games.
        /// </summary>
        English,

        /// <summary>
        /// The language definition for the French language.
        /// </summary>
        French,

        /// <summary>
        /// The language definition for the German language.
        /// </summary>
        German,

        /// <summary>
        /// The language definition for the Italian language.
        /// </summary>
        Italian,

        /// <summary>
        /// The language definition for the Japanese language.
        /// </summary>
        Japanese,

        /// <summary>
        /// The language definition for the Korean language.
        /// </summary>
        Korean,

        /// <summary>
        /// The language definition for the Polish language.
        /// </summary>
        Polish,

        /// <summary>
        /// The language definition for the Russian language.
        /// </summary>
        Russian,

        /// <summary>
        /// The language definition for the Spanish language.
        /// </summary>
        Spanish,

        /// <summary>
        /// The language definition for the Thai language.
        /// </summary>
        Thai
    }

    /// <summary>
    /// Extension class containing convenient methods for use with the <see cref="OfficialLanguage"/> enum.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class OfficialLanguageExtension
    {
        /// <summary>
        /// Turns a given enum value into a recognised string identifier for use with the Alamo Engine.
        /// </summary>
        /// <param name="enumValue">The extension base type.</param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        public static string ToAlamoLanguageIdentifierString(this OfficialLanguage enumValue)
        {
            return enumValue.ToString().ToUpper();
        }

        /// <summary>
        /// Returns the appropriate <see cref="CultureInfo"/> for a given Alamo Language.
        /// </summary>
        /// <param name="enumValue">The extension base type.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static CultureInfo ToCultureInfo(this OfficialLanguage enumValue)
        {
            return enumValue switch
            {
                OfficialLanguage.Chinese => CultureInfo.GetCultureInfo("zh-CN"),
                OfficialLanguage.English => CultureInfo.GetCultureInfo("en-US"),
                OfficialLanguage.French => CultureInfo.GetCultureInfo("fr-FR"),
                OfficialLanguage.German => CultureInfo.GetCultureInfo("de-DE"),
                OfficialLanguage.Italian => CultureInfo.GetCultureInfo("it-IT"),
                OfficialLanguage.Japanese => CultureInfo.GetCultureInfo("ja"),
                OfficialLanguage.Korean => CultureInfo.GetCultureInfo("ko"),
                OfficialLanguage.Polish => CultureInfo.GetCultureInfo("pl"),
                OfficialLanguage.Russian => CultureInfo.GetCultureInfo("ru"),
                OfficialLanguage.Spanish => CultureInfo.GetCultureInfo("es-ES"),
                OfficialLanguage.Thai => CultureInfo.GetCultureInfo("th"),
                _ => throw new ArgumentOutOfRangeException(nameof(enumValue), enumValue, null)
            };
        }

        /// <summary>
        /// Tries to parse a <see cref="CultureInfo"/> to a <see cref="OfficialLanguage"/>.
        /// </summary>
        /// <param name="cultureInfo">The culture info to parse.</param>
        /// <param name="ol">The parsed language or the default language <see cref="OfficialLanguage.English"/></param>
        /// <returns></returns>
        [ExcludeFromCodeCoverage]
        public static bool TryGetFromCultureInfo(CultureInfo cultureInfo,
            out OfficialLanguage ol)
        {
            foreach (OfficialLanguage officialLanguage in Enum.GetValues(typeof(OfficialLanguage)))
            {
                if (!cultureInfo.Equals(officialLanguage.ToCultureInfo()))
                {
                    continue;
                }

                ol = officialLanguage;
                return true;
            }

            ol = OfficialLanguage.English;
            return false;
        }

        /// <summary>
        /// Tries to parse a string into a <see cref="OfficialLanguage"/>.
        /// </summary>
        /// <param name="s">The string to parse.</param>
        /// <param name="ol">The parsed value.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        [ExcludeFromCodeCoverage]
        public static bool TryParse(string s,
            out OfficialLanguage ol)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentException("No string to parse.", nameof(s));
            }

            s = s.Trim();
            foreach (OfficialLanguage officialLanguage in Enum.GetValues(typeof(OfficialLanguage)))
            {
                if (!s.Equals(officialLanguage.ToAlamoLanguageIdentifierString()))
                {
                    continue;
                }

                ol = officialLanguage;
                return true;
            }

            ol = OfficialLanguage.English;
            return false;
        }
    }
}
