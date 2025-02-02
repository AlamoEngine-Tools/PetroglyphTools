// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.StarWarsGame.Files.MEG.Files;

/// <summary>
/// Represents the available file versions of a PG .MEG archive as defined in the
///  <a href="https://modtools.petrolution.net/docs/MegFileFormat"> .MEG file specification</a>.
/// </summary>
public enum MegFileVersion
{
    /// <summary>
    /// .MEG file version 1.
    /// These archives are unencrypted.
    /// Used by Empire at War, Forces of Corruption and Universe at War
    /// </summary>
    V1,
    /// <summary>
    /// .MEG file version 2.
    /// These archives are unencrypted.
    /// Used by Guardians of Graxia.
    /// </summary>
    V2,
    /// <summary>
    /// .MEG file version 3.
    /// These archives may be encrypted.
    /// Used by Grey Goo, 8-Bit Armies or The Great War: Western Front
    /// </summary>
    V3
}