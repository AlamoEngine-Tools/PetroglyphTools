// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using PG.Commons.Services.Builder;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

namespace PG.StarWarsGame.Files.MEG.Services.Builder;

/// <summary>
/// Service to create MEG files from local files or other MEG data entries ensuring custom validation and normalization rules.
/// </summary>
public interface IMegBuilder : IFileBuilder<IReadOnlyCollection<MegFileDataEntryBuilderInfo>, MegFileInformation>
{
    /// <summary>
    /// Gets a value indicating whether the <see cref="IMegBuilder"/> normalizes a data entry's path before adding it.
    /// </summary>
    /// <remarks>
    /// Path normalization is performed before encoding.
    /// </remarks>
    bool NormalizesEntryPaths { get; }

    /// <summary>
    /// Gets a value indicating whether the <see cref="IMegBuilder"/> overwrites an already existing data entry with the new version when trying to a data entry
    /// or does not add the new data entry.
    /// </summary>
    /// <remarks>
    /// A duplicate is determined by the data entry's file path.
    /// </remarks>
    bool OverwritesDuplicateEntries { get; }

    /// <summary>
    /// Gets a collection of all data entries which shall be packed to a .MEG file.
    /// </summary>
    IReadOnlyCollection<MegFileDataEntryBuilderInfo> DataEntries { get; }

    /// <summary>
    /// Gets the data entry validator for this <see cref="IMegBuilder"/>.
    /// </summary>
    IBuilderInfoValidator DataEntryValidator { get; }

    /// <summary>
    /// Gets the file information validator for this <see cref="IMegBuilder"/>.
    /// </summary>
    IMegFileInformationValidator MegFileInformationValidator { get; }

    /// <summary>
    /// Gets the data entry path normalizer or <see langword="null"/> if no normalizer is specified.
    /// </summary>
    IMegDataEntryPathNormalizer? DataEntryPathNormalizer { get; }

    /// <summary>
    /// Adds a local file as a data entry to the <see cref="IMegBuilder"/> and returns a status information to indicate whether the entry was successfully added. 
    /// </summary>
    /// <remarks>
    /// The actual data entry's file path might be different to <paramref name="filePathInMeg"/> due to optional normalization and mandatory encoding.
    /// </remarks>
    /// <param name="filePath">The local path to the file which get added as an data entry.</param>
    /// <param name="filePathInMeg">The desired file path of the data entry inside the MEG archive.</param>
    /// <param name="encrypt">Indicates whether the data entry shall be encrypted.</param>
    /// <returns>The result of this operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="filePath"/> or <paramref name="filePathInMeg"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="filePath"/> or <paramref name="filePathInMeg"/> is empty.</exception>
    AddDataEntryToBuilderResult AddFile(string filePath, string filePathInMeg, bool encrypt = false);

    /// <summary>
    /// Adds an existing data entry to the <see cref="IMegBuilder"/> and returns a status information to indicate whether the entry was successfully added. 
    /// </summary>
    /// <remarks>
    /// The actual data entry's file path might be different to <paramref name="overridePathInMeg"/> depending whether this <see cref="IMegBuilder"/> normalizes path before adding, or not.
    /// </remarks>
    /// <param name="entryReference">The existing data entry to add.</param>
    /// <param name="overridePathInMeg">When not <see langword="null"/>, the specified file path is used; otherwise the data entry's file path is used.</param>
    /// <param name="overrideEncrypt">When not <see langword="null"/>, the specified encryption information is used; otherwise the encryption state of the data entry is used.</param>
    /// <returns>The result of this operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="entryReference"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="overridePathInMeg"/> is empty.</exception>
    AddDataEntryToBuilderResult AddEntry(MegDataEntryLocationReference entryReference, string? overridePathInMeg = null,
        bool? overrideEncrypt = null);

    /// <summary>
    /// Removes the first occurrence of a specific object from the <see cref="IMegBuilder"/>.
    /// </summary>
    /// <param name="info">The builder information to remove from the <see cref="IMegBuilder"/>.</param>
    /// <returns>
    /// <see langword="true"/> if item was successfully removed from the <see cref="IMegBuilder"/>; otherwise, <see langword="false"/>.
    /// This method also returns false if item is not found in the original <see cref="IMegBuilder"/>.
    /// </returns>
    bool Remove(MegFileDataEntryBuilderInfo info);

    /// <summary>
    /// Removes all builder information from the <see cref="IMegBuilder"/>.
    /// </summary>
    void Clear();
}