// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;

namespace PG.Commons.Files;

/// <summary>
/// Contains file information about a <see cref="PetroglyphFileHolder{TModel,TParam}"/> that may be packed inside a MEG archive.
/// </summary>
public abstract record PetroglyphMegPackableFileInformation : PetroglyphFileInformation
{
    /// <summary>
    /// Gets a value indicating whether the file is packed inside a MEG archive.
    /// </summary>
    public bool IsInsideMeg { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PetroglyphFileInformation"/> class with a given path.
    /// </summary>
    /// <param name="path">The fully qualified name of the new file, or the relative file name.</param>
    /// <param name="isInMeg">Information whether the file is inside a MEG archive.</param>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="path"/> is empty.</exception>
    [SetsRequiredMembers]
    protected PetroglyphMegPackableFileInformation(string path, bool isInMeg) : base(path)
    {
        IsInsideMeg = isInMeg;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PetroglyphFileInformation"/> class.
    /// </summary>
    protected PetroglyphMegPackableFileInformation()
    {
    }
}