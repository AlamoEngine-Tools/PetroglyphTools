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
    ///     Gets a copy of the 16 byte long initialization vector (IV) used for encryption or <see langword="null" /> if the file is not
    ///     encrypted.
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
    ///     Gets a copy of the AES-128 encryption key used for encryption or <see langword="null" /> if the file is not encrypted.
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
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="iv"></param>
    /// <exception cref="ArgumentException"></exception>
    public MegEncryptionData(ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv)
    {
        if (!IsValidKeyOrIVSize(key))
            throw new ArgumentException("Specified key is not a valid size for MEG encryption.", nameof(key));
        if (!IsValidKeyOrIVSize(iv))
            throw new ArgumentException("Specified IV is not a valid size for MEG encryption.", nameof(iv));

        _keyValue = key.ToArray();
        _ivValue = iv.ToArray();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public MegEncryptionData Copy()
    {
        if (_keyValue is null || _ivValue is null)
            throw new ObjectDisposedException(GetType().Name);

        return new MegEncryptionData(_keyValue, _ivValue);
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

    private static bool IsValidKeyOrIVSize(ReadOnlySpan<byte> data)
    {
        var bitLength = data.Length * 8L;
        return bitLength == 128;
    }
}