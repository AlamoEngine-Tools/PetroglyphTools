// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.StarWarsGame.Files.Binary;

namespace PG.StarWarsGame.Files.MEG.Binary.Metadata;

internal interface IMegHeader : IBinary
{
    /// <remarks>
    /// The .MEG specification allows <see cref="uint"/>, however in .NET we are
    /// limited to <see cref="int"/> for indexing native list-like structures.  
    /// </remarks>
    int FileNumber { get; }
}