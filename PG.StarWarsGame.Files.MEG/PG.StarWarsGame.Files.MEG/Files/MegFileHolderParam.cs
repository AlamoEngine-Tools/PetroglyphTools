// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
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
        get => _key;
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
        get => _iv;
        init => _iv = (byte[]?)value?.Clone();
    }

    /// <summary>
    /// 
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