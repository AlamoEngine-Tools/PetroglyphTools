// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Files.MEG.Data;
using System;
using System.Diagnostics.CodeAnalysis;

namespace PG.StarWarsGame.Files.MEG.Files;

/// <summary>
/// Contains file information of an <see cref="IMegFile"/>.
/// </summary>
/// <remarks>
/// Disposing an instance of this class will clear all present encryption keys.
/// </remarks>
public sealed record MegFileInformation : PetroglyphFileInformation
{
    /// <summary>
    /// Gets the MEG version of the MEG file.
    /// </summary>
    public MegVersion Version { get; }

    /// <summary>
    /// Gets a value indicating whether an <see cref="IMegFile"/> is encrypted.
    /// </summary>
    [MemberNotNullWhen(true, nameof(EncryptionData))]
    public bool HasEncryption => EncryptionData is not null;

    /// <summary>
    /// Gets the encryption data of an <see cref="IMegFile"/> or <see langword="null"/> if the <see cref="IMegFile"/> is not encrypted.
    /// </summary>
    public MegEncryptionData? EncryptionData { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MegFileInformation"/> class.
    /// </summary>
    /// <param name="path">The file path of the MEG file.</param>
    /// <param name="version">The MEG version of the file.</param>
    /// <param name="encryptionData">The encryption data of MEG file or <see langword="null"/> if the MEG file is not encrypted.</param>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="path"/> is empty.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="version"/> is not <see cref="MegVersion.V3"/> but <paramref name="encryptionData"/> is not <see langword="null"/>.
    /// </exception>
    [SetsRequiredMembers]
    public MegFileInformation(string path, MegVersion version, MegEncryptionData? encryptionData = null) : base(path)
    {
        if (encryptionData is not null && version != MegVersion.V3)
            throw new ArgumentException("Encrypted MEG files are required to be version V3.", nameof(version));
        Version = version;
        EncryptionData = encryptionData;
    }

#pragma warning disable IDE0051
    // Required so that the copy magic for C# records works and EncryptionData gets copied.
    [SetsRequiredMembers]
    private MegFileInformation(MegFileInformation original) : base(original)
    {
        Version = original.Version;
        EncryptionData = original.EncryptionData?.Copy();
    }
#pragma warning restore IDE0051

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if(disposing)
            EncryptionData?.Dispose();
    }
}