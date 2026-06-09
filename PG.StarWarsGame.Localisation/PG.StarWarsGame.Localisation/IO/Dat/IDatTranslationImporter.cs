// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Files.DAT.Data;

namespace PG.StarWarsGame.Localisation.IO.Dat
{
    /// <summary>
    /// Imports translation data from a binary DAT model into a translation database.
    /// </summary>
    public interface IDatTranslationImporter : ITranslationImporter<IDatModel>
    {
    }
}
