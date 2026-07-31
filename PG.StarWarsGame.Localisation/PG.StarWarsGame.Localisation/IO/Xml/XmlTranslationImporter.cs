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
    /// Default implementation of <see cref="IXmlTranslationImporter"/>.
    /// Reads eaw-translation v1 XML documents.
    /// </summary>
    public sealed class XmlTranslationImporter : IXmlTranslationImporter
    {
        private readonly ILanguageService _languageService;

        /// <summary>Initialises a new instance with the given language service.</summary>
        public XmlTranslationImporter(ILanguageService languageService)
        {
            _languageService = languageService ?? throw new ArgumentNullException(nameof(languageService));
        }

        /// <inheritdoc/>
        public void Import(XDocument source, ITranslationDatabase target)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (target is null) throw new ArgumentNullException(nameof(target));

            var ns = XmlTranslationExporter.Ns;
            var root = source.Root;
            if (root is null) return;

            // An ordered database appends on every SetTranslation, so a multi-language entry has to be
            // opened once and then filled in place; otherwise one key would become one entry per language.
            var ordered = target as IOrderedTranslationDatabase;

            foreach (var locEl in root.Elements(ns + "Localisation"))
            {
                var key = locEl.Attribute("key")?.Value;
                if (key is null) continue;

                var rowIndex = -1;

                foreach (var dataEl in locEl.Elements(ns + "TranslationData"))
                {
                    foreach (var transEl in dataEl.Elements(ns + "Translation"))
                    {
                        var langId = transEl.Attribute("Language")?.Value;
                        var value = transEl.Value;

                        if (langId is null) continue;
                        if (!_languageService.TryGetByIdentifier(langId, out var lang) || lang is null)
                            continue;
                        if (!target.Languages.Contains(lang)) continue;

                        if (ordered is null)
                        {
                            target.SetTranslation(key, lang, value);
                        }
                        else if (rowIndex < 0)
                        {
                            ordered.SetTranslation(key, lang, value);
                            rowIndex = ordered.Count - 1;
                        }
                        else
                        {
                            ordered.SetTranslationAt(rowIndex, lang, value);
                        }
                    }
                }
            }
        }
    }
}
