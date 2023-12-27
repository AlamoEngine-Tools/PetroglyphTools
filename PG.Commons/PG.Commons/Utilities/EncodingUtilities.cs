using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace PG.Commons.Utilities;

/// <summary>
/// Provides PG specific extension methods to the <see cref="Encoding"/> type.
/// </summary>
public static partial class EncodingUtilities
{
    /// <summary>
    ///     Gets the number of bytes required for encoding a given number of characters.
    /// </summary>
    /// <remarks>
    ///     This method should <b>only</b> be used while reading a .DAT or .MEG file as it is probably not safe in other use cases
    ///     (given how complex the .NET source code is).
    /// <br/>
    /// <br/>
    ///     Note:
    /// <br/>
    ///     PG binary files are not consistent whether they store the number of characters (.DAT or .MEG)
    ///     or the required bytes (chunked files). Since most strings in PG games are ASCII-encoded
    ///     we do have a 1:1 relation between # char and # bytes, this does not matter most of the time.
    ///     But in DAT Values we use UTF16 (Unicode), which requires 2x more bytes than characters.
    /// <br/>
    /// <br/>
    ///     The assumption made here for this method to work is that, who ever created the binary file, did proper encoding -
    ///     e.g. replacing non ASCII with '?' or other fallback characters. 
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

    /// <summary>
    /// Encodes a string value.
    /// </summary>
    /// <param name="value">The string to encode.</param>
    /// <param name="encoding">The encoding to use.</param>
    /// <returns>The encoded string.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="encoding"/> or <paramref name="value"/>is <see langword="null"/>.</exception>
    public static string EncodeString(this Encoding encoding, string value)
    {
        if (value == null) 
            throw new ArgumentNullException(nameof(value));
        return EncodeString(encoding, value.AsSpan());
    }

    /// <summary>
    /// Encodes a string value.
    /// </summary>
    /// <param name="value">The string to encode.</param>
    /// <param name="maxByteCount">Maximum bytes required for encoding.</param>
    /// <param name="encoding">The encoding to use.</param>
    /// <returns>The encoded string.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="encoding"/> or <paramref name="value"/>is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="maxByteCount"/> is less than actually required.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxByteCount"/> is negative.</exception>
    public static string EncodeString(this Encoding encoding, string value, int maxByteCount)
    {
        if (value == null) 
            throw new ArgumentNullException(nameof(value));

        return EncodeString(encoding, value.AsSpan(), maxByteCount);
    }

    /// <summary>
    /// Encodes a character sequence.
    /// </summary>
    /// <param name="value">The span of characters to encode.</param>
    /// <param name="encoding">The encoding to use.</param>
    /// <returns>The encoded string.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="encoding"/> is <see langword="null"/>.</exception>
    public static string EncodeString(this Encoding encoding, ReadOnlySpan<char> value)
    {
        if (encoding == null) 
            throw new ArgumentNullException(nameof(encoding));

        return EncodeString(encoding, value, encoding.GetMaxByteCount(value.Length));
    }

    /// <summary>
    /// Encodes a character sequence.
    /// </summary>
    /// <param name="value">The span of characters to encode.</param>
    /// <param name="maxByteCount">Maximum bytes required for encoding.</param>
    /// <param name="encoding">The encoding to use.</param>
    /// <returns>The encoded string.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="encoding"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="maxByteCount"/> is less than actually required.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxByteCount"/> is negative.</exception>
    public static unsafe string EncodeString(this Encoding encoding, ReadOnlySpan<char> value, int maxByteCount)
    {
        if (encoding == null)
            throw new ArgumentNullException(nameof(encoding));
        if (maxByteCount < 0)
            throw new ArgumentOutOfRangeException(nameof(maxByteCount), "value must not be negative.");

        var buffer = maxByteCount <= 256 ? stackalloc byte[maxByteCount] : new byte[maxByteCount];
        var stringBytes = encoding.GetBytesReadOnly(value, buffer);
        return encoding.GetString(stringBytes);
    }

    /// <summary>
    /// Encodes into a read-only span of bytes a set of characters from the specified read-only span.
    /// </summary>
    /// <remarks>
    /// The returned read-only span is sliced from <paramref name="inputBuffer"/>.
    /// This means, modifying <paramref name="inputBuffer"/> might also modify the returned read-only span. 
    /// </remarks>
    /// <param name="encoding">The encoding to use.</param>
    /// <param name="value">The span of characters to encode.</param>
    /// <param name="inputBuffer">The byte span to hold the encoded bytes.</param>
    /// <returns>The read-only byte span that holds the encoded bytes.</returns>
    public static ReadOnlySpan<byte> GetBytesReadOnly(this Encoding encoding, ReadOnlySpan<char> value, Span<byte> inputBuffer)
    {
        var pathBytesWritten = encoding.GetBytes(value, inputBuffer);
        return inputBuffer.Slice(0, pathBytesWritten);
    }
}