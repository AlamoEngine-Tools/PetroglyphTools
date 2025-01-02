// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO;
using System.IO.Abstractions;
using AnakinRaW.CommonUtilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace PG.Commons.Files;

/// <inheritdoc cref="IPetroglyphFileHolder{TModel,TFileInfo}"/>
public abstract class PetroglyphFileHolder<TModel, TFileInfo> : DisposableObject, IPetroglyphFileHolder<TModel, TFileInfo>
    where TModel : notnull
    where TFileInfo : PetroglyphFileInformation
{
    private TFileInfo? _internalFileInformation;

    /// <inheritdoc />
    public string Directory { get; }

    /// <inheritdoc />
    public string FileName { get; }

    /// <inheritdoc />
    public TFileInfo FileInformation
    {
        get
        {
            var originalParams = _internalFileInformation;
            if (originalParams is null)
                throw new ObjectDisposedException(GetType().Name);

            // Return copy, so that we can safely dispose the returned instance without affecting the original value.
            return originalParams with { };
        }
    }

    object IPetroglyphFileHolder.Content => Content;

    PetroglyphFileInformation IPetroglyphFileHolder.FileInformation => FileInformation;

    /// <inheritdoc cref="Content"/>
    public TModel Content { get; }

    /// <inheritdoc />
    public string FilePath { get; }

    /// <summary>
    /// The logger of this service.
    /// </summary>
    protected internal ILogger Logger { get; }

    /// <summary>
    /// The file system implementation to be used.
    /// </summary>
    protected internal IFileSystem FileSystem { get; }

    /// <summary>
    /// Returns the service provider.
    /// </summary>
    protected internal IServiceProvider Services { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PetroglyphFileHolder{TModel,TParam}" /> class with the specified model and file information.
    /// </summary>
    /// <remarks>
    /// <paramref name="fileInformation"/> can be safely disposed after initialization without affecting <see cref="FileInformation"/>.
    /// </remarks>
    /// <param name="model">The data model of this holder.</param>
    /// <param name="fileInformation">The file information for this holder.</param>
    /// <param name="serviceProvider">The <see cref="IServiceProvider" /> for this instance.</param>
    /// <exception cref="ArgumentNullException"><paramref name="model"/> or <paramref name="fileInformation"/> or <paramref name="serviceProvider"/> is <see langword="null" />.</exception>
    /// <exception cref="FileNotFoundException">The underlying local file of the <see cref="IPetroglyphFileHolder{TModel,TFileInfo}"/> does not exist.</exception>
    protected PetroglyphFileHolder(TModel model, TFileInfo fileInformation, IServiceProvider serviceProvider)
    {
        if (model == null) 
            throw new ArgumentNullException(nameof(model));
        if (fileInformation == null) 
            throw new ArgumentNullException(nameof(fileInformation));

        Content = model;
        Services = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        FileSystem = serviceProvider.GetRequiredService<IFileSystem>();
        Logger = serviceProvider.GetService<ILoggerFactory>()?.CreateLogger(GetType()) ?? NullLogger.Instance;

        var fileInfo = FileSystem.FileInfo.New(fileInformation.FilePath);

        // We got a path with trailing path separator which is treated as a directory path.
        if (string.IsNullOrEmpty(fileInfo.Name))
            throw new ArgumentException($"The specified path '{fileInfo.FullName}' is not a valid file path.");

        var fileName = fileInfo.Name;
        ThrowHelper.ThrowIfNullOrEmpty(fileName);

        FileName = fileInfo.Name;

        if (fileInformation is PetroglyphMegPackableFileInformation { IsInsideMeg: true })
        {
            FilePath = fileInformation.FilePath;
            Directory = FileSystem.Path.GetDirectoryName(fileInformation.FilePath) ??
                        throw new InvalidOperationException($"No directory found for file '{FilePath}'");

            _internalFileInformation = fileInformation with { };
        }
        else
        {
            // We can only check whether the file exists if it is a local file.
            if (!fileInfo.Exists)
                throw new FileNotFoundException($"File '{fileInfo.FullName}' not found.", fileInfo.FullName);

            // Use absolute paths for local files.
            FilePath = fileInfo.FullName;
            Directory = fileInfo.DirectoryName ??
                        throw new InvalidOperationException($"No directory found for file '{FilePath}'");

            // Create a copy with the full file path.
            _internalFileInformation = fileInformation with { FilePath = FilePath };
        }
    }

    /// <inheritdoc />
    protected sealed override void DisposeManagedResources()
    {
        base.DisposeManagedResources();
        _internalFileInformation?.Dispose();
        _internalFileInformation = null!;
        if (Content is IDisposable disposableContent)
            disposableContent.Dispose();
    }
}