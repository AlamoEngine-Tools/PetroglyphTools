// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Generic;
using PG.Commons.Files;
using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Files;

/// <summary>
///     The provided holder for Petroglyph's
///     <a href="https://modtools.petrolution.net/docs/MegFileFormat"> .MEG files</a>.
///     *.MEG or Mega files are a proprietary archive type bundling files together in a RAM friendly way.
/// </summary>
public interface IMegFile : IFileHolder<IReadOnlyList<MegFileDataEntry>, MegAlamoFileType>, IReadOnlyCollection<MegFileDataEntry>
{
    /// <summary>
    /// Gets the file version of the MEG file.
    /// </summary>
    public MegFileVersion FileVersion { get; }

    /// <summary>
    /// Gets a copy of the initialization vector (IV) used for encryption. <see langword="null"/> if the file is not encrypted.
    /// </summary>
    byte[]? IV { get; }

    /// <summary>
    /// Gets a copy of the encryption key used for encryption. <see langword="null"/> if the file is not encrypted.
    /// </summary>
    byte[]? Key { get; }


    /// <summary>
    /// Gets a value indicating whether the MEG file is encrypted.
    /// </summary>
    bool HasEncryption { get; }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    public bool Contains(MegFileDataEntry entry);
    
    /// <summary>
    /// Tries to find any <see cref="MegFileDataEntry"/> by matching the provided filename.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="megFileDataEntries"></param>
    /// <returns></returns>
    bool TryGetAllEntriesWithMatchingPattern(string fileName, out IReadOnlyList<MegFileDataEntry> megFileDataEntries);
}