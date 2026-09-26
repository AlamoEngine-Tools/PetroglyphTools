// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.IO;

namespace PG.StarWarsGame.Localisation.IO.Csv
{
    /// <summary>
    /// Imports multi-language translation data from a CSV text reader.
    /// Schema: first column is <c>key</c>, remaining columns are language identifiers.
    /// </summary>
    public interface ICsvTranslationImporter : IMultiLanguageTranslationImporter<TextReader>
    {
    }
}
