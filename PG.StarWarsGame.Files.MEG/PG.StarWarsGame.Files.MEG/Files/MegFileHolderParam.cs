// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Files;

namespace PG.StarWarsGame.Files.MEG.Files;

///<inheritdoc />
public sealed record MegFileHolderParam : PetroglyphFileInformation
{
    /// <inheritdoc cref="MegFileHolder.FileVersion" />
    public MegFileVersion FileVersion { get; init; }
}