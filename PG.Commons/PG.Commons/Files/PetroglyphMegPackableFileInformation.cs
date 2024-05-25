using System;
using System.Diagnostics.CodeAnalysis;

namespace PG.Commons.Files;

/// <summary>
/// Contains file information about a <see cref="PetroglyphFileHolder{TModel,TParam}"/>. The file may be packed inside a MEG archive.
/// </summary>
public abstract record PetroglyphMegPackableFileInformation : PetroglyphFileInformation
{
    /// <summary>
    /// Gets whether this file info is packed as a MEG data entry.
    /// </summary>
    public bool IsInsideMeg { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PetroglyphFileInformation"/> class with a given path.
    /// </summary>
    /// <param name="path">The fully qualified name of the new file, or the relative file name.</param>
    /// <param name="isInMeg">Information whether this file info is created from a meg data entry.</param>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="path"/> is empty.</exception>
    [SetsRequiredMembers]
    protected PetroglyphMegPackableFileInformation(string path, bool isInMeg) : base(path)
    {
        IsInsideMeg = isInMeg;
    }
}