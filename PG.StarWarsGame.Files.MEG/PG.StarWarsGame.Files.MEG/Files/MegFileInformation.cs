// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;
using PG.Commons.Files;

namespace PG.StarWarsGame.Files.MEG.Files;

/// <summary>
/// Contains parameters to initialize an instance of <see cref="IMegFile"/>.
/// </summary>
public sealed record MegFileInformation : PetroglyphFileInformation
{
    /// <summary>
    /// 
    /// </summary>
    public MegFileVersion FileVersion { get; }

    /// <summary>
    /// 
    /// </summary>
    public bool HasEncryption => EncryptionData is not null;

    /// <summary>
    /// 
    /// </summary>
    public MegEncryptionData? EncryptionData { get; }

    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <param name="fileVersion"></param>
    /// <param name="encryptionData"></param>
    [SetsRequiredMembers]
    public MegFileInformation(string path, MegFileVersion fileVersion, MegEncryptionData? encryptionData = null) : base(path)
    {
        if (encryptionData is not null && fileVersion != MegFileVersion.V3)
            throw new ArgumentException("Encrypted MEG files are required to be version V3.", nameof(fileVersion));
        FileVersion = fileVersion;
        EncryptionData = encryptionData;
    }

#pragma warning disable IDE0051
    // Required so that the copy magic for C# records works and EncryptionData gets copied.
    [SetsRequiredMembers]
    private MegFileInformation(MegFileInformation original) : base(original)
    {
        FileVersion = original.FileVersion;
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