// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Hashing;

namespace PG.StarWarsGame.Files.DAT.Data;

/// <summary>
/// Represents the available entry ordering of a Petroglyph Localized String Table (.DAT).
/// </summary>
public enum DatLayoutKind
{
    /// <summary>
    /// Represents a sorted DAT. This only allows for unique text keys and is expected to
    /// be sorted on the text key's <see cref="Crc32" /> in an ascending manner. Any
    /// MasterText DAT is of this archive type.
    /// </summary>
    OrderedByCrc32,

    /// <summary>
    /// Represents an unsorted .DAT file. This file type allows (and encourages) non-unique text
    /// keys, as this format is used for the CreditsText DAT, where the keys
    /// act as formatting information/instructions rather than queriable keys. The order of the
    /// entries is also equivalent to the order of the credits displayed on screen, so the insert
    /// order of the key-value pairs is important and should be retained.
    /// </summary>
    NotOrdered
}