// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.StarWarsGame.Files.Binary.File;

namespace PG.StarWarsGame.Files.Binary;

/// <summary>
///     A builder that is capable of converting a generic file model to its binary
///     representation and vice versa.
/// </summary>
/// <typeparam name="TBinaryModel">The type of the binary model.</typeparam>
/// <typeparam name="TFileModel">The type of the file model.</typeparam>
public interface IBinaryConverter<TBinaryModel, TFileModel> where TBinaryModel : IBinaryFile
{
    /// <summary>
    ///     Builds a binary model from the specified file model.
    /// </summary>
    /// <param name="model">The model of the binary file.</param>
    /// <returns>The converted binary file.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="model"/> is <see langword="null"/>.</exception>
    TBinaryModel ModelToBinary(TFileModel model);

    /// <summary>
    ///     Builds a file model from the specified binary model.
    /// </summary>
    /// <param name="binary">The <see cref="IBinaryFile" /> to convert.</param>
    /// <returns>The <typeparamref name="TFileModel"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="binary"/> is <see langword="null"/>.</exception>
    /// <exception cref="BinaryCorruptedException"><paramref name="binary"/> cannot be converted to <typeparamref name="TFileModel"/>.</exception>
    TFileModel BinaryToModel(TBinaryModel binary);
}