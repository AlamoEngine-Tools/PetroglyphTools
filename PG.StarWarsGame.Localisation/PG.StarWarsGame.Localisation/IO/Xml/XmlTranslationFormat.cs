// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.StarWarsGame.Localisation.IO.Xml
{
    /// <summary>
    /// The names of the eaw-translation XML format: the format the EaW text editor writes and
    /// <c>Data/Config/v1/eaw-translation.xsd</c> describes. No namespace; one
    /// <see cref="TranslationData"/> per key, holding a <see cref="Translation"/> per language.
    /// </summary>
    /// <example>
    /// <code>
    /// &lt;LocalisationData&gt;
    ///   &lt;Localisation Key="TEXT_HOTH"&gt;
    ///     &lt;TranslationData&gt;
    ///       &lt;Translation Language="ENGLISH"&gt;Assault on Hoth&lt;/Translation&gt;
    ///     &lt;/TranslationData&gt;
    ///   &lt;/Localisation&gt;
    /// &lt;/LocalisationData&gt;
    /// </code>
    /// </example>
    public static class XmlTranslationFormat
    {
        /// <summary>The root element.</summary>
        public const string Root = "LocalisationData";

        /// <summary>One key's element.</summary>
        public const string Localisation = "Localisation";

        /// <summary>The key's attribute on <see cref="Localisation"/>.</summary>
        public const string Key = "Key";

        /// <summary>The element holding a key's translations.</summary>
        public const string TranslationData = "TranslationData";

        /// <summary>One language's translation.</summary>
        public const string Translation = "Translation";

        /// <summary>The language's attribute on <see cref="Translation"/>.</summary>
        public const string Language = "Language";
    }
}
