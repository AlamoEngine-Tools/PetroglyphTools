﻿// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Data;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.MEG.Files;

namespace PG.StarWarsGame.Files.MEG.Binary.Metadata;

internal interface IMegFileDescriptor : IBinary, IHasCrc32, IComparable<IMegFileDescriptor>
{
    public uint FileOffset { get; }

    public uint FileSize { get; }

    /// <remarks>
    /// The .MEG specification allows <see cref="uint"/>, however in .NET we are
    /// limited to <see cref="int"/> for indexing native list-like structures.<br/>
    /// <br/>
    /// For <see cref="MegFileVersion.V3"/> .MEG archives this values type must be treated as <see cref="ushort"/>.<br/>
    /// When reading or writing a .MEG binary, assertions should be implemented to verify data validity.
    /// </remarks>
    public int FileNameIndex { get; }

    /// <remarks>
    /// The .MEG specification allows <see cref="uint"/>, however in .NET we are
    /// limited to <see cref="int"/> for indexing native list-like structures.<br/>
    /// </remarks>
    public int Index { get; }
    
    public bool Encrypted { get; }
}