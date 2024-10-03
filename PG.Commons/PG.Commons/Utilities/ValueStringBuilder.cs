using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PG.Commons.Utilities;

/// <summary>
/// Represents a string builder which attempts to reduce as many allocations as possible.
/// </summary>
/// <remarks>
/// Mind this a <b>ref struct</b>. Pass it by-ref to methods.
/// </remarks>
public ref struct ValueStringBuilder
{
    private char[]? _arrayToReturnToPool;
    private Span<char> _chars;
    private int _pos;

    /// <summary>
    /// Gets or sets the current length of the represented string.
    /// </summary>
    /// <value>
    /// The current length of the represented string.
    /// </value>
    public int Length
    {
        readonly get => _pos;
        set
        {
            Debug.Assert(value >= 0);
            Debug.Assert(value <= _chars.Length);
            _pos = value;
        }
    }

    /// <summary>
    /// Gets the current maximum capacity before growing the array.
    /// </summary>
    /// <value>
    /// The current maximum capacity before growing the array.
    /// </value>
    public readonly int Capacity => _chars.Length;

    /// <summary>
    /// Returns the character at the given index or throws an <see cref="IndexOutOfRangeException"/> if the index is bigger than the string.
    /// </summary>
    /// <param name="index">Index position, which should be retrieved.</param>
    public readonly ref char this[int index]
    {
        get
        {
            Debug.Assert(index < _pos);
            return ref _chars[index];
        }
    }

    /// <summary>Returns the underlying storage of the builder.</summary>
    public Span<char> RawChars => _chars;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueStringBuilder"/> struct.
    /// </summary>
    /// <param name="initialBuffer">Initial buffer for the string builder to begin with.</param>
    public ValueStringBuilder(Span<char> initialBuffer)
    {
        _arrayToReturnToPool = null;
        _chars = initialBuffer;
        _pos = 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueStringBuilder"/> struct.
    /// </summary>
    /// <param name="initialCapacity">The initial capacity that will be allocated for this instance.</param>
    public ValueStringBuilder(int initialCapacity)
    {
        _arrayToReturnToPool = ArrayPool<char>.Shared.Rent(initialCapacity);
        _chars = _arrayToReturnToPool;
        _pos = 0;
    }

    /// <summary>
    /// Ensures that the builder has at least <paramref name="capacity"/> amount of capacity.
    /// </summary>
    /// <param name="capacity">New capacity for the builder.</param>
    /// <remarks>
    /// If <paramref name="capacity"/> is smaller or equal to <see cref="Length"/> nothing will be done.
    /// </remarks>
    public void EnsureCapacity(int capacity)
    {
        // This is not expected to be called this with negative capacity
        Debug.Assert(capacity >= 0);

        // If the caller has a bug and calls this with negative capacity, make sure to call Grow to throw an exception.
        if ((uint)capacity > (uint)_chars.Length)
            Grow(capacity - _pos);
    }

    /// <summary>
    /// Gets a pinnable reference to the represented string from this builder.
    /// The content after <see cref="Length"/> is not guaranteed to be null terminated.
    /// </summary>
    /// <returns>The pointer to the first instance of the string represented by this builder.</returns>
    /// <remarks>
    /// This method is used for use-cases where the user wants to use "fixed" calls like the following:
    /// <code>
    /// using var stringBuilder = new ValueStringBuilder();
    /// stringBuilder.Append("Hello World");
    /// fixed (var* buffer = stringBuilder) { ... }
    /// </code>
    /// </remarks>
    public ref char GetPinnableReference()
    {
        return ref MemoryMarshal.GetReference(_chars);
    }

    /// <summary>
    /// Get a pinnable reference to the builder.
    /// </summary>
    /// <param name="terminate">Ensures that the builder has a null char after <see cref="Length"/></param>
    public ref char GetPinnableReference(bool terminate)
    {
        if (terminate)
        {
            EnsureCapacity(Length + 1);
            _chars[Length] = '\0';
        }
        return ref MemoryMarshal.GetReference(_chars);
    }

    /// <summary>
    /// Creates a <see cref="string"/> instance from that builder and disposed this instance.
    /// </summary>
    /// <returns>The <see cref="string"/> instance.</returns>
    public override string ToString()
    {
        var s = _chars.Slice(0, _pos).ToString();
        Dispose();
        return s;
    }

    /// <summary>
    /// Returns a span around the contents of the builder.
    /// </summary>
    /// <param name="terminate">Ensures that the builder has a null char after <see cref="Length"/></param>
    public ReadOnlySpan<char> AsSpan(bool terminate)
    {
        if (terminate)
        {
            EnsureCapacity(Length + 1);
            _chars[Length] = '\0';
        }
        return _chars.Slice(0, _pos);
    }

    /// <summary>
    /// Returns a readonly view of the underlying storage of this builder.
    /// </summary>
    public readonly ReadOnlySpan<char> AsSpan()
    {
        return _chars.Slice(0, _pos);
    }

    /// <summary>
    /// Returns a readonly view of the underlying storage of this builder from a specified starting position;
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/> is less than zero or greater than <see cref="Length"/>.</exception>
    public readonly ReadOnlySpan<char> AsSpan(int start)
    {
        return _chars.Slice(start, _pos - start);
    }

    /// <summary>
    /// Returns a readonly view of the underlying storage of this builder from a specified starting position;
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/> or <paramref name="start"/> + <paramref name="length"/> is less than zero or greater than <see cref="Length"/>.</exception>
    public readonly ReadOnlySpan<char> AsSpan(int start, int length)
    {
        return _chars.Slice(start, length);
    }

    /// <summary>
    /// Tries to copy the represented string into the given <see cref="Span{T}"/> and always disposes this instance.
    /// </summary>
    /// <param name="destination">The destination where the internal string is copied into.</param>
    /// <param name="charsWritten">The number of characters copied to <paramref name="destination"/> is stored in this variable.</param>
    /// <returns><see langword="true"/>, if the copy was successful, otherwise <see langword="false"/>.</returns>
    public bool TryCopyTo(Span<char> destination, out int charsWritten)
    {
        if (_chars.Slice(0, _pos).TryCopyTo(destination))
        {
            charsWritten = _pos;
            Dispose();
            return true;
        }

        charsWritten = 0;
        Dispose();
        return false;
    }

    /// <summary>
    /// Inserts the string at the specified index into this builder.
    /// </summary>
    /// <param name="index">Index where <paramref name="value"/> should be inserted.</param>
    /// <param name="value">String to insert into this builder.</param>
    public void Insert(int index, string? value)
    {
        if (value == null)
            return;

        var count = value.Length;

        if (_pos > _chars.Length - count) 
            Grow(count);

        var remaining = _pos - index;
        _chars.Slice(index, remaining).CopyTo(_chars.Slice(index + count));
        value
#if !NET
            .AsSpan()
#endif
            .CopyTo(_chars.Slice(index));
        _pos += count;
    }

    /// <summary>
    /// Removes a range of characters from this builder.
    /// </summary>
    /// <param name="startIndex">The inclusive index from where the string gets removed.</param>
    /// <param name="length">The length of the slice to remove.</param>
    /// <remarks>
    /// This method will not affect the internal size of the string.
    /// </remarks>
    public void Remove(int startIndex, int length)
    {
        if (length == 0)
            return;

        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length), "The given length can't be negative.");

        if (startIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(startIndex), "The given start index can't be negative.");

        if (length > Length - startIndex)
            throw new ArgumentOutOfRangeException(nameof(length), $"The given Span ({startIndex}..{length})length is outside the the represented string.");

        if (Length == length && startIndex == 0)
        {
            Length = 0;
            return;
        }

        var currentLength = Length;
        var remaining = _chars.Slice(startIndex + length, currentLength - length);
        var toOverwrite = _chars.Slice(startIndex);
        remaining.CopyTo(toOverwrite);
        _pos = currentLength - length;
    }

    /// <summary>
    /// Appends a character to the string builder.
    /// </summary>
    /// <param name="c">Character, which will be added to this builder.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(char c)
    {
        var pos = _pos;
        var chars = _chars;
        if ((uint)pos < (uint)chars.Length)
        {
            chars[pos] = c;
            _pos = pos + 1;
        }
        else
        {
            GrowAndAppend(c);
        }
    }

    /// <summary>
    /// Appends a string to the string builder.
    /// </summary>
    /// <param name="str">String, which will be added to this builder.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Append(string? str)
    {
        if (str == null)
            return;

        var pos = _pos;
        if (str.Length == 1 && (uint)pos < (uint)_chars.Length) // very common case, e.g. appending strings from NumberFormatInfo like separators, percent symbols, etc.
        {
            _chars[pos] = str[0];
            _pos = pos + 1;
        }
        else
        {
            AppendSlow(str);
        }
    }

    private void AppendSlow(string s)
    {
        var pos = _pos;
        if (pos > _chars.Length - s.Length)
        {
            Grow(s.Length);
        }

        s
#if !NET
            .AsSpan()
#endif
            .CopyTo(_chars.Slice(pos));
        _pos += s.Length;
    }

    /// <summary>
    /// Appends a string to the string builder.
    /// </summary>
    /// <param name="value">String, which will be added to this builder.</param>
    public void Append(scoped ReadOnlySpan<char> value)
    {
        var pos = _pos;
        if (pos > _chars.Length - value.Length)
        {
            Grow(value.Length);
        }

        value.CopyTo(_chars.Slice(_pos));
        _pos += value.Length;
    }

    /// <summary>
    /// Gets a <see cref="Span{T}"/> of desired length of the string builder.
    /// </summary>
    /// <param name="length">The desired length of the new span.</param>
    /// <returns>The appended span.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<char> AppendSpan(int length)
    {
        var origPos = _pos;
        if (origPos > _chars.Length - length)
        {
            Grow(length);
        }

        _pos = origPos + length;
        return _chars.Slice(origPos, length);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void GrowAndAppend(char c)
    {
        Grow(1);
        Append(c);
    }

    /// <summary>
    /// Resize the internal buffer either by doubling current buffer size or
    /// by adding <paramref name="additionalCapacityBeyondPos"/> to
    /// <see cref="_pos"/> whichever is greater.
    /// </summary>
    /// <param name="additionalCapacityBeyondPos">
    /// Number of chars requested beyond current position.
    /// </param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private void Grow(int additionalCapacityBeyondPos)
    {
        Debug.Assert(additionalCapacityBeyondPos > 0);
        Debug.Assert(_pos > _chars.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");

        const uint ArrayMaxLength = 0x7FFFFFC7; // same as Array.MaxLength

        // Increase to at least the required size (_pos + additionalCapacityBeyondPos), but try
        // to double the size if possible, bounding the doubling to not go beyond the max array length.
        var newCapacity = (int)Math.Max(
            (uint)(_pos + additionalCapacityBeyondPos),
            Math.Min((uint)_chars.Length * 2, ArrayMaxLength));

        // Make sure to let Rent throw an exception if the caller has a bug and the desired capacity is negative.
        // This could also go negative if the actual required length wraps around.
        var poolArray = ArrayPool<char>.Shared.Rent(newCapacity);

        _chars.Slice(0, _pos).CopyTo(poolArray);

        var toReturn = _arrayToReturnToPool;
        _chars = _arrayToReturnToPool = poolArray;
        if (toReturn != null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }
    
    /// <summary>
    /// Disposes the instance and returns rented buffer from an array pool if needed.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        var toReturn = _arrayToReturnToPool;
        this = default; // for safety, to avoid using pooled array if this instance is erroneously appended to again
        if (toReturn != null)
        {
            ArrayPool<char>.Shared.Return(toReturn);
        }
    }
}