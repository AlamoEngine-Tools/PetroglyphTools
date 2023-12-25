// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;
using PG.Commons.Utilities;

namespace PG.Commons.Files;

/// <summary>
/// Contains file information about a <see cref="PetroglyphFileHolder{TModel,TParam}"/>.
/// </summary>
public abstract record PetroglyphFileInformation : IDisposable
{
    private readonly string _filePath;

    /// <summary>
    /// Gets or sets the file path e.g, "c:/my/path/myfile.txt"
    /// </summary>
    /// <remarks>
    /// The path may be relative.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public required string FilePath
    {
        get => _filePath;
        [MemberNotNull(nameof(_filePath))]
        init
        {
            ThrowHelper.ThrowIfNullOrEmpty(value);
            _filePath = value;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PetroglyphFileInformation"/> class.
    /// </summary>
    protected PetroglyphFileInformation()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PetroglyphFileInformation"/> class with a given path.
    /// </summary>
    /// <param name="path">The fully qualified name of the new file, or the relative file name.</param>
    /// <exception cref="ArgumentNullException"><paramref name="path"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="path"/> is empty.</exception>
    [SetsRequiredMembers]
    protected PetroglyphFileInformation(string path)
    {
        ThrowHelper.ThrowIfNullOrEmpty(path);
        FilePath = path;
    }

    /// <inheritdoc cref="Finalize"/>
    ~PetroglyphFileInformation()
    {
        Dispose(false);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes this instance and frees managed resources.
    /// </summary>
    /// <param name="disposing">When set to <see langword="true"/> managed resources get disposed.</param>
    protected virtual void Dispose(bool disposing)
    {
    }
}