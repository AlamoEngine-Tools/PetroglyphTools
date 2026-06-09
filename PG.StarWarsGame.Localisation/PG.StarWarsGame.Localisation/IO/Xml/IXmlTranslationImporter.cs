// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Xml.Linq;

namespace PG.StarWarsGame.Localisation.IO.Xml
{
    /// <summary>
    /// Imports multi-language translation data from an eaw-translation XML document.
    /// </summary>
    public interface IXmlTranslationImporter : IMultiLanguageTranslationImporter<XDocument>
    {
    }
}
