// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Linq;
using System.Text;
using PG.StarWarsGame.Localisation.Data;
using PG.StarWarsGame.Localisation.Languages;

namespace PG.StarWarsGame.Localisation.IO.Properties
{
    /// <summary>
    /// Default implementation of <see cref="IPropertiesTranslationExporter"/>.
    /// Produces <c>KEY=Value</c> lines, one per entry.
    /// </summary>
    public sealed class PropertiesTranslationExporter : IPropertiesTranslationExporter
    {
        /// <inheritdoc/>
        public string Export(ITranslationDatabase source, IAlamoLanguageDefinition language)
        {
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (language is null) throw new ArgumentNullException(nameof(language));

            var rows = source is IKeyedTranslationDatabase
                ? source.OrderBy(e => e.Key, StringComparer.Ordinal).AsEnumerable()
                : source.AsEnumerable();

            var sb = new StringBuilder();
            foreach (var entry in rows)
            {
                if (entry.TryGetTranslation(language, out var value) && value is not null)
                {
                    sb.Append(EscapeKey(entry.Key));
                    sb.Append('=');
                    sb.AppendLine(value);
                }
            }
            return sb.ToString();
        }

        private static string EscapeKey(string key) =>
            key.Replace("=", "\\=").Replace(":", "\\:").Replace("#", "\\#");
    }
}
