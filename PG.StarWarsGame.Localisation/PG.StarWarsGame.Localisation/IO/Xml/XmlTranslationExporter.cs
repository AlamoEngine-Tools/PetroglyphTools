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
        /// <inheritdoc/>
        public XDocument Export(ITranslationDatabase source)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));

            var format = XmlTranslationFormat.TextEditor;
            var root = new XElement(format.Root);

            var rows = source is IKeyedTranslationDatabase
                ? source.OrderBy(e => e.Key, StringComparer.Ordinal).AsEnumerable()
                : source.AsEnumerable();

            foreach (var entry in rows)
            {
                var dataEl = new XElement(format.TranslationData);
                foreach (var kv in entry.Translations.OrderBy(kv => kv.Key.LanguageIdentifier))
                {
                    dataEl.Add(new XElement(format.Translation,
                        new XAttribute(format.Language, kv.Key.LanguageIdentifier),
                        kv.Value));
                }

                root.Add(new XElement(format.Localisation,
                    new XAttribute(format.Key, entry.Key), dataEl));
            }

            return new XDocument(root);
        }
    }
}
