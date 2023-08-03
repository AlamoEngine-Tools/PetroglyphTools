// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace PG.Commons.Binary.File;

/// <summary>
///     A binary file writer to write a <see cref="IBinaryFile" /> to disc.
/// </summary>
/// <typeparam name="TBinaryModel"></typeparam>
public interface IBinaryFileWriter<in TBinaryModel> where TBinaryModel : IBinaryFile
{
    /// <summary>
    ///     Basic write function.
    /// </summary>
    /// <param name="absoluteTargetFilePath">The path to write the file to.</param>
    /// <param name="model">The model to write to disc.</param>
    void WriteBinary(string absoluteTargetFilePath, TBinaryModel model);
}