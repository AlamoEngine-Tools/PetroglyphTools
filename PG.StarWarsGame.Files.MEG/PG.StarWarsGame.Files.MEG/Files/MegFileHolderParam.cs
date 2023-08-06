// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Files;

namespace PG.StarWarsGame.Files.MEG.Files;

///<inheritdoc />
public sealed record MegFileHolderParam : FileHolderParamBase
{
    /// <summary>
    ///     <see cref="MegFileHolder.FileVersion" />
    /// </summary>
    public MegFileVersion? FileVersion { get; set; }
}