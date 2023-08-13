using System;
using System.Buffers;
using System.Diagnostics;
using System.Text;

namespace PG.Commons.Utilities;

/// <summary>
/// Provides primitive helper methods for strings.
/// </summary>
public static class StringUtilities
{
    /// <summary>
    /// Checks that a given string, when converted to bytes, is not longer than the max value of an UInt16.
    /// Throws an <see cref="OverflowException"/> if the string is longer
    /// </summary>
    /// <remarks>
    /// The encoding is necessary information.<br/>
    /// For example consider the string "🤔":<br/>
    ///     .NET string length (# of characters): 2.<br/>
    ///     ASCII required bytes (each char is 1 byte): 2<br/>
    ///     Unicode required bytes (each char is 2 bytes): 4.<br/>
    /// Many PG binaries use size information for processing strings. So if we wanted to use Unicode encoding,
    /// we need the actual byte size, and not what .NET thinks.
    /// <b>However: The games usually use ASCII, or at least one-byte encoding, internally,
    /// which makes makes using Unicode problematic.</b>
    /// </remarks>
    /// <param name="value">The string to validate.</param>
    /// <param name="encoding">The encoding that shall be used to get the string length.</param>
    /// <returns>The actual length of the value in bytes.</returns>
    /// <exception cref="OverflowException">When the string was too long.</exception>
    public static ushort ValidateFileNameByteSizeUInt16(string value, Encoding encoding)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));
        if (encoding == null)
            throw new ArgumentNullException(nameof(encoding));

        var length = encoding.GetByteCount(value);
        try
        {
            return Convert.ToUInt16(length);
        }
        catch (OverflowException)
        {
            throw new OverflowException($"The value {value} is longer that the expected {ushort.MaxValue} characters.");
        }
    }

    /// <summary>
    /// Transforms a string to a given encoding.
    /// </summary>
    /// <param name="value">The string to transform.</param>
    /// <param name="encoding">The encoding to use for transformation.</param>
    /// <returns>The encoded string.</returns>
    public static string EncodeString(string value, Encoding encoding)
    {
        // Overapproximation is OK in this case, since EncodeString(string, int, Encoding)
        // uses the actual byte code during the transformation.
        var byteCount = encoding.GetMaxByteCount(value.Length);
        return EncodeString(value, byteCount, encoding);
    }


    /// <summary>
    /// Transforms a string to a given encoding.
    /// </summary>
    /// <param name="value">The string to transform.</param>
    /// <param name="byteCount">Minimum bytes required for the transformation.</param>
    /// <param name="encoding">The encoding to use for transformation.</param>
    /// <returns>The encoded string.</returns>
#if NETCOREAPP2_1_OR_GREATER
    public static string EncodeString(string value, int byteCount, Encoding encoding)
#else
    public static unsafe string EncodeString(string value, int byteCount, Encoding encoding)
#endif
    {
        if (value == null) 
            throw new ArgumentNullException(nameof(value));
        if (encoding == null) 
            throw new ArgumentNullException(nameof(encoding));

        using var memoryOwner = MemoryPool<byte>.Shared.Rent(byteCount);
        var buffer = memoryOwner.Memory.Span;

#if NETCOREAPP2_1_OR_GREATER
        var bytesWritten = encoding.GetBytes(value.AsSpan(), buffer);
        Debug.Assert(bytesWritten <= byteCount);
        return encoding.GetString(buffer.Slice(0, bytesWritten));
#else
        fixed (char* pFileName = value)
        fixed (byte* pBuffer = buffer)
        {
            var bytesWritten = encoding.GetBytes(pFileName, value.Length, pBuffer, byteCount);
            Debug.Assert(bytesWritten <= byteCount);
            return encoding.GetString(pBuffer, bytesWritten);
        }
#endif
    }
}