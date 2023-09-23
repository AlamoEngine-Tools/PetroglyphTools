// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using PG.Commons.Files;
using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Files;

/// <inheritdoc cref="IMegFile" />
/// <remarks>
///     This class does not hold all files that are packaged in a *.MEG file,
///     but all necessary meta-information to extract a given file on-demand.
/// </remarks>
public sealed class MegFileHolder : FileHolderBase<MegFileHolderParam, IReadOnlyList<MegFileDataEntry>, MegAlamoFileType>, IMegFile
{
    private byte[]? _keyValue;
    private byte[]? _ivValue;

    /// <summary>
    ///     Gets the file version of the MEG file.
    /// </summary>
    public MegFileVersion FileVersion { get; }

    /// <inheritdoc/>
    public int Count => Content.Count;

    /// <summary>
    ///     Gets a copy of the initialization vector (IV) used for encryption. <see langword="null" /> if the file is not
    ///     encrypted.
    /// </summary>
    public byte[]? IV
    {
        [return: NotNullIfNotNull(nameof(_ivValue))]
        get
        {
            if (_ivValue is null)
            {
                return null;
            }

            return (byte[])_ivValue.Clone();
        }
    }

    /// <summary>
    ///     Gets a copy of the encryption key used for encryption. <see langword="null" /> if the file is not encrypted.
    /// </summary>
    public byte[]? Key
    {
        [return: NotNullIfNotNull(nameof(_keyValue))]
        get
        {
            if (_keyValue is null)
            {
                return null;
            }

            return (byte[])_keyValue.Clone();
        }
    }


    /// <summary>
    ///     Gets a value indicating whether the MEG file is encrypted.
    /// </summary>
    public bool HasEncryption
    {
        get
        {
            if (FileVersion is MegFileVersion.V1 or MegFileVersion.V2)
            {
                return false;
            }

            return Key != null && IV != null;
        }
    }

    internal MegFileHolder(IReadOnlyList<MegFileDataEntry> model, MegFileHolderParam param, IServiceProvider serviceProvider) : base(model, param, serviceProvider)
    {
    }

    /// <inheritdoc />
    public bool Contains(MegFileDataEntry entry)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public bool TryGetAllEntriesWithMatchingPattern(string fileName,
        out IReadOnlyList<MegFileDataEntry> megFileDataEntries)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            megFileDataEntries = Array.Empty<MegFileDataEntry>();
            return false;
        }

        megFileDataEntries = Content.Where(dataEntry => ContainsPathIgnoreCase(dataEntry.RelativeFilePath, fileName))
            .ToList();
        return megFileDataEntries.Any();
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

    private static bool ContainsPathIgnoreCase(string relativePath, string partFileName)
    {
#if NETSTANDARD2_1_OR_GREATER
        return relativePath.Contains(partFileName, StringComparison.CurrentCultureIgnoreCase);
#else
        return relativePath.IndexOf(partFileName, StringComparison.CurrentCultureIgnoreCase) >= 0;
#endif
    }


    private static void ValidateEncryptionLength(ReadOnlySpan<byte> data)
    {
        var bitLength = data.Length * 8L;
        if (bitLength != 128)
        {
            throw new ArgumentException("Specified data is not a valid size for MEG encryption.", nameof(data));
        }
    }

    /// <inheritdoc/>
    public IEnumerator<MegFileDataEntry> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}