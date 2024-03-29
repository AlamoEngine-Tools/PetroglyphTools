// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.Commons.Files;

/// <summary>
/// A generic wrapper around Petroglyph game files that holds the file content in an accessible data structure as well as other file information.
/// </summary>
/// <typeparam name="TModel">The type of the content this file holds.</typeparam>
/// <typeparam name="TFileInfo">The type of the file information.</typeparam>
public interface IPetroglyphFileHolder<out TModel, out TFileInfo> : IDisposable
    where TModel : notnull
    where TFileInfo : PetroglyphFileInformation
{
    /// <summary>
    /// Gets a copy of the file information. 
    /// </summary>
    /// <remarks>
    /// The returned file information can be safely disposed without affecting this instance.
    /// </remarks>
    /// <exception cref="ObjectDisposedException">The current instance is already disposed.</exception>
    TFileInfo FileInformation { get; }

    /// <summary>
    /// Gets the data model of the alamo file in a usable data format.
    /// </summary>
    TModel Content { get; }
    
    /// <summary>
    /// Gets the absolute file path e.g, "c:/my/path/myfile.txt"
    /// </summary>
    string FilePath { get; }

    /// <summary>
    /// The absolute path to the directory of the file.
    /// </summary>
    string Directory { get; }

    /// <summary>
    /// The file name with extension, e.g, "myfile.txt".
    /// </summary>
    string FileName { get; }
}