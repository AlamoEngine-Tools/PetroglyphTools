// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using PG.Commons.DataTypes;

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

    /// <inheritdoc />
    public TModel Content { get; }

    /// <inheritdoc />
    public string FilePath { get; }

    /// <summary>
    ///     The logger of this service.
    /// </summary>
    protected internal ILogger Logger { get; }

    /// <summary>
    ///     The file system implementation to be used.
    /// </summary>
    protected internal IFileSystem FileSystem { get; }

    /// <summary>
    ///     The service provider.
    /// </summary>
    protected internal IServiceProvider Services { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="PetroglyphFileHolder{TModel,TParam}" /> class.
    /// </summary>
    /// <param name="model">The data model of this holder.</param>
    /// <param name="param">The creation param.</param>
    /// <param name="serviceProvider">The <see cref="IServiceProvider" /> for this instance.</param>
    /// <exception cref="ArgumentNullException">When any parameter is <see langword="null" />.</exception>
    protected PetroglyphFileHolder(TModel model, TFileInfo param, IServiceProvider serviceProvider)
    {
        if (model == null) 
            throw new ArgumentNullException(nameof(model));
        if (param == null) 
            throw new ArgumentNullException(nameof(param));

        Content = model;
        Services = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        FileSystem = serviceProvider.GetRequiredService<IFileSystem>();
        Logger = serviceProvider.GetService<ILoggerFactory>()?.CreateLogger(GetType()) ?? NullLogger.Instance;

        // We do not use FileNameUtilities.IsValidFileName() cause don't want a custom PG as a dependency.
        // Instead, we check whether given file is valid in general on the current filesystem.
        var fileInfo = FileSystem.FileInfo.New(param.FilePath);
        
        FilePath = fileInfo.FullName;

        // Empty file names such as "Data/.meg" are allowed in general. Thus, we don't check for this constraint here.
        // The concrete holder can check for possible file name constraints.
        FileName = fileInfo.Name;

        // Remember: For a file path "myfile.txt" the path is empty but not null.
        Directory = fileInfo.DirectoryName ??
                    throw new InvalidOperationException($"No directory found for file '{FilePath}'");

        _internalFileInformation = param with { FilePath = FilePath };
    }

    /// <inheritdoc />
    protected sealed override void DisposeManagedResources()
    {
        base.DisposeManagedResources();
        _internalFileInformation?.Dispose();
        _internalFileInformation = null!;
    }
}