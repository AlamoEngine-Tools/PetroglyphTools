// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO;
using System.IO.Abstractions;

namespace PG.Commons.Utilities;

/// <summary>
/// Provides extensions methods to the <see cref="Stream"/> class.
/// </summary>
public static class StreamExtensions
{
    /// <summary>
    /// Gets the file path of the file opened in the <see cref="Stream"/>. The path may be relative.
    /// </summary>
    /// <param name="stream">The stream to get the file path from.</param>
    /// <param name="isMegStream">Stores the status whether <paramref name="stream"/> is a MEG stream.</param>
    /// <returns>The file path of the opened file.</returns>
    /// <exception cref="InvalidOperationException"><paramref name="stream"/> does not have path information.</exception>
    public static string GetFilePath(this Stream stream, out bool isMegStream)
    {
        isMegStream = false;
        if (stream is FileStream fileStream)
            return fileStream.Name;
        if (stream is FileSystemStream fileSystemStream)
            return fileSystemStream.Name;
        if (stream is IMegFileDataStream megFileDataStream)
        {
            isMegStream = true;
            return megFileDataStream.EntryPath;
        }
        
        throw new InvalidOperationException("Unable to get file path from Stream");
    }

    /// <summary>
    /// Gets the file path of the file opened in the <see cref="Stream"/>. The path may be relative.
    /// </summary>
    /// <param name="stream">The stream to get the file path from.</param>
    /// <returns>The file path of the opened file.</returns>
    /// <exception cref="InvalidOperationException"><paramref name="stream"/> does not have path information.</exception>
    public static string GetFilePath(this Stream stream)
    {
        return GetFilePath(stream, out _);
    }
}