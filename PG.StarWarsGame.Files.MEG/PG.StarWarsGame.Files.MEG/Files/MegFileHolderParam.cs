// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;
using PG.Commons.Files;

namespace PG.StarWarsGame.Files.MEG.Files;

/// <summary>
/// Contains parameters to initialize an instance of <see cref="IMegFile"/>.
/// </summary>
/// <remarks>
/// Disposing an instance of this class, will clear the <see cref="Key"/> and <see cref="IV"/> arrays.
/// </remarks>
public sealed record MegFileHolderParam : FileHolderParamBase, IDisposable
{
    private byte[]? _iv;
    private byte[]? _key;

    /// <inheritdoc cref="MegFileHolder.FileVersion" />
    public MegFileVersion FileVersion { get; init; }

    /// <summary>
    /// The AES-128 encryption key.
    /// </summary>
    /// <remarks>
    /// Initializing this property will clone the input value.
    /// </remarks>
    public byte[]? Key
    {
        [return: NotNullIfNotNull(nameof(_key))]
        get
        {
            if (_key is null)
                return null;
            return (byte[])_key.Clone();
        }
        init => _key = (byte[]?) value?.Clone();
    }

    /// <summary>
    /// The Initialization vector for encryption.
    /// </summary>
    /// <remarks>
    /// Initializing this property will clone the input value.
    /// </remarks>
    public byte[]? IV
    {
        [return: NotNullIfNotNull(nameof(_iv))]
        get
        {
            if (_iv is null)
                return null;
            return (byte[])_iv.Clone();
        }
        init => _iv = (byte[]?)value?.Clone();
    }

    /// <summary>
    /// <see langword="true"/> if this instance has an encryption key and an initialization vector; <see langword="false"/> otherwise.
    /// </summary>
    public bool HasEncryption => Key is not null && IV is not null;

    /// <inheritdoc/>
    public void Dispose()
    {
        try
        {
            if (_key is not null)
                Array.Clear(_key, 0, _key.Length);
            if (_iv is not null)
                Array.Clear(_iv, 0, _iv.Length);
        }
        finally
        {
            _key = null;
            _iv = null;
        }
    }
}