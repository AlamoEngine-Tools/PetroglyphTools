// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.IO;

namespace PG.StarWarsGame.Localisation.IO.Properties
{
    /// <summary>
    /// Imports single-language translation data from a Java-style .properties text reader.
    /// Format: <c>KEY=Value</c>, lines beginning with <c>#</c> are comments.
    /// </summary>
    public interface IPropertiesTranslationImporter : ITranslationImporter<TextReader>
    {
    }
}
