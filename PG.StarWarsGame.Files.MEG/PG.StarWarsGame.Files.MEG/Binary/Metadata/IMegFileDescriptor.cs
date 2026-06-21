// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Data;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.MEG.Data;

namespace PG.StarWarsGame.Files.MEG.Binary.Metadata;

internal interface IMegFileDescriptor : IBinary, IHasCrc32, IComparable<IMegFileDescriptor>
{
    /// <summary>
    /// Gets the offset, in bytes, where the described file's data starts within the .MEG archive.
    /// </summary>
    public uint FileOffset { get; }

    /// <summary>
    /// Gets the size, in bytes, of the described file's data.
    /// </summary>
    public uint FileSize { get; }

    /// <summary>
    /// Gets the index of the described file's name within the file name table.
    /// </summary>
    /// <remarks>
    /// The .MEG specification allows <see cref="uint"/>, however in .NET we are
    /// limited to <see cref="int"/> for indexing native list-like structures.<br/>
    /// <br/>
    /// For <see cref="MegVersion.V3"/> .MEG archives this values type must be treated as <see cref="ushort"/>.<br/>
    /// When reading or writing a .MEG binary, assertions should be implemented to verify data validity.
    /// </remarks>
    public int FileNameIndex { get; }

    /// <summary>
    /// Gets the index of the described file within the file table.
    /// </summary>
    /// <remarks>
    /// The .MEG specification allows <see cref="uint"/>, however in .NET we are
    /// limited to <see cref="int"/> for indexing native list-like structures.<br/>
    /// </remarks>
    public int Index { get; }

    /// <summary>
    /// Gets a value that indicates whether the described file's data is encrypted.
    /// </summary>
    public bool Encrypted { get; }
}