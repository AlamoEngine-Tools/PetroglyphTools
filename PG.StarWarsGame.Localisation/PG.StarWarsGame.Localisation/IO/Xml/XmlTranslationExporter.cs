// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Linq;
using System.Xml.Linq;
using PG.StarWarsGame.Localisation.Data;
using PG.StarWarsGame.Localisation.Services;

namespace PG.StarWarsGame.Localisation.IO.Xml
{
    /// <summary>
    /// Default implementation of <see cref="IXmlTranslationExporter"/>.
    /// Produces an eaw-translation v1 XML document.
    /// </summary>
    public sealed class XmlTranslationExporter : IXmlTranslationExporter
    {
        internal static readonly XNamespace Ns = "http://www.example.org/eaw-translation/";

        /// <inheritdoc/>
        public XDocument Export(ITranslationDatabase source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));

            var root = new XElement(Ns + "LocalisationData");

            var rows = source is IKeyedTranslationDatabase
                ? source.OrderBy(e => e.Key, StringComparer.Ordinal).AsEnumerable()
                : source.AsEnumerable();

            foreach (var entry in rows)
            {
                var locEl = new XElement(Ns + "Localisation",
                    new XAttribute("key", entry.Key));

                foreach (var kv in entry.Translations.OrderBy(kv => kv.Key.LanguageIdentifier))
                {
                    locEl.Add(new XElement(Ns + "TranslationData",
                        new XElement(Ns + "Translation",
                            new XAttribute("Language", kv.Key.LanguageIdentifier),
                            kv.Value)));
                }

                root.Add(locEl);
            }

            return new XDocument(root);
        }
    }
}
