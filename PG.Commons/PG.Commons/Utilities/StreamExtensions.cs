// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Data;
using System;
using System.Diagnostics.CodeAnalysis;
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
    /// <returns>The file path of the opened file.</returns>
    /// <exception cref="InvalidOperationException"><paramref name="stream"/> does not have path information.</exception>
    public static string GetFilePath(this Stream stream)
    {
        return stream.GetFilePath(out _);
    }

    /// <summary>
    /// Gets the file path of the file opened in the <see cref="Stream"/>. The path may be relative.
    /// </summary>
    /// <param name="stream">The stream to get the file path from.</param>
    /// <param name="isMegStream">Stores the status whether <paramref name="stream"/> is a MEG stream.</param>
    /// <returns>The file path of the opened file.</returns>
    /// <exception cref="InvalidOperationException"><paramref name="stream"/> does not have path information.</exception>
    public static string GetFilePath(this Stream stream, out bool isMegStream)
    {
        return !stream.TryGetFilePath(out var filePath, out isMegStream)
            ? throw new InvalidOperationException("Unable to get file path from Stream")
            : filePath;
    }

    /// <summary>
    /// Attempts to retrieve the file path of the file opened in the specified <see cref="Stream"/>.
    /// </summary>
    /// <param name="stream">The stream to retrieve the file path from.</param>
    /// <returns>
    /// The file path of the opened file if the operation was successful; otherwise, <see langword="null"/>.
    /// </returns>
    public static string? TryGetFilePath(this Stream stream)
    {
        return stream.TryGetFilePath(out var fileName, out _) ? fileName : null;
    }

    /// <summary>
    /// Attempts to retrieve the file path of the file opened in the specified <see cref="Stream"/>.
    /// </summary>
    /// <param name="stream">The stream to retrieve the file path from.</param>
    /// <param name="fileName">
    /// When this method returns, contains the file path of the opened file if the operation was successful; 
    /// otherwise, <see langword="null"/>. This parameter is passed uninitialized.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the file path was successfully retrieved; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool TryGetFilePath(this Stream stream, [NotNullWhen(true)] out string? fileName)
    {
        return stream.TryGetFilePath(out fileName, out _);
    }

    /// <summary>
    /// Attempts to retrieve the file path of the file opened in the specified <see cref="Stream"/>.
    /// </summary>
    /// <param name="stream">The stream to retrieve the file path from.</param>
    /// <param name="fileName">
    /// When this method returns, contains the file path of the opened file if the operation was successful; 
    /// otherwise, <see langword="null"/>. This parameter is passed uninitialized.
    /// </param>
    /// <param name="isMegStream">
    /// When this method returns, contains a value indicating whether the stream is a MEG file data stream.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the file path was successfully retrieved; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool TryGetFilePath(this Stream stream, [NotNullWhen(true)] out string? fileName, out bool isMegStream)
    {
        isMegStream = false;
        switch (stream)
        {
            case FileStream fileStream:
                fileName =  fileStream.Name;
                break;
            case FileSystemStream fileSystemStream:
                fileName = fileSystemStream.Name;
                break;
            case IMegFileDataStream megFileDataStream:
                isMegStream = true;
                fileName =  megFileDataStream.EntryPath;
                break;
            default:
                fileName = null;
                return false;
        }
        return true;
    }
}