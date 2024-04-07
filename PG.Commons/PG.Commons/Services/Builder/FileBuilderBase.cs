// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO;
using System.IO.Abstractions;
using AnakinRaW.CommonUtilities.FileSystem;
using PG.Commons.Files;

namespace PG.Commons.Services.Builder;

/// <summary>
/// Base class for a <see cref="IFileBuilder{TData,TFileInformation}"/> service providing the fundamental implementations.
/// </summary>
/// <typeparam name="TFileInformation">The type of the file information data.</typeparam>
/// <typeparam name="TData">The type of the data to build files from.</typeparam>
public abstract class FileBuilderBase<TData, TFileInformation> : ServiceBase, IFileBuilder<TData, TFileInformation>
    where TFileInformation : PetroglyphFileInformation 
    where TData : notnull
{
    /// <inheritdoc />
    public abstract TData BuilderData { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileBuilderBase{TData,TFileInformation}"/> class.
    /// </summary>
    /// <param name="services">The service provider.</param>
    protected FileBuilderBase(IServiceProvider services) : base(services)
    {
    }

    /// <inheritdoc />
    public void Build(TFileInformation fileInformation, bool overwrite)
    {
        ThrowIfDisposed();

        if (fileInformation is null)
            throw new ArgumentNullException(nameof(fileInformation));

        // Prevent races by creating getting a copy of the current state
        var data = BuilderData;


        if (!ValidateFileInformationCore(fileInformation, data, out var reason))
            throw new InvalidOperationException($"Provided file parameters are not valid for this builder: {reason}");

        var fileInfo = FileSystem.FileInfo.New(fileInformation.FilePath);

        // file path points to a directory
        // NB: This is not a full inclusive check. We leave it up to the file system to throw exceptions if something is still invalid.
        if (string.IsNullOrEmpty(fileInfo.Name) || fileInfo.Directory is null)
            throw new IOException("Specified file information contains an invalid file path.");

        var fullPath = fileInfo.FullName;

        if (!overwrite && fileInfo.Exists)
            throw new IOException($"The file '{fullPath}' already exists.");

        fileInfo.Directory.Create();


        using var tmpFileStream = FileSystem.File.CreateRandomHiddenTemporaryFile(fileInfo.DirectoryName);
        BuildFileCore(tmpFileStream, fileInformation, data);

        using var destinationStream = FileSystem.FileStream.New(fullPath, FileMode.Create, FileAccess.Write);
        tmpFileStream.Seek(0, SeekOrigin.Begin);
        tmpFileStream.CopyTo(destinationStream);
    }

    /// <summary>
    /// Writes the data of this builder into the specified file stream.
    /// </summary>
    /// <param name="fileStream">The file stream to write into.</param>
    /// <param name="fileInformation">The file information specified by the <see cref="Build"/> method.</param>
    /// <param name="data">The data to write into the file stream.</param>
    protected abstract void BuildFileCore(FileSystemStream fileStream, TFileInformation fileInformation, TData data);

    /// <inheritdoc />
    public bool ValidateFileInformation(TFileInformation fileInformation)
    {
        if (fileInformation == null) 
            throw new ArgumentNullException(nameof(fileInformation));
        return ValidateFileInformationCore(fileInformation, BuilderData, out _);
    }

    /// <summary>
    /// Checks whether the specified file information is valid for this <see cref="IFileBuilder{TData,TFileInformation}"/>.
    /// </summary>
    /// <param name="fileInformation">The file information to validate.</param>
    /// <param name="builderData">The data of this builder for additional context.</param>
    /// <param name="failedReason">Stores an optional message into the variable to reason the validation result.</param>
    /// <returns><see langowrd="true"/> if <paramref name="fileInformation"/> is valid; otherwise, <see langowrd="false"/>.</returns>
    protected abstract bool ValidateFileInformationCore(TFileInformation fileInformation, TData builderData,
        out string? failedReason);
}