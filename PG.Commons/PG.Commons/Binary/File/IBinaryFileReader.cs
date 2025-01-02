// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO;

namespace PG.Commons.Binary.File;

/// <summary>
/// A reader that is capable of converting a stream of bytes into a binary model.
/// </summary>
/// <typeparam name="TBinaryModel">The type of the binary model.</typeparam>
public interface IBinaryFileReader<out TBinaryModel> where TBinaryModel : IBinaryFile
{
    /// <summary>
    /// Builds a binary file from a given byte array.
    /// </summary>
    /// <param name="byteStream">The binary data</param>
    /// <returns>The converted binary file.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="byteStream"/> is <see langword="null"/>.</exception>
    /// <exception cref="BinaryCorruptedException"><paramref name="byteStream"/> cannot be converted to <typeparamref name="TBinaryModel"/>.</exception>
    TBinaryModel ReadBinary(Stream byteStream);
}