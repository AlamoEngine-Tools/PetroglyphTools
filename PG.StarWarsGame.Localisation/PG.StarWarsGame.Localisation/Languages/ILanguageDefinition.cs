// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Globalization;

namespace PG.StarWarsGame.Localisation.Languages
{
    public interface ILanguageDefinition
    {
        string LanguageIdentifier { get; }
        CultureInfo Culture { get; }
    }
}
