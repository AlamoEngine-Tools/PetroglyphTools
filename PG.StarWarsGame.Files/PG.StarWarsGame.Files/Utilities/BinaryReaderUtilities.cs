// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO;
using System.Text;
using AnakinRaW.CommonUtilities.Extensions;

namespace PG.StarWarsGame.Files.Utilities;

/// <summary>
///     Provides useful extension methods to the <see cref="BinaryReader"/> type to read PG binary data.
/// </summary>
public static class BinaryReaderUtilities
{
    /// <summary>
    ///     Reads a string from the binary reader. This operation requires the string length <b>in bytes</b> to be known beforehand.
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
    /// <param name="length">The number of <b>bytes</b> to read.</param>
    /// <param name="encoding">The encoding to produce the string.</param>
    /// <param name="isZeroTerminated">When set to <see langword="true"/>, the resulting string is truncated to the first found null-terminator ('\0'). Default is <see langword="false"/>.</param>
    /// <returns>The string being read.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="reader"/> or <paramref name="encoding"/> is <see langword="null"/>.</exception>
    /// <exception cref="EndOfStreamException">The number of bytes read, mismatches the expected number of bytes.</exception>
    /// <exception cref="IOException">An I/O error occurred.</exception>
    /// <exception cref="IOException"><paramref name="isZeroTerminated"/> is <see langowrd="true"/> but the string read did not contain a null-terminator.</exception>
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
        // This is the only perf optimization I could get (~ half in runtime and allocations)
        if (length <= 256)
        {
            Span<byte> buffer = stackalloc byte[length];
            var br = reader.Read(buffer);
            if (br != length)
                throw new EndOfStreamException("The number of bytes read, mismatched the expected number of bytes.");
            result = encoding.GetString(buffer);
        }
#endif

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (result is null)
        {
            var bytes = reader.ReadBytes(length);
            if (bytes.Length != length)
                throw new EndOfStreamException("The number of bytes read, mismatched the expected number of bytes.");
            result = encoding.GetString(bytes.AsSpan());
        }

        if (!isZeroTerminated) 
            return result;

        var firstZero = result.IndexOf('\0');
        if (firstZero == -1)
            throw new IOException("The string is not zero-terminated.");
        
        return result.Substring(0, firstZero);
    }
}