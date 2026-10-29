// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Xml.Linq;

namespace PG.StarWarsGame.Localisation.IO.Xml
{
    /// <summary>
    /// The names of an eaw-translation XML file, in one of the two shapes it is read and written in.
    /// <see cref="TextEditor"/> is the format the EaW text editor reads and writes, and the one
    /// <c>Data/Config/v1/eaw-translation.xsd</c> describes: no namespace, <c>Key</c>, one
    /// <see cref="TranslationData"/> per key holding a <see cref="Translation"/> per language.
    /// <see cref="Namespaced"/> is this library's own shape: the same elements in the
    /// <c>urn:alamoenginetools:localisation:v1</c> namespace, keyed by <c>key</c>. The text editor rejects it.
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
    public sealed class XmlTranslationFormat
    {
        /// <summary>The text editor's format. Written when the exporter omits the namespace.</summary>
        public static XmlTranslationFormat TextEditor { get; } = new(XNamespace.None, "Key");

        /// <summary>This library's own shape. Written by default.</summary>
        public static XmlTranslationFormat Namespaced { get; } =
            new(XNamespace.Get("urn:alamoenginetools:localisation:v1"), "key");

        private XmlTranslationFormat(XNamespace ns, string key)
        {
            Root = ns + "LocalisationData";
            Localisation = ns + "Localisation";
            TranslationData = ns + "TranslationData";
            Translation = ns + "Translation";
            Key = key;
        }

        /// <summary>The shape <paramref name="document"/> is written in, told by its root's namespace.</summary>
        public static XmlTranslationFormat Of(XDocument document) =>
            document.Root?.Name.Namespace == Namespaced.Root.Namespace ? Namespaced : TextEditor;

        /// <summary>The root element.</summary>
        public XName Root { get; }

        /// <summary>One key's element.</summary>
        public XName Localisation { get; }

        /// <summary>The key's attribute on <see cref="Localisation"/>.</summary>
        public XName Key { get; }

        /// <summary>The element holding a key's translations.</summary>
        public XName TranslationData { get; }

        /// <summary>One language's translation.</summary>
        public XName Translation { get; }

        /// <summary>The language's attribute on <see cref="Translation"/>.</summary>
        public XName Language { get; } = "Language";
    }
}
