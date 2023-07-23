// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.StarWarsGame.Files.MEG.Data;

/// <summary>
/// Represents the location information of a file to be packed inside a MEG file.
/// </summary>
public class MegFileDataEntryLocation
{
    /// <summary>
    /// Gets the relative file path as defined in the *.MEG file.<br />
    /// Usually this file path is relative to the game or mod's DATA directory, e.g. Data/My/file.xml
    /// </summary>
    public string PathInMeg { get; }

    /// <summary>
    /// Gets the current path of the file on the local file system.
    /// </summary>
    public string PathToFile { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MegFileDataEntryLocation"/> struct.
    /// </summary>
    /// <param name="pathInMeg">The relative file path as defined in the *.MEG file.</param>
    /// <param name="pathToFile">The current path of the file on the local file system.</param>
    public MegFileDataEntryLocation(string pathInMeg, string pathToFile)
    {
        if (string.IsNullOrWhiteSpace(pathInMeg))
            throw new ArgumentNullException(nameof(pathInMeg));
        if (string.IsNullOrWhiteSpace(pathToFile))
            throw new ArgumentNullException(nameof(pathToFile));
        PathInMeg = pathInMeg;
        PathToFile = pathToFile;
    }
}