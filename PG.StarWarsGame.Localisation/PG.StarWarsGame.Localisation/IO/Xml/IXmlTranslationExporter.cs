// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Xml.Linq;
using PG.StarWarsGame.Localisation.Data;

namespace PG.StarWarsGame.Localisation.IO.Xml
{
    /// <summary>
    /// Exports all languages from a translation database to an eaw-translation XML document.
    /// <see cref="IMultiLanguageTranslationExporter{TResult}.Export"/> writes the
    /// <see cref="XmlTranslationFormat.Namespaced"/> shape.
    /// </summary>
    public interface IXmlTranslationExporter : IMultiLanguageTranslationExporter<XDocument>
    {
        /// <summary>
        /// Exports <paramref name="source"/>, in the <see cref="XmlTranslationFormat.TextEditor"/> shape when
        /// <paramref name="omitNamespace"/> is set and in the <see cref="XmlTranslationFormat.Namespaced"/> one
        /// otherwise. Only a file without the namespace opens in the EaW text editor.
        /// </summary>
        XDocument Export(ITranslationDatabase source, bool omitNamespace);
    }
}
