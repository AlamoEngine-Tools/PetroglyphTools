﻿// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO;

namespace PG.StarWarsGame.Files.Services.Builder;

/// <summary>
/// Service to write data into their file representation. 
/// </summary>
/// <typeparam name="TFileInformation">The type of the file information data.</typeparam>
/// <typeparam name="TData">The type of the data to build files from.</typeparam>
public interface IFileBuilder<out TData, in TFileInformation> : IDisposable
    where TFileInformation : PetroglyphFileInformation 
    where TData : notnull
{
    /// <summary>
    /// Gets a copy of the data from the <see cref="IFileBuilder{TData,TFileInformation}"/>.
    /// </summary>
    TData BuilderData { get; }

    /// <summary>
    /// Creates a file from the <see cref="IFileBuilder{TData,TFileInformation}"/>.
    /// </summary>
    /// <param name="fileInformation">The file parameters of the new file.</param>
    /// <param name="overwrite">When set to <see langword="true"/> an existing file of the same path will be overwritten; otherwise an <see cref="IOException"/> is thrown if the file already exists.</param>
    /// <exception cref="ArgumentNullException"><paramref name="fileInformation"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="fileInformation"/> is not valid.</exception>
    /// <exception cref="IOException">The file could not be created due to an IO error.</exception>
    void Build(TFileInformation fileInformation, bool overwrite);

    /// <summary>
    /// Checks whether the specified file information is valid for the <see cref="IFileBuilder{TData,TFileInformation}"/>.
    /// </summary>
    /// <param name="fileInformation">The file information to validate</param>
    /// <returns><see langword="true"/> if the passed file information are valid; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="fileInformation"/> is <see langword="null"/>.</exception>
    public bool ValidateFileInformation(TFileInformation fileInformation);
}