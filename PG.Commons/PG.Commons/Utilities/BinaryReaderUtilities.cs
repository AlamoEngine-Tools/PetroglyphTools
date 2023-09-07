// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.IO;
using System.Text;
using System;

namespace PG.Commons.Utilities;

/// <summary>
///     Provides useful extension methods to the <see cref="BinaryReader"/> type to read PG binary data.
/// </summary>
public static class BinaryReaderUtilities
{
    /// <summary>
    ///     Reads a string from the current stream. This operation requires the string length <b>in bytes</b> to be known beforehand.
    /// <br/>
    /// <br/>
    ///     This method can be used to read:
    /// <br/>
    ///     zero terminated string from .ALO, .ALA, .TED or .MTD files
    /// <br/>
    ///     or
    /// <br/>
    ///     read non-zero terminated strings from .MEG or .DAT files.
    /// </summary>
    /// <remarks>
    ///     BinaryReader does not restore the file position after an unsuccessful read.
    /// </remarks>
    /// <param name="reader">The binary reader instance.</param>
    /// <param name="length">The length in <b>bytes</b>.</param>
    /// <param name="encoding">The encoding used to produce the string.</param>
    /// <param name="isZeroTerminated">When set to <see langword="true"/>, the any possible null terminators ('\0') are removed from the end of the result. Default is <see langword="false"/>.</param>
    /// <returns>The string being read.</returns>
    /// <exception cref="ArgumentNullException">A argument is <see langword="null"/>.</exception>
    /// <exception cref="IndexOutOfRangeException">The number of bytes read, mismatched the expected number of bytes.</exception>
    public static string ReadString(this BinaryReader reader, int length, Encoding encoding, bool isZeroTerminated = false)
    {
        if (reader is null) 
            throw new ArgumentNullException(nameof(reader));
        if (encoding is null) 
            throw new ArgumentNullException(nameof(encoding));

        if (length == 0)
            return string.Empty;

        string result = null!;

#if NETSTANDARD2_1_OR_GREATER || NET
        // This is the only perf optimization i could get (~ half in runtime and allocations)
        if (length <= 256)
        {
            Span<byte> buffer = stackalloc byte[length];
            var br = reader.Read(buffer);
            if (br != length)
                throw new IndexOutOfRangeException("The number of bytes read, mismatched the expected number of bytes.");
            result = encoding.GetString(buffer);
        }
#endif

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (result is null)
        {
            var bytes = reader.ReadBytes(length);
            if (bytes.Length != length)
                throw new IndexOutOfRangeException("The number of bytes read, mismatched the expected number of bytes.");
            result = encoding.GetString(bytes);
        }

        if (isZeroTerminated)
            result = result.TrimEnd('\0');

        return result;
    }
}