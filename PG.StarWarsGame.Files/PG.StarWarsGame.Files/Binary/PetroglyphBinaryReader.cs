using System;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using PG.Commons.Utilities;
#if NETSTANDARD2_0
using AnakinRaW.CommonUtilities.Extensions;
#endif

namespace PG.StarWarsGame.Files.Binary;

/// <summary>
/// Reads primitive data types from Petroglyph binary files with dedicated support for various string formats. 
/// </summary>
public sealed class PetroglyphBinaryReader : BinaryReader
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PetroglyphBinaryReader"/> based on the specified stream and optionally leaves the stream open.
    /// </summary>
    /// <param name="input">The input stream.</param>
    /// <param name="leaveOpen"><see langword="true"/> to leave the stream open after the BinaryReader object is disposed; otherwise, <see langword="false"/>.</param>
    public PetroglyphBinaryReader(Stream input, bool leaveOpen) : base(input, InvalidEncoding.Instance, leaveOpen)
    {
        // We use a special, invalid encoding here to ensure that the encoding property
        // of System.IO.BinaryReader is not used and throws an InvalidOperationException.
    }

    /// <summary>
    /// Reads a string from the current stream using the specified encoding and number of characters.
    /// <br/>
    /// <br/>
    /// This method can be used to read:
    /// <br/>
    /// zero-terminated strings from .ALO, .ALA, .TED or .MTD files
    /// <br/>
    /// or
    /// <br/>
    /// read non-zero-terminated strings from .MEG or .DAT files.
    /// </summary>
    /// <remarks>
    /// <see cref="PetroglyphBinaryReader"/> does not restore the file position after an unsuccessful read.
    /// </remarks>
    /// <param name="numberOfChars">The number of characters to read.</param>
    /// <param name="encoding">The encoding to produce the string.</param>
    /// <param name="isZeroTerminated">When set to <see langword="true"/>, the resulting string is truncated to the first found null-terminator ('\0'). Default is <see langword="false"/>.</param>
    /// <returns>The string being read.</returns>
    /// <exception cref="EndOfStreamException">The number of bytes read, mismatches the expected number of bytes.</exception>
    /// <exception cref="IOException">An I/O error occurred.</exception>
    /// <exception cref="IOException"><paramref name="isZeroTerminated"/> is <see langowrd="true"/> but the string read did not contain a null-terminator.</exception>
    /// <exception cref="NotSupportedException"><paramref name="encoding"/> is not supported.</exception>
    public string ReadString(Encoding encoding, int numberOfChars, bool isZeroTerminated = false)
    {
        if (encoding is null)
            throw new ArgumentNullException(nameof(encoding));

        if (numberOfChars == 0)
            return string.Empty;

        char[]? pooledArray = null;

        try
        {
            var buffer = numberOfChars > 265
                ? pooledArray = ArrayPool<char>.Shared.Rent(numberOfChars)
                : stackalloc char[numberOfChars];
            var n = ReadString(buffer, encoding, numberOfChars, isZeroTerminated);
            return buffer.Slice(0, n).ToString();
        }
        finally
        {
            if (pooledArray is not null)
                ArrayPool<char>.Shared.Return(pooledArray);
        }
    }

    /// <summary>
    /// Reads, from the current stream, a string of the specified length and writes it in the provided character span
    /// and advances the current position in accordance with the <see cref="Encoding"/> used and the specific character being read from the stream.
    /// <br/>
    /// <br/>
    /// This method can be used to read:
    /// <br/>
    /// zero-terminated strings from .ALO, .ALA, .TED or .MTD files
    /// <br/>
    /// or
    /// <br/>
    /// read non-zero terminated strings from .MEG or .DAT files.
    /// </summary>
    /// <remarks>
    /// <see cref="PetroglyphBinaryReader"/> does not restore the file position after an unsuccessful read.
    /// </remarks>
    /// <param name="destination">The character span to write the string into.</param>
    /// <param name="encoding">The encoding to produce the string.</param>
    /// <param name="numberOfChars">The number of characters to read.</param>
    /// <param name="isZeroTerminated">When set to <see langword="true"/>, the resulting string is truncated to the first found null-terminator ('\0'). Default is <see langword="false"/>.</param>
    /// <returns>
    /// The total number of characters read into the buffer.
    /// This might be less than <paramref name="numberOfChars"/> if <paramref name="isZeroTerminated"/> is <see langword="true"/>
    /// and the read string contained multiple zero-terminators.
    /// </returns>
    /// <exception cref="EndOfStreamException">The number of bytes read, mismatches the expected number of bytes.</exception>
    /// <exception cref="ArgumentException"><paramref name="destination"/> does not have enough capacity to accommodate the resulting characters.</exception>
    /// <exception cref="IOException">An I/O error occurred.</exception>
    /// <exception cref="IOException"><paramref name="isZeroTerminated"/> is <see langowrd="true"/> but the string read did not contain a null-terminator.</exception>
    /// <exception cref="NotSupportedException"><paramref name="encoding"/> is not supported.</exception>
    public int ReadString(Span<char> destination, Encoding encoding, int numberOfChars, bool isZeroTerminated = false)
    {
        if (encoding is null)
            throw new ArgumentNullException(nameof(encoding));

        if (numberOfChars == 0)
            return 0;

        var length = encoding.GetByteCountPG(numberOfChars);

        scoped Span<byte> buffer;

        if (length <= 256)
        {
            buffer = stackalloc byte[length];
            var n = Read(buffer);
            buffer = buffer.Slice(0, n);
        }
        else
            buffer = ReadBytes(length);

        if (buffer.Length != length)
            throw new EndOfStreamException("The number of bytes read, mismatched the expected number of bytes.");

        var actualLength = encoding.GetChars(buffer, destination);
        Debug.Assert(actualLength == numberOfChars);

        if (!isZeroTerminated)
            return actualLength;

        var firstZero = destination.Slice(0, actualLength).IndexOf('\0');
        if (firstZero == -1)
            throw new IOException("The string is not zero-terminated.");

        return firstZero;
    }

#if NETSTANDARD2_0 || NETFRAMEWORK

    internal int Read(Span<byte> buffer)
    {
        var sharedBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length);
        try
        {
            var numRead = Read(sharedBuffer, 0, buffer.Length);
            if ((uint)numRead > (uint)buffer.Length)
                throw new IOException("Stream was too long.");

            new ReadOnlySpan<byte>(sharedBuffer, 0, numRead).CopyTo(buffer);
            return numRead;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(sharedBuffer);
        }
    }

#endif

#if NETSTANDARD2_1_OR_GREATER || NET

    /// <inheritdoc />
    /// <remarks>This method is not supported for the <see cref="PetroglyphBinaryReader"/>.
    /// Use <see cref="ReadString(Encoding,int,bool)"/> instead.</remarks>
    /// <exception cref="NotSupportedException">The method is not supported.</exception>
    public override int Read(Span<char> buffer)
    {
        throw new NotSupportedException();
    }

#endif

    /// <inheritdoc />
    /// <remarks>This method is not supported for the <see cref="PetroglyphBinaryReader"/>.</remarks>
    /// <exception cref="NotSupportedException">The method is not supported.</exception>
    public override int Read()
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    /// <remarks>This method is not supported for the <see cref="PetroglyphBinaryReader"/>.
    /// Use <see cref="ReadString(Encoding,int,bool)"/> instead.</remarks>
    /// <exception cref="NotSupportedException">The method is not supported.</exception>
    public override int Read(char[] buffer, int index, int count)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    /// <remarks>This method is not supported for the <see cref="PetroglyphBinaryReader"/>.</remarks>
    /// <exception cref="NotSupportedException">The method is not supported.</exception>
    public override char ReadChar()
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    /// <remarks>This method is not supported for the <see cref="PetroglyphBinaryReader"/>.
    /// Use <see cref="ReadString(Encoding,int,bool)"/> instead.</remarks>
    /// <exception cref="NotSupportedException">The method is not supported.</exception>
    public override string ReadString()
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    /// <remarks>This method is not supported for the <see cref="PetroglyphBinaryReader"/>.
    /// Use <see cref="ReadString(Encoding,int,bool)"/> instead.</remarks>
    /// <exception cref="NotSupportedException">The method is not supported.</exception>
    public override char[] ReadChars(int count)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    /// <remarks>This method is not supported for the <see cref="PetroglyphBinaryReader"/>.</remarks>
    /// <exception cref="NotSupportedException">The method is not supported.</exception>
    public override int PeekChar()
    {
        throw new NotSupportedException();
    }

    private class InvalidEncoding : Encoding
    {
        public static readonly InvalidEncoding Instance = new();
        [ExcludeFromCodeCoverage]
        public override int GetByteCount(char[] chars, int index, int count) => throw new InvalidOperationException();
        [ExcludeFromCodeCoverage]
        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex) => throw new InvalidOperationException();
        [ExcludeFromCodeCoverage]
        public override int GetCharCount(byte[] bytes, int index, int count) => throw new InvalidOperationException();
        [ExcludeFromCodeCoverage]
        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex) => throw new InvalidOperationException();
        public override int GetMaxByteCount(int charCount) => charCount;
        public override int GetMaxCharCount(int byteCount) => byteCount;
    }
}