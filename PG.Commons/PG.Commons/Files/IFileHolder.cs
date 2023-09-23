// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.Commons.Files;

/// <summary>
///     A generic wrapper around alamo file types that holds the file content in an accessible data structure.
/// </summary>
public interface IFileHolder : IDisposable
{
    /// <summary>
    ///     The path to the directory that holds the file on disc.
    /// </summary>
    /// <remarks>
    ///     This path may represent a relative location.
    /// </remarks>
    string Directory { get; }

    /// <summary>
    ///     The desired file name without the file extension, eg "myfile".
    /// </summary>
    string FileName { get; }
}

/// <summary>
///     A generic wrapper around alamo file types that holds the file content in an accessible data structure.
/// </summary>
/// <typeparam name="TModel">The content of the alamo file in a usable data format.</typeparam>
/// <typeparam name="TAlamoFileType">The alamo file type definition implementing <see cref="IAlamoFileType" /></typeparam>
public interface IFileHolder<out TModel, out TAlamoFileType> : IFileHolder
    where TAlamoFileType : IAlamoFileType
{
    /// <summary>
    ///     The alamo file type definition implementing <see cref="IAlamoFileType" />
    /// </summary>
    TAlamoFileType FileType { get; }

    /// <summary>
    ///     The data model of the alamo file in a usable data format.
    /// </summary>
    TModel Content { get; }

    /// <summary>
    ///     The file name including the full path, eg. "c:/my/path/myfile.txt"
    /// </summary>
    /// <remarks>
    ///     This path may represent a relative location.
    /// </remarks>
    string FilePath { get; }
}