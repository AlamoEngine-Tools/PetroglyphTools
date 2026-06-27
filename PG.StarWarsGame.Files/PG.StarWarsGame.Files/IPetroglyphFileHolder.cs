// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.StarWarsGame.Files;

/// <summary>
/// Represents a wrapper around Petroglyph game files that holds the file's content in an accessible data structure as well as other file information.
/// </summary>
public interface IPetroglyphFileHolder : IDisposable
{
    /// <summary>
    /// Gets a copy of the file information. 
    /// </summary>
    /// <remarks>
    /// The returned file information can be safely disposed without affecting the <see cref="IPetroglyphFileHolder"/>.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">The current instance is disposed.</exception>
    PetroglyphFileInformation FileInformation { get; }

    /// <summary>
    /// Gets the data model of the file in a usable data format.
    /// </summary>
    object Content { get; }

    /// <summary>
    /// Gets the relative or absolute file path e.g, "c:/my/path/myfile.txt".
    /// </summary>
    /// <remarks>
    /// <para>
    /// If <see cref="FileInformation"/> originates from a MEG file,
    /// the returned path is normalized to the current operating system.
    /// This means the Windows directory separator ("\")
    /// is treated as directory separator on linux systems too.
    /// To retrieve the original file path for MEG files, use <see cref="PetroglyphFileInformation.FilePath"/> of <see cref="FileInformation"/>.
    /// </para>
    /// <para>
    /// Relative paths are only returned if the file is packed inside a MEG archive.
    /// </para>
    /// </remarks>
    string FilePath { get; }

    /// <summary>
    /// Gets the relative or absolute directory path of the file e.g, "my/path"
    /// or <see cref="string.Empty"/> if the file does not contain directory information.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If <see cref="FileInformation"/> originates from a MEG file,
    /// the returned directory path is normalized to the current operating system.
    /// This means the Windows directory separator ("\")
    /// is treated as directory separator on linux systems too.
    /// To retrieve the original file path for MEG files, use <see cref="PetroglyphFileInformation.FilePath"/> of <see cref="FileInformation"/>.
    /// </para>
    /// <para>
    /// Relative or empty paths are only allowed if the file is packed inside a MEG archive.
    /// </para>
    /// </remarks>
    string Directory { get; }

    /// <summary>
    /// Gets the file name with extension, e.g, "myfile.txt".
    /// </summary>
    /// <remarks>
    /// If <see cref="FileInformation"/> originates from a MEG file,
    /// the returned file name is normalized to the current operating system.
    /// This means the Windows directory separator ("\")
    /// is treated as directory separator on linux systems too.
    /// To retrieve the original file path for MEG files, use <see cref="PetroglyphFileInformation.FilePath"/> of <see cref="FileInformation"/>.
    /// </remarks>
    string FileName { get; }
}

/// <summary>
/// Represents a generic wrapper around Petroglyph game files that holds the file's content in an accessible data structure as well as other file information.
/// </summary>
/// <typeparam name="TModel">The type of the content this file holds.</typeparam>
/// <typeparam name="TFileInfo">The type of the file information.</typeparam>
public interface IPetroglyphFileHolder<out TModel, out TFileInfo> : IPetroglyphFileHolder
    where TModel : notnull
    where TFileInfo : PetroglyphFileInformation
{
    /// <summary>
    /// Gets a copy of the file information. 
    /// </summary>
    /// <remarks>
    /// The returned file information can be safely disposed without affecting this instance.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">The current instance is disposed.</exception>
    new TFileInfo FileInformation { get; }

    /// <summary>
    /// Gets the data model of the file in a usable data format.
    /// </summary>
    new TModel Content { get; }
}