// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.StarWarsGame.Localisation.IO.Csv
{
    /// <summary>
    /// Exports all languages from a translation database to a CSV string.
    /// Schema: first column is <c>key</c>, remaining columns are language identifiers.
    /// </summary>
    public interface ICsvTranslationExporter : IMultiLanguageTranslationExporter<string>
    {
    }
}
