// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;

namespace PG.StarWarsGame.Files.MEG.Binary.Validation;

/// <summary>
/// Defines a mechanism for validating MEG files on a binary level.
/// </summary>
/// <typeparam name="TMetadata">The type of metadata to validate.</typeparam>
internal interface IMegBinaryValidator<in TMetadata> where TMetadata : IMegFileMetadata
{
    /// <summary>
    /// Validates the integrity and correctness of the specified MEG file metadata against the provided binary data
    /// and throws an <see cref="BinaryCorruptedException"/> if the file is invalid.
    /// </summary>
    /// <param name="metadata">The metadata of the MEG file to validate.</param>
    /// <param name="actualMetadataSize">The actual size of the metadata in bytes as read from the binary data.</param>
    /// <param name="actualFileSize">The actual size of the MEG file in bytes.</param>
    /// <exception cref="ArgumentNullException"><paramref name="metadata"/> is <see langword="null"/>.</exception>
    /// <exception cref="BinaryCorruptedException">The MEG file is invalid.</exception>
    void Validate(TMetadata metadata, long actualMetadataSize, long actualFileSize);
}