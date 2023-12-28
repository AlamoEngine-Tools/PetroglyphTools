using System;
using System.Diagnostics.CodeAnalysis;

namespace PG.StarWarsGame.Files.MEG.Files;

/// <summary>
/// 
/// </summary>
public sealed class MegEncryptionData : IDisposable
{
    private byte[]? _keyValue;
    private byte[]? _ivValue;

    /// <summary>
    /// Gets a copy of the 16 byte long initialization vector (IV).
    /// </summary>
    public byte[] IV
    {
        [return: NotNullIfNotNull(nameof(_ivValue))]
        get
        {
            if (_ivValue is null)
                throw new ObjectDisposedException(GetType().Name);
            return (byte[])_ivValue.Clone();
        }
    }

    /// <summary>
    /// Gets a copy of the AES-128 encryption key used for encryption.
    /// </summary>
    public byte[] Key
    {
        [return: NotNullIfNotNull(nameof(_keyValue))]
        get
        {
            if (_keyValue is null)
                throw new ObjectDisposedException(GetType().Name);
            return (byte[])_keyValue.Clone();
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MegEncryptionData"/> class with the required encryption keys.
    /// </summary>
    /// <remarks>
    /// Both, key and initialization vector, must be 128 bit long.
    /// </remarks>
    /// <param name="key">The encryption key.</param>
    /// <param name="iv">The initialization vector.</param>
    /// <exception cref="ArgumentException">The key size for <paramref name="key"/> or <paramref name="iv"/> is invalid.</exception>
    public MegEncryptionData(ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv)
    {
        if (!IsValidKeyOrIVSize(key))
            throw new ArgumentException("Specified key is not a valid size for MEG encryption.", nameof(key));
        if (!IsValidKeyOrIVSize(iv))
            throw new ArgumentException("Specified IV is not a valid size for MEG encryption.", nameof(iv));

        _keyValue = key.ToArray();
        _ivValue = iv.ToArray();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_keyValue is not null)
        {
            Array.Clear(_keyValue, 0, _keyValue.Length);
            _keyValue = null;
        }

        if (_ivValue is not null)
        {
            Array.Clear(_ivValue, 0, _ivValue.Length);
            _ivValue = null;
        }
    }

    internal MegEncryptionData Copy()
    {
        if (_keyValue is null || _ivValue is null)
            throw new ObjectDisposedException(GetType().Name);

        return new MegEncryptionData(_keyValue, _ivValue);
    }

    private static bool IsValidKeyOrIVSize(ReadOnlySpan<byte> data)
    {
        var bitLength = data.Length * 8L;
        return bitLength == 128;
    }
}