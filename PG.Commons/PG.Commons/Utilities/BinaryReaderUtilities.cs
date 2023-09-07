// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.IO;
using System.Text;
using System;
using System.Runtime.CompilerServices;

namespace PG.Commons.Utilities;

/// <summary>
/// Provides PG specific extension methods to the <see cref="Encoding"/> type.
/// </summary>
public static class EncodingUtilities
{
    /// <summary>
    /// Gets the number of bytes required for encoding a given number of characters.
    /// </summary>
    /// <remarks>
    /// This method should <b>only</b> be used while reading a .DAT or .MEG file as it is probably not safe in other use cases
    /// (given how complex the .NET source code is).
    /// <br/>
    /// <br/>
    /// Note:
    /// <br/>
    /// PG binary files are not consistent whether they store the number of characters (.DAT or .MEG)
    /// or the required bytes (chunked files). Since most strings in PG games are ASCII-encoded
    /// we do have a 1:1 relation between # char and # bytes, this does not matter most of the time.
    /// But in DAT Values we use UTF16 (Unicode), which requires 2x more bytes than characters.
    /// <br/>
    /// <br/>
    /// The assumption made here for this method to work is that, who ever created the binary file, did proper encoding -
    /// e.g. replacing non ASCII with '?' or other fallback characters. 
    /// </remarks>
    /// <param name="encoding">The encoding to be used.</param>
    /// <param name="numberOfChars">The number of characters to encode.</param>
    /// <returns>The number of bytes</returns>
    /// <exception cref="ArgumentNullException"><paramref name="encoding"/> is <see langowrd="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="numberOfChars"/> is less than zero.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetByteCountPG(this Encoding encoding, int numberOfChars)
    {
        if (encoding == null) 
            throw new ArgumentNullException(nameof(encoding));
        if (numberOfChars < 0)
            throw new ArgumentOutOfRangeException(nameof(numberOfChars), "Number of chars is less than zero.");

        // Prevent checking for rogue subtypes
        var encodingType = encoding.GetType();

        if (encoding.IsSingleByte && IsOriginalAsciiEncoding(encodingType))
        {
            // 1 : 1 relation for ASCII
            return numberOfChars;
        }
        
        // PG .DAT Values are encoded in UTF-16LE, thus UTF-16BE is not supported
        if (encodingType == typeof(UnicodeEncoding) && !encoding.Equals(Encoding.BigEndianUnicode))
        {
            // UTF-16 has double the bytes than characters. 
            // Shifting is slightly faster than multiplication
            return numberOfChars << 1;
        }

        throw new NotSupportedException($"Encoding {encoding} is currently not supported.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsOriginalAsciiEncoding(Type encodingType)
    {
        var asciiType = typeof(ASCIIEncoding);
        return encodingType == asciiType || 
               (asciiType.IsAssignableFrom(encodingType) && encodingType.Assembly == asciiType.Assembly);
    }
}

/// <summary>
/// Provides useful extension methods to the <see cref="BinaryReader"/> type to read PG binary data.
/// </summary>
public static class BinaryReaderUtilities
{
    /// <summary>
    /// Reads a string from the current stream. This operation requires the string length <b>in bytes</b> to be known beforehand.
    /// <br/>
    /// <br/>
    /// This method can be used to read:
    /// <br/>
    /// zero terminated string from .ALO, .ALA, .TED or .MTD files
    /// <br/>
    /// or
    /// <br/>
    /// read non-zero terminated strings from .MEG or .DAT files.
    /// </summary>
    /// <remarks>
    /// BinaryReader does not restore the file position after an unsuccessful read.
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