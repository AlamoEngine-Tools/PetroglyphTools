// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;
using PG.Commons.Files;
using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Files;

/// <inheritdoc cref="IMegFile" />
/// <remarks>
///     This class does not hold all files that are packaged in a *.MEG file,
///     but all necessary meta-information to extract a given file on-demand.
/// </remarks>
public sealed class MegFileHolder : FileHolderBase<MegFileHolderParam, IMegArchive, MegAlamoFileType>, IMegFile
{
    private byte[]? _keyValue;
    private byte[]? _ivValue;

    /// <summary>
    ///     Gets the file version of the MEG file.
    /// </summary>
    public MegFileVersion FileVersion { get; }

    /// <summary>
    ///     Gets a copy of the 16 byte long initialization vector (IV) used for encryption or <see langword="null" /> if the file is not
    ///     encrypted.
    /// </summary>
    public byte[]? IV
    {
        [return: NotNullIfNotNull(nameof(_ivValue))]
        get
        {
            if (_ivValue is null)
                return null;
            return (byte[])_ivValue.Clone();
        }
    }

    /// <summary>
    ///     Gets a copy of the AES-128 encryption key used for encryption or <see langword="null" /> if the file is not encrypted.
    /// </summary>
    public byte[]? Key
    {
        [return: NotNullIfNotNull(nameof(_keyValue))]
        get
        {
            if (_keyValue is null)
                return null;
            return (byte[])_keyValue.Clone();
        }
    }


    /// <summary>
    ///     Gets a value indicating whether the MEG file is encrypted.
    /// </summary>
    public bool HasEncryption => Key != null && IV != null;

    /// <summary>
    /// Initializes a new instance of the <see cref="MegFileHolder"/> class. 
    /// </summary>
    /// <remarks>
    /// It is safe to dispose the <paramref name="param"/> after an instance of this class has been created.
    /// </remarks>
    /// <param name="model">The meg archive model.</param>
    /// <param name="param">The initialization parameters.</param>
    /// <param name="serviceProvider">The service provider for this instance.</param>
    public MegFileHolder(IMegArchive model, MegFileHolderParam param, IServiceProvider serviceProvider) :
        base(model, param, serviceProvider)
    {
        FileVersion = param.FileVersion;

        if (param.HasEncryption && param.FileVersion != MegFileVersion.V3)
            throw new ArgumentException("Encrypted MEG files must be of version V3");

        var key = (byte[]?)param.Key?.Clone();
        var iv = (byte[]?)param.IV?.Clone();

        if (!IsValidKeyOrIVSize(key))
            throw new ArgumentException("Specified key is not a valid size for MEG encryption.", nameof(key));
        if (!IsValidKeyOrIVSize(iv))
            throw new ArgumentException("Specified IV is not a valid size for MEG encryption.", nameof(iv));

        _keyValue = key;
        _ivValue = iv;
    }

    /// <inheritdoc />
    protected override void DisposeManagedResources()
    {
        base.DisposeManagedResources();
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