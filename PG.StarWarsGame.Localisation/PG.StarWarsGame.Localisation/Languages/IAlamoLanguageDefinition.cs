// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Globalization;

namespace PG.StarWarsGame.Localisation.Languages;

/// <summary>
///     An interface exposing all relevant data to describe a language to be used in the Alamo Engine.
/// </summary>
public interface IAlamoLanguageDefinition
{
    /// <summary>
    ///     A string that is being used to identify the language of the *.DAT file, e.g. a language identifier
    ///     "english" would produce the file "mastertextfile_english.dat"
    /// </summary>
    string LanguageIdentifier { get; }

    /// <summary>
    ///     The .NET Culture that best describes the language. This culture can be used for spell checking,
    ///     auto-translation between languages, etc.
    /// </summary>
    CultureInfo Culture { get; }
}