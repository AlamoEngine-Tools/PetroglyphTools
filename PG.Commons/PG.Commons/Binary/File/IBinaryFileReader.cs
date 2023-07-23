// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.IO;

namespace PG.Commons.Binary.File;

/// <summary>
/// A reader that is capable of converting a stream of bytes into a <see cref="IBinaryFile"/> representation 
/// </summary>
/// <typeparam name="TBinaryModel">The associated <see cref="IBinaryFile"/> of this builder.</typeparam>
public interface IBinaryFileReader<out TBinaryModel> where TBinaryModel : IBinaryFile
{
    /// <summary>
    /// Builds a binary file from a given byte array.
    /// </summary>
    /// <param name="byteStream">The binary data</param>
    /// <returns>The converted binary file.</returns>
    TBinaryModel ReadBinary(Stream byteStream);
}